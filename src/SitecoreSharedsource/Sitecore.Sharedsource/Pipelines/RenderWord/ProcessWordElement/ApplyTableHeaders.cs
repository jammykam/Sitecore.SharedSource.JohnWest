namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class ApplyTableHeaders : ProcessWordElementProcessor
  {
    public override void Process(ProcessWordElementArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.Element, "args.Element");

      if (args.Element.Name != "table")
      {
        return;
      }

      foreach (HtmlNode tr in args.Element.SelectNodes(".//tr[1]"))
      {
        foreach(HtmlNode th in tr.Descendants("td"))
        {
          th.Name = "th";
        }
      }
    }
  }
}