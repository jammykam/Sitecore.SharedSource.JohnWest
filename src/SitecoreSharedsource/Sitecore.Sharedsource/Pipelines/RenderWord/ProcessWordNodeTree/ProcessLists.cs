namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using System;
  using System.Linq;

  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class ProcessLists : ProcessWordNodeTreeProcessor
  {
    private void RemoveLeader(HtmlNode li)
    {
      Assert.ArgumentNotNull(li, "li");
      int discard;

      // ToArray() prevents illegal modification of lists during loops
      foreach (HtmlNode checkSpan in li.ChildNodes.ToArray())
      {
        if (checkSpan.NodeType == HtmlNodeType.Element
          && checkSpan.Name == "span"
          && checkSpan.ChildNodes[0].InnerText != "."
          && (checkSpan.InnerText == "o" // "bullet"
          || (checkSpan.ChildNodes[0].InnerText.Length < 4 // the rest checks for numbers 2 digits or less in 1. or 1) format
            && (checkSpan.ChildNodes[0].InnerText.EndsWith(".") || (checkSpan.ChildNodes[0].InnerText.EndsWith(")") && int.TryParse(checkSpan.ChildNodes[0].InnerText.TrimEnd('.', ')'), out discard))))))
        {
          checkSpan.Remove();
          break;
        }
      }
    }

    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");
      string outerClass = args.CurrentElement.GetAttributeValue("class", string.Empty);

      // only process outer lists
      if (args.CurrentElement.Name != "p"
        || (outerClass != "sitecorenumberedlist" && outerClass != "sitecorebulletedlist"))
      {
        return;
      }

      // should be a 1., 1), or o. or something
      this.RemoveLeader(args.CurrentElement);

      // ol or ul element to wrap the list
      HtmlNode list = args.CurrentElement.OwnerDocument.CreateElement(outerClass == "sitecorenumberedlist" ? "ol" : "ul");

      // move the paragraph into the list
      args.CurrentElement.ParentNode.InsertBefore(list, args.CurrentElement);
      args.CurrentElement.Remove();
      args.CurrentElement.Name = "li";

      // add indented things to this <li> until the next item in that outer nested list. 
      HtmlNode outerLi = args.CurrentElement;
      outerLi.Attributes.RemoveAll();
      list.ChildNodes.Add(outerLi);

      if (args.NextElement == null)
      {
        // this isn't really an exception, but a list should have more than one item
        return;
      }

      // the class of the next paragraph indicates whether to 
      // add it to the li, create a new li, or close the outer list.
      string nextClass = args.NextElement.GetAttributeValue("class", string.Empty);

      // until we (probably?) need to close the outer list
      while (args.NextElement.Name == "p"
        && (!string.IsNullOrEmpty(nextClass))
        && (nextClass == "sitecorebullet2ndlevel"
          || nextClass == "sitecorenoteheaderindent"
          || nextClass == "sitecorenoteheaderindent2ndlevel"
          || nextClass == "sitecorenumberedlist2ndlevel"
          || nextClass == "sitecoreindent"
          || nextClass == outerClass))
      {
        // the class of the following element could indicates whether to 
        // add it to the inner list, add an li in the inner list, add 
        // an li to the outer list, or close the outer list.
        HtmlNode followingElement = args.GetNextElement(args.NextElement);

        switch (nextClass)
        {
          // convert <p> representing outer nested list item and add to outerLi
          case "sitecorenumberedlist":
          case "sitecorebulletedlist":
            RemoveLeader(args.NextElement);
            args.NextElement.Name = "li";
            outerLi = args.NextElement.Clone();
            outerLi.Attributes.RemoveAll();
            args.NextElement.Remove();
            list.ChildNodes.Add(outerLi);
            args.NextElement = followingElement;
            break;

          // add this <p> to outerLi
          case "sitecoreindent":
            args.NextElement.Attributes.Remove("class"); // because it's already in an li
            args.NextElement.Remove();
            outerLi.ChildNodes.Add(args.NextElement.Clone());
            args.NextElement = followingElement;
            break;

          // add an outer list
          case "sitecorebullet2ndlevel":
          case "sitecorenumberedlist2ndlevel":
            HtmlNode innerList = args.NextElement.OwnerDocument.CreateElement(
            nextClass == "sitecorebullet2ndlevel" ? "ul" : "ol");
            outerLi.ChildNodes.Add(innerList);
            string outerListClass = nextClass;
            HtmlNode innerLi = null;

            // while the next paragraph belongs to this inner <li>
            while (nextClass == outerListClass
              || nextClass == "sitecoreindent2ndlevel"
              || nextClass == "sitecorenoteheaderindent2ndlevel")
            {
              if (nextClass == "sitecorenoteheaderindent2ndlevel")
              {
                HtmlNode section = args.NextElement.OwnerDocument.CreateElement("section");
                section.SetAttributeValue("class", "note");
                innerLi.ChildNodes.Add(section);
                args.NextElement.Remove();
                section.ChildNodes.Add(args.NextElement.Clone());
                args.NextElement = followingElement;

                if (args.NextElement != null)
                {
                  followingElement = args.GetNextElement(args.NextElement);
                }

                nextClass = args.NextElement.GetAttributeValue("class", string.Empty);

                // add subsequent <p> note bodys to this section 
                while (nextClass == "sitecorenotebodyindent2ndlevel")
                {
                  args.NextElement.Remove();
                  section.ChildNodes.Add(args.NextElement.Clone());
                  args.NextElement = followingElement;

                  if (args.NextElement == null)
                  {
                    break;
                  }

                  nextClass = args.NextElement.GetAttributeValue("class", string.Empty);
                  followingElement = args.GetNextElement(args.NextElement);
                }

                // no remaining <p> note bodys
              }
              else
              {
                this.RemoveLeader(args.NextElement);

                if (nextClass == "sitecoreindent2ndlevel")
                {
                  // I (no longer) believe [that] the next loop will move the current <p> to the current innerLi
                  Assert.IsNotNull(innerLi, "innerLi");
                  //TODO:JW23april                  nextElement.Remove();
                  innerLi.ChildNodes.Add(args.NextElement.Clone());
                  args.NextElement.Remove();
                }
                else
                {
                  // I believe we're done with this inner <li>
                  args.NextElement.Remove();
                  args.NextElement.Name = "li";
                  innerLi = args.NextElement.Clone();
                  innerLi.Attributes.RemoveAll();
                  innerList.ChildNodes.Add(innerLi);
                }

                // the next inner while will process the next element or terminate
                args.NextElement = followingElement;

                if (args.NextElement == null)
                {
                  break;
                }

                followingElement = args.GetNextElement(args.NextElement);
                nextClass = args.NextElement.GetAttributeValue("class", string.Empty);
              }
            }

            break;
          case "sitecorenoteheaderindent":

            // Create <section class="note">, move <p>s into <section>. 
            HtmlNode indentSection = args.NextElement.OwnerDocument.CreateElement("section");
            indentSection.SetAttributeValue("class", "note");
            outerLi.ChildNodes.Add(indentSection);
            args.NextElement.Remove();
            indentSection.ChildNodes.Add(args.NextElement.Clone());
            args.NextElement = followingElement;

            if (args.NextElement != null)
            {
              followingElement = args.GetNextElement(args.NextElement);
            }

            nextClass = args.NextElement.GetAttributeValue("class", string.Empty);

            // add subsequent <p> note bodys to this section 
//            while (nextClass == "Sitecore-Note-Body")
            while (nextClass == "sitecorenotebodyindent")
            {
              args.NextElement.Remove();
              indentSection.ChildNodes.Add(args.NextElement.Clone());
              args.NextElement = followingElement;

              if (args.NextElement == null)
              {
                break;
              }

              nextClass = args.NextElement.GetAttributeValue("class", string.Empty);
              followingElement = args.GetNextElement(args.NextElement);
            }

            break;
          default:
            // i believe the outer while loop makes this condition impossible
            throw new Exception("impossible condition");
        }

        if (args.NextElement == null)
        {
          // to outer while
          break;
        }

        // next inner while will terminate for any following <p>
        // elements not relevant to this innerLi
        nextClass = args.NextElement.GetAttributeValue("class", string.Empty);
      }

      args.AbortPipeline();
    }
  }
}