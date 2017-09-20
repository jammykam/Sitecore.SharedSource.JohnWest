namespace Sitecore.Sharedsource.Web.UI
{
  using System;
  using System.Linq;

  using Assert = Sitecore.Diagnostics.Assert;
  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class DataSourceHelper : IDisposable
  {
    private SC.Data.Items.Item[] _items;

    private bool? _disposeOfSearchContext;

    private IQueryable<CS.SearchTypes.SitecoreUISearchResultItem> _uiSearchResults;

    private string[] _searchResultIDs;

    private CS.IProviderSearchContext _searchContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceHelper"/> class. 
    /// </summary>
    /// <param name="dataSource">The string data source passed to the presentation component.
    /// </param>
    /// <param name="contextItem">The context item for relative paths and specifying the
    /// database from which to retrieve other items.
    /// </param>
    /// <param name="searchContext">The search context.
    /// </param>
    public DataSourceHelper(
      string dataSource,
      SC.Data.Items.Item contextItem,
      CS.IProviderSearchContext searchContext = null)
    {
      this.RawDataSource = dataSource;
      this.ContextItem = contextItem;
      this.SearchContext = searchContext;
    }

    /// <summary>
    /// Gets or sets a value indicating 
    /// whether to store the IDs of search results.
    /// Set to true for XSL and anything else that 
    /// does not use UISearchResultItems 
    /// or DataSourceItems.
    /// </summary>
    public bool StoreSearchResultIDs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to store the
    /// Sitecore.ContentSearch.SearchTypes.SitecoreUISearchResultItem
    /// objects determined by a search query.
    /// </summary>
    public bool StoreUISearchResults { get; set; }

    /// <summary>
    /// Gets a value indicating 
    /// the string passed as a data source to the presentation component.
    /// </summary>
    public string RawDataSource { get; private set; }

    /// <summary>
    /// Gets a value indicating the context item 
    /// from which to determine relative paths
    /// and the database containing other items.
    /// </summary>
    public SC.Data.Items.Item ContextItem { get; private set; }

    /// <summary>
    /// Gets the IDs of the items matched by a search query.
    /// </summary>
    public string[] SearchResultIDs
    {
      get
      {
        if (this._searchResultIDs == null)
        {
          this.StoreSearchResultIDs = true;
          this.ManagePipeline(true);
        }

        return this._searchResultIDs;
      }
    }

    /// <summary>
    /// Gets the 
    /// Sitecore.ContentSearch.SearchTypes.SitecoreUISearchResultItem
    /// objects matched by a search query.
    /// </summary>
    public IQueryable<CS.SearchTypes.SitecoreUISearchResultItem> UISearchResults
    {
      get
      {
        if (this._uiSearchResults == null)
        {
          this.StoreUISearchResults = true;
          this.ManagePipeline(true);
        }

        return this._uiSearchResults;
      }
    }

    /// <summary>
    /// Gets or sets  a value indicating the search context.
    /// </summary>
    public CS.IProviderSearchContext SearchContext
    {
      get
      {
        return this._searchContext;
      }

      set
      {
        this._searchContext = value;
        this._disposeOfSearchContext = false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether 
    /// this object previously invoked the getPresentationDataSources pipeline.
    /// </summary>
    public bool PipelineInvoked { get; private set; }

    /// <summary>
    /// Gets the items matched by the search query.
    /// </summary>
    public SC.Data.Items.Item[] Items
    {
      get
      {
        if (this._items == null)
        {
          this.ManagePipeline();

          if (this._items == null)
          {
            if (this.SearchResultIDs != null)
            {
              Assert.ArgumentNotNull(this.ContextItem, "ContextItem");
              this._items = this.SearchResultIDs.Select(id => this.ContextItem.Database.GetItem(id)).Where(item => item != null).ToArray();
            }
            else if (this.UISearchResults != null)
            {
              this._items = this.UISearchResults.Select(item => item.GetItem()).ToArray();
            }
          }
        }

        return this._items;
      }
    }

    /// <summary>
    /// Gets the first item from the list of items matched by the query.
    /// </summary>
    /// <param name="assertMaxOne">Throw an exception if there is more than one
    /// item in the list.</param>
    /// <param name="assertMinOne">Throw an exception if there is less than one
    /// item in the list.</param>
    /// <returns>The first item in the list of items matched by the query.</returns>
    public SC.Data.Items.Item GetSingleItem(bool assertMaxOne, bool assertMinOne)
    {
      if (assertMaxOne)
      {
        Assert.IsFalse(this.Items.Length > 1, "this.Items.Length > 1");
      }

      if (assertMinOne)
      {
        Assert.IsFalse(this.Items.Length < 1, "this.Items.Length < 1");
      }

      return this.Items[0];
    }

    /// <summary>
    /// Gets the first item from the list of items matched by the query.
    /// Throws an exception if there is more or less than one item in the list.</summary>
    /// <returns>The first item in the list of items matched by the query.</returns>
    public SC.Data.Items.Item GetSingleItem()
    {
      return this.GetSingleItem(true /*assertMaxOne*/, true /*assertMinOne*/);
    }

    /// <summary>
    /// Disposes of the search context if appropriate.
    /// </summary>
    public void Dispose()
    {
      if (this._searchContext != null
        && ((!this._disposeOfSearchContext.HasValue) || this._disposeOfSearchContext.Value))
      {
        this._searchContext.Dispose();
        this._searchContext = null;
      }
    }

    /// <summary>
    /// Invoke the pipeline if not previously invoked.
    /// </summary>
    private void ManagePipeline()
    {
      this.ManagePipeline(false /*force*/);
    }

    /// <summary>
    /// Invoke the pipeline if not previously invoked
    /// or if forced.
    /// </summary>
    /// <param name="force">Force invocation
    /// even if previously invoked.</param>
    private void ManagePipeline(bool force)
    {
      if (this.PipelineInvoked && !force)
      {
        return;
      }

      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args =
      new SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs(
        this.RawDataSource, this.ContextItem, this.StoreUISearchResults, this.StoreSearchResultIDs, this.SearchContext);
      SC.Pipelines.CorePipeline.Run("getPresentationDataSources", args);
      this.PipelineInvoked = true;

      if (this.StoreSearchResultIDs)
      {
        this._searchResultIDs = args.SearchResultIDs;
      }

      if (this.StoreUISearchResults)
      {
        this._uiSearchResults = args.UISearchResults;
      }

      if (args.DataSourceItems != null && args.DataSourceItems.Count > 0)
      {
        this._items = args.DataSourceItems.ToArray();
      }
    }
  }
}