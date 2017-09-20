namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class TrimText : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");

      foreach (HtmlNode child in args.Element.ChildNodes)
      {
        HtmlTextNode text = child as HtmlTextNode;

        if (text == null)
        {
          continue;
        }

        string trimmed = child.InnerText.TrimEnd();

        if (child.InnerText != trimmed && child.InnerText != " ")
        {
          text.Text = trimmed;
        }
      }
    }
  }
}