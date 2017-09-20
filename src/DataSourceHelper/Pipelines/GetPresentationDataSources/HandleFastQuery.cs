namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using SC = Sitecore;

  public class HandleFastQuery
    : SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(GetPresentationDataSourcesArgs args)
    {
      if (!(args.RawDataSource.StartsWith("query:fast:") 
        || args.RawDataSource.StartsWith("fast:")))
      {
        return;
      }

      string query = args.RawDataSource;

      if (query.StartsWith("query:fast:"))
      {
        query = query.Substring("query:fast:".Length);
      }

      if (query.StartsWith("fast:"))
      {
        query = query.Substring("fast:".Length);
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

      args.DataSourceItems.AddRange(item.Database.SelectItems("fast:" + query));
    }
  }
}