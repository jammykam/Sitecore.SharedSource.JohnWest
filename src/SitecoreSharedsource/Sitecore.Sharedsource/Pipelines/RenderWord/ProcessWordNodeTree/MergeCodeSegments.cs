namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using System;

  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class MergeCodeSegments : ProcessWordNodeTreeProcessor
  {
    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");

      if (args.CurrentElement.Name != "p"
        || args.CurrentElement.GetAttributeValue("class", string.Empty) != "sitecorecodesample")
      {
        return;
      }

      HtmlNode pre = args.CurrentElement;

      ////TODO: assumes no inner elements
      string text = Environment.NewLine + args.CurrentElement.InnerText;

      while (args.NextElement != null
        && args.NextElement.Name == "p"
        && args.NextElement.GetAttributeValue("class", string.Empty) == "sitecorecodesample")
      {
        text += Environment.NewLine + args.NextElement.InnerText.Replace("<", "&lt;").Replace(">", "&gt;");
        HtmlNode temp = args.NextElement;
        args.NextElement = args.GetNextElement(args.NextElement);
        temp.Remove();
      }

      pre.RemoveAllChildren();
      pre.Name = "pre";
      pre.SetAttributeValue("class", "prettyprint");
      HtmlNode code = args.CurrentElement.OwnerDocument.CreateElement("code");
      pre.ChildNodes.Add(code);
      code.ChildNodes.Add(HtmlNode.CreateNode(text + Environment.NewLine));
      args.AbortPipeline();
    }
  }
}