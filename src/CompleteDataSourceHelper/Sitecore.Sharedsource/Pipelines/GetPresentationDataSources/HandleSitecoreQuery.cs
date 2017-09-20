namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using System.Collections.Generic;

  using SC = Sitecore;

  public class HandleSitecoreQuery : 
    SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
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

      if (args.DataSourceItems == null)
      {
        args.DataSourceItems = new List<SC.Data.Items.Item>();
      }

      args.DataSourceItems.AddRange(item.Database.SelectItems(query));
    }  
  }
}