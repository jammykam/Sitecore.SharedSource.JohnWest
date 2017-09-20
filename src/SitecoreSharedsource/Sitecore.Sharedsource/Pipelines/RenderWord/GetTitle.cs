namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using Sitecore.Diagnostics;

  public class GetTitle : RenderWordProcessor
  {
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.WordDoc, "args.WordDoc");
      args.Title = args.WordDoc.PackageProperties.Title;
    }
  }
}