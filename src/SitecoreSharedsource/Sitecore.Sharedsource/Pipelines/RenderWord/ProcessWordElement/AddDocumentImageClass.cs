namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using Sitecore.Diagnostics;

  public class AddDocumentImageClass : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");

      if (args.Element.Name != "img")
      {
        return;
      }

      args.Element.SetAttributeValue("class", "documentimage");
    }
  }
}