namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using System.Linq;

  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class MergeRedundantElements : ProcessWordNodeTreeProcessor
  {
    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");
      this.Process(args.CurrentElement, args);
    }

    private void Process(HtmlNode node, ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(node, "node");

      // if the element to process is of one of the types to merge
      if (node.NodeType == HtmlNodeType.Element
        && (node.Name == "em" || node.Name == "strong" || node.Name == "code"))
      {
        ////TODO: assumes node contains no elements

        // start with the text of the first node, then
        // append text from subsequent nodes of the same type
        string text = node.InnerText;
        HtmlNode looper = args.GetNextElement(node);

        // while there is a next element of the same type
        while (looper != null && looper.Name == node.Name)
        {
          text += looper.InnerText;
          HtmlNode temp = looper;
          looper = args.GetNextElement(looper);
          temp.Remove();
        }

        node.RemoveAllChildren();

        ////TODO: replace leading spaces with nbsps?
        node.ChildNodes.Add(HtmlTextNode.CreateNode(text));
      }

      // recursion
      foreach (HtmlNode child in node.ChildNodes.ToArray())
      {
        // if it hasn't already been removed (such as a secondary <code> element)
        if (child.ParentNode != null)
        {
          this.Process(child, args);
        }
      }
    }
  }
}