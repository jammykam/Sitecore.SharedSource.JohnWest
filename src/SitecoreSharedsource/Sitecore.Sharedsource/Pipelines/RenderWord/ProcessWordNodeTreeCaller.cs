namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using HtmlAgilityPack;

  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree;

  public class ProcessWordNodeTreeCaller : RenderWordProcessor
  {
    //TODO: ensure adding things to body, not root
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.HtmlDoc, "args.HtmlDoc");
      Assert.ArgumentNotNull(args.HtmlDoc.DocumentNode, "args.HtmlDoc.DocumentNode");

      //TODO: this should not be necessary; may be due to no <html> element or wrong method for adding elements
      args.HtmlDoc.LoadHtml(args.HtmlDoc.DocumentNode.WriteTo());

      foreach (HtmlNode child in args.HtmlDoc.DocumentNode.ChildNodes)
      {
        if (child.NodeType != HtmlNodeType.Element || child.Name != "body")
        {
          continue;
        }

        foreach (HtmlNode grandchild in child.ChildNodes)
        {
          if (grandchild.NodeType != HtmlNodeType.Element)
          {
            continue;
          }

          ProcessWordNodeTreeArgs innerArgs = new ProcessWordNodeTreeArgs(grandchild);
          CorePipeline.Run("processWordNodeTree", innerArgs);
          HtmlNode next = innerArgs.NextElement;

          if (innerArgs.TokenTable != null)
          {
            args.TokenTable = innerArgs.TokenTable;
          }

          while (next != null)
          {
            ProcessWordNodeTreeArgs innerInnerArgs = new ProcessWordNodeTreeArgs(next);
            CorePipeline.Run("processWordNodeTree", innerInnerArgs);

            if (innerInnerArgs.TokenTable != null)
            {
              Assert.IsNull(args.TokenTable, "multiple token tables");
              args.TokenTable = innerInnerArgs.TokenTable;
            }

            next = innerInnerArgs.NextElement;
          }

          break;
        }
      }
    }
  }
}