namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using Sitecore.Diagnostics;

  public class MapKnownElementNames : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");

      if (args.Element.Name == "sitecoreemphasis")
      {
        args.Element.Name = "em";
        args.Element.Attributes.RemoveAll();
      }
      else if (args.Element.Name == "sitecorewindow")
      {
        args.Element.Name = "strong";
        args.Element.Attributes.RemoveAll();
      }
      else if (args.Element.Name == "sitecorecodechar")
      {
        args.Element.Name = "code";
        args.Element.Attributes.RemoveAll();
      }
    }
  }
}