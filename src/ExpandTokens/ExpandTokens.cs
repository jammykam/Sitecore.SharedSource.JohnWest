namespace Sitecore.Sharedsource.Pipelines.GetLookupSourceItems
{
  public class ExpandTokens
  {
    public void Process(
      Sitecore.Pipelines.GetLookupSourceItems.GetLookupSourceItemsArgs args)
    {
      if (args.Source.Contains("$now"))
      {
        args.Source = args.Source.Replace(
          "$now", 
          Sitecore.DateUtil.IsoNow);
      }

      if (args.Source.Contains("$id"))
      {
        args.Source = args.Source.Replace(
          "$id", 
          args.Item.ID.ToString());
      }
    }
  }
}