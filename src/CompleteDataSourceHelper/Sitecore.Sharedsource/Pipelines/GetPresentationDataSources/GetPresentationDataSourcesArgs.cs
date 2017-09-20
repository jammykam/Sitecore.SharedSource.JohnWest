namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using System.Collections.Generic;
  using System.Linq;

  using Assert = Sitecore.Diagnostics.Assert;
  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class GetPresentationDataSourcesArgs : SC.Pipelines.PipelineArgs
  {
    public List<SC.Data.Items.Item> DataSourceItems { get; set; }

    public IQueryable<CS.SearchTypes.SitecoreUISearchResultItem> UISearchResults { get; set; }

    public SC.Data.Items.Item ContextItem { get; private set; }

    public string[] SearchResultIDs { get; set; }

    public string RawDataSource { get; private set; }

    public CS.IProviderSearchContext SearchContext { get; set; }

    public bool StoreSearchResultIDs { get; private set; }

    public bool StoreUISearchResults { get; private set; }

    public GetPresentationDataSourcesArgs(
      string dataSource,
      SC.Data.Items.Item contextItem,
      bool storeUISearchResults,
      bool storeSearchResultIDs,
      CS.IProviderSearchContext searchContext)
    {
      Assert.ArgumentNotNull(contextItem, "contextItem");
      this.RawDataSource = dataSource;
      this.ContextItem = contextItem;
      this.StoreUISearchResults = storeUISearchResults;
      this.StoreSearchResultIDs = storeSearchResultIDs;
      this.SearchContext = searchContext;
    }
  }
}