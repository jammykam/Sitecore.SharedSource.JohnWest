namespace Sitecore.Sharedsource.Web.UI
{
  using System.Collections.Generic;

  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public abstract class WebControl : SC.Web.UI.WebControl
  {
    private SC.Sharedsource.Web.UI.DataSourceHelper _dataSourceHelper;

    protected DataSourceHelper DataSourceHelper
    {
      get
      {
        if (this._dataSourceHelper == null)
        {
          this._dataSourceHelper = new SC.Sharedsource.Web.UI.DataSourceHelper(
            this.DataSource,
            SC.Context.Item);
        }

        return this._dataSourceHelper;
      }
    }

    protected override SC.Data.Items.Item GetItem()
    {
      return this.DataSourceHelper.GetSingleItem();
    }

    protected SC.Data.Items.Item[] GetItems()
    {
      return this.DataSourceHelper.Items;
    }

    protected IEnumerable<CS.SearchTypes.SitecoreUISearchResultItem> UISearchResults
    {
      get
      {
        this.DataSourceHelper.StoreUISearchResults = true;
        return this.DataSourceHelper.UISearchResults;
      }
    }

    protected string[] SearchResultIDs
    {
      get
      {
        this.DataSourceHelper.StoreSearchResultIDs = true;
        return this.DataSourceHelper.SearchResultIDs;
      }
    }
  }
}