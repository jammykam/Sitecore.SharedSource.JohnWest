namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Web;
  using System.IO;

  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  // WARNING: THE SAME CLASS NAMES SEEM TO APPEAR IN NUMEROUS NAMESPACES
  // WITHIN THE DocumentFormat.OpenXml API. ADDING USING DIRECTIVES FOR 
  // NAMESPACES OTHER THAN THESE TWO MOST COMMONLY USED COULD RAISE CHALLENGES.

  using DocumentFormat.OpenXml;
  using DocumentFormat.OpenXml.Wordprocessing;
  using DocumentFormat.OpenXml.Packaging;

  using Blip = DocumentFormat.OpenXml.Drawing.Blip;
  using DocProperties = DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties;

  public class ConvertToMarkup : RenderWordProcessor
  {
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.WordDoc, "args.WordDoc");

      // create a root <body> element for an XML structure
      args.HtmlDoc = new HtmlDocument();
      HtmlNode htmlBody = args.HtmlDoc.CreateElement("body");
      args.HtmlDoc.DocumentNode.ChildNodes.Add(htmlBody);
      Body body = args.WordDoc.MainDocumentPart.Document.Body;

      // for each element in the Word document
      foreach (OpenXmlElement current in body.ChildElements)
      {
        Paragraph paragraph = current as Paragraph;

        // Word treats almost everything (list items, etc.) as paragraphs.
        if (paragraph != null)
        {
          // the name of the style, which can also identify what should be <li> 
          // or could be used as or to determine values for the class attribute
          string style = null;

          if (paragraph.ParagraphProperties != null && paragraph.ParagraphProperties.ParagraphStyleId != null)
          {
            style = paragraph.ParagraphProperties.ParagraphStyleId.Val.ToString().ToLower();
          }

          // the text of the "paragraph"
          string text = string.Empty;

          // for each thing in the paragraph
          foreach (var child in paragraph.ChildElements)
          {
            // ignore paragraph properties.
            if (child is ParagraphProperties)
            {
              continue;
            }

            // some paragraphs may consist of a single run in a single style
            SdtRun sdtRun = child as SdtRun;

            if (sdtRun != null)
            {
              text += HttpUtility.HtmlEncode(sdtRun.InnerText);

              if (sdtRun.HasChildren)
              {
                //TODO: process images and runs?
              }

              continue;
            }

            // outer for-each invokes this for each run in the paragraph
            Run run = child as Run;

            if (run != null)
            {
              // if the run has a specific style, add it as an element name around the text
              if (run.RunProperties != null && run.RunProperties.RunStyle != null)
              {
                string name = run.RunProperties.RunStyle.Val.ToString().ToLower();
                text += "<" + name + ">" + HttpUtility.HtmlEncode(run.InnerText) + "</" + name + ">";
              }
              else
              {
                text += HttpUtility.HtmlEncode(run.InnerText);
              }

              foreach (Drawing drawing in run.Descendants<Drawing>())
              {
                string alt = null;
                string title = null;

                // path to image file on disk
                string file = null;
                string width = null;
                string height = null;

                // for each drawing in the run
                foreach (DocumentFormat.OpenXml.Drawing.Extents extent
                  in drawing.Descendants<DocumentFormat.OpenXml.Drawing.Extents>())
                {
                  if (extent.Cx != null)
                  {
                    width = this.GetPixels(extent.Cx.ToString()).ToString(CultureInfo.InvariantCulture);
                  }

                  if (extent.Cy != null)
                  {
                    height = this.GetPixels(extent.Cy.ToString()).ToString(CultureInfo.InvariantCulture);
                  }
                }

                // there is probably only one DocProperties per image
                // retrieve alt text, title, and part of file name if possible
                foreach (DocProperties docProps in drawing.Descendants<DocProperties>())
                {
                  if (!string.IsNullOrEmpty(docProps.Title))
                  {
                    title = docProps.Title;
                  }

                  if (!string.IsNullOrEmpty(docProps.Description))
                  {
                    alt = docProps.Description;
                  }

                  if (!string.IsNullOrEmpty(docProps.Name))
                  {
                    file = docProps.Name;
                  }

                  if (string.IsNullOrEmpty(alt))
                  {
                    if (string.IsNullOrEmpty(title))
                    {
                      if (!string.IsNullOrEmpty(file))
                      {
                        alt = file;
                      }
                    }
                    else
                    {
                      alt = title;
                    }
                  }
                }

                string unique = null;

                foreach (Blip blip
                  in drawing.Descendants<Blip>())
                {
                  OpenXmlAttribute attribute = blip.GetAttribute(
                    "embed",
                    "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

                  if (attribute != null)
                  {
                    Assert.IsNull(unique, "unique");
                    unique = attribute.Value;
                    ImagePart imagePart = (ImagePart)args.WordDoc.MainDocumentPart.GetPartById(attribute.Value);
                    Assert.IsNotNull(imagePart, "imagePart");
                  }
                  else
                  {
                    Log.Info("no embed", this);
                  }
                }

                if (!string.IsNullOrEmpty(unique))
                {
                  string path = this.WriteImageFile(args.WordDoc, "C:\\temp", unique, file).Replace("\\", "\\\\");
                  text += "<img src=\"" + path + "\" height=\"" + height + "\" width=\"" + width + "\" alt=\"" + alt + "\" title=\"" + title + "\" />";
                }
              }

              continue;
            }

            // ignore these types of elements for now
            if (child is ProofError
              || child is BookmarkStart
              || child is BookmarkEnd)
            {
              continue;
            }

            // identify any new types of elements
            throw new Exception("Unknown child element: " + child.GetType());
          }

          string html = "<p";

          if (!string.IsNullOrEmpty(style))
          {
            html += " class=\"" + style + "\"";
          }

          HtmlNode para = HtmlNode.CreateNode(html + ">" + text + "</p>");
          htmlBody.ChildNodes.Add(para);
          continue;
        }

        Table table = current as Table;

        if (table != null)
        {
          HtmlNode tableNode = args.HtmlDoc.CreateElement("table");
          HtmlNode tableBody = args.HtmlDoc.CreateElement("tbody");
          tableNode.ChildNodes.Add(tableBody);

          foreach (TableRow row in table.Elements<TableRow>())
          {
            HtmlNode tableRow = args.HtmlDoc.CreateElement("tr");
            tableBody.ChildNodes.Add(tableRow);

            foreach (TableCell cell in row.Elements<TableCell>())
            {
              HtmlNode tableCell = args.HtmlDoc.CreateElement("td");
              tableRow.ChildNodes.Add(tableCell);

              foreach (Paragraph para in cell.Elements<Paragraph>())
              {
                //TODO: process runs?
                tableCell.ChildNodes.Add(HtmlNode.CreateNode(para.InnerText));
              }
            }
          }

          htmlBody.ChildNodes.Add(tableNode);
          continue;
        }

        // ignore these types of elements
        if (current is SectionProperties
          || current is BookmarkEnd)
        {
          continue;
        }

        throw new Exception("Unknown element: " + current.GetType());
      }
    }

    /// <summary>
    /// Convert the number we get from Word to pixels, or -1 if not possible.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value, in pixels.</returns>
    private int GetPixels(string value)
    {
      Assert.ArgumentNotNull(value, "value");
      long num = -1;

      if (!Int64.TryParse(value, out num))
      {
        return -1;
      }

      if (num <= 0)
      {
        return -1;
      }

      return (int)Math.Round((decimal)num / 9525);
    }

    private string WriteImageFile(WordprocessingDocument document, string subdirectory, string unique, string name)
    {
      Assert.ArgumentNotNull(document, "document");
      Assert.ArgumentNotNullOrEmpty(subdirectory, "subdirectory");
      Assert.ArgumentNotNullOrEmpty(unique, "unique");
      Assert.ArgumentNotNullOrEmpty(name, "name");
      ImagePart imagePart = (ImagePart)document.MainDocumentPart.GetPartById(unique);
      Assert.IsNotNull(imagePart, "imagePart " + unique);
      Assert.IsNotNull(imagePart.Uri, "imagePart.Uri");
      string url = imagePart.Uri.ToString();
      int pos = url.LastIndexOf(".", StringComparison.Ordinal);
      Assert.IsTrue(pos > -1, "pos");
      string ext = url.Substring(pos);
      Assert.IsNotNullOrEmpty(ext, "ext");
      Stream stream = imagePart.GetStream();
      long length = stream.Length;
      byte[] byteStream = new byte[length];
      stream.Read(byteStream, 0, (int)length);
      string path = subdirectory + "\\" + name + "." + unique + ext;
      FileStream fstream = new FileStream(path, FileMode.OpenOrCreate);
      fstream.Write(byteStream, 0, (int)length);
      fstream.Close();
      return path;
    }
  }
}