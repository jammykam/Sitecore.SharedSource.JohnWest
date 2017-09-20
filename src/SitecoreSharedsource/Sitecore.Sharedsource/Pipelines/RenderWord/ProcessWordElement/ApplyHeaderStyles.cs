namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using Sitecore.Diagnostics;

  public class ApplyHeaderStyles : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");

      if (args.Element.Name != "p")
      {
        return;
      }

      string value = args.Element.GetAttributeValue("class", string.Empty);

      if (value == "heading1")
      {
        args.Element.Name = "h2";
        args.Element.Attributes.RemoveAll();
      }
      else if (value == "heading2")
      {
        args.Element.Name = "h3";
        args.Element.Attributes.RemoveAll();
      }
      else if (value == "heading3")
      {
        args.Element.Name = "h4";
        args.Element.Attributes.RemoveAll();
      }
      else if (value == "heading4")
      {
        args.Element.Name = "h5";
        args.Element.Attributes.RemoveAll();
      }
    }
  }
}