namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using HtmlAgilityPack;

  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement;

  public class ProcessWordElements : RenderWordProcessor
  {
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.HtmlDoc, "args.HtmlDoc");
      Assert.ArgumentNotNull(
        args.HtmlDoc.DocumentNode, 
        "args.HtmlDoc.DocumentNode");
      Assert.ArgumentNotNullOrEmpty(args.Name, "args.Name");
      Assert.ArgumentNotNull(args.Path, "args.Path");
      this.Process(args.HtmlDoc.DocumentNode, args);
    }

    private void Process(HtmlNode node, RenderWordArgs args)
    {
      Assert.ArgumentNotNull(node, "node");

      if (node.NodeType == HtmlNodeType.Element)
      {
        ProcessWordElementArgs innerArgs = new ProcessWordElementArgs(
          args.Database, 
          node, 
          args.Name, 
          args.Path);
        CorePipeline.Run("processWordElement", innerArgs);
      }

      foreach (HtmlNode child in node.ChildNodes)
      {
        this.Process(child, args);
      }
    }
  }
}