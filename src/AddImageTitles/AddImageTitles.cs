namespace Sitecore.Sharedsource.Pipelines.RenderField
{
  using System;
  using System.IO;
  using System.Text;
  using System.Collections.Generic;

  public class AddImageTitles
  {
    public List<string> Types
    {
      get;
      set;
    }

    public AddImageTitles()
    {
      this.Types = new List<string>();
    }

    public void Process(Sitecore.Pipelines.RenderField.RenderFieldArgs args)
    {
      if (String.IsNullOrEmpty(args.FieldValue)
        || !this.Types.Contains(args.FieldTypeKey))
      {
        return;
      }

      HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(args.Result.FirstPart);
      HtmlAgilityPack.HtmlNodeCollection missingTitles = doc.DocumentNode.SelectNodes("//img[not(@title)]");

      if (missingTitles == null || missingTitles.Count < 1)
      {
        return;
      }

      foreach (HtmlAgilityPack.HtmlNode img in missingTitles)
      {
        string title = img.GetAttributeValue("alt", String.Empty);

        if (String.IsNullOrEmpty(title))
        {
          title = img.GetAttributeValue("longdesc", String.Empty);
        }

        if (!String.IsNullOrEmpty(title))
        {
          img.SetAttributeValue("title", title);
        }
      }

      StringBuilder sb = new StringBuilder();
      StringWriter sw = new StringWriter(sb);
      doc.Save(sw);
      sw.Close();
      args.Result.FirstPart = sb.ToString();
    }
  }
}