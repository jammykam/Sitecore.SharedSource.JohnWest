namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using Sitecore.Diagnostics;

  public class ExtractTitle : ProcessWordNodeTreeProcessor
  {
    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");

      if (args.CurrentElement.Name != "p"
        || args.CurrentElement.GetAttributeValue("class", string.Empty) != "sitecoretopictitle")
      {
        return;
      }

      args.CurrentElement.Remove();
      args.AbortPipeline();
    }
  }
}