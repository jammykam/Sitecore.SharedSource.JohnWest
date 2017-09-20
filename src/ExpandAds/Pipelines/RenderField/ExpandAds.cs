namespace Sitecore.Sharedsource.Pipelines.RenderField
{
  using System.Text.RegularExpressions;
  using Sitecore.Data.Items;

  public class ExpandAds
  {
    private Regex _regex = new Regex(@"\$ad\s*\(\s*[""']?([^)""']+)[""']?\s*\)");

    public void Process(Sitecore.Pipelines.RenderField.RenderFieldArgs args)
    {
      if (Sitecore.Context.PageMode.IsPageEditorEditing)
      {
        return;
      }

      // for efficiencly, filter by field type first
      args.Result.FirstPart = this.Replace(args.Result.FirstPart);
      args.Result.LastPart = this.Replace(args.Result.LastPart);
    }

    protected string Replace(string input)
    {
      // slightly inefficient if the user inserts the same ad twice
      foreach (Match match in this._regex.Matches(input))
      {
        Item item = Sitecore.Context.Database.GetItem(match.Groups[1].Value);
        Sitecore.Diagnostics.Assert.IsNotNull(item, match.Groups[1].Value);
        input = input.Replace(match.Groups[0].Captures[0].Value, item["Content"]);
      }

      return input;
    }
  }
}