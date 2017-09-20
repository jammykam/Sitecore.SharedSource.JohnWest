namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using Sitecore.Diagnostics;

  public class LowercaseClassNames : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");
      string value = args.Element.GetAttributeValue("class", string.Empty);

      if (string.IsNullOrEmpty(value))
      {
        return;
      }

      args.Element.SetAttributeValue("class", value.ToLower());
    }
  }
}