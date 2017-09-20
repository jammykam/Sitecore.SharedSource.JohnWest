namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class ProcessNotes : ProcessWordNodeTreeProcessor
  {
    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");

      if (args.CurrentElement.Name != "p"
        || args.CurrentElement.GetAttributeValue("class", string.Empty) != "sitecorenoteheader")
      {
        return;
      }

      HtmlNode section = args.CurrentElement.OwnerDocument.CreateElement("section");
      section.SetAttributeValue("class", "note");
      args.CurrentElement.ParentNode.InsertBefore(section, args.CurrentElement);
      section.ChildNodes.Add(args.CurrentElement.Clone());
      args.CurrentElement.Remove();

      while (args.NextElement != null
        && args.NextElement.NodeType == HtmlNodeType.Element
        && args.NextElement.Name == "p"
        && args.NextElement.GetAttributeValue("class", string.Empty) == "sitecorenotebody")
      {
        section.ChildNodes.Add(args.NextElement.Clone());
        HtmlNode temp = args.NextElement;
        args.NextElement = args.GetNextElement(args.NextElement);
        temp.Remove();
      }

      args.AbortPipeline();
    }
  }
}