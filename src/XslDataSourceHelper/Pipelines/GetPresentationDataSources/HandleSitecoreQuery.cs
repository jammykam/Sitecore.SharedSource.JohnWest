namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using SC = Sitecore;

  public class HandleSitecoreQuery
    : SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(GetPresentationDataSourcesArgs args)
    {
      string query = args.RawDataSource;

      if (!(query.StartsWith("query:") || query.StartsWith(".")))
      {
        return;
      }

      if (query.StartsWith("query:"))
      {
        query = query.Substring("query:".Length);
      }

      SC.Data.Items.Item item = args.ContextItem;

      while (query.StartsWith("../"))
      {
        item = item.Parent;
        query = query.Substring("../".Length);
      }

      if (query.StartsWith("./"))
      {
        query = query.Substring("./".Length);
      }

      if (!query.StartsWith("/"))
      {
        query = item.Paths.FullPath + "/" + query;
      }

      args.DataSourceItems.AddRange(item.Database.SelectItems(query));
    }  
  }
}