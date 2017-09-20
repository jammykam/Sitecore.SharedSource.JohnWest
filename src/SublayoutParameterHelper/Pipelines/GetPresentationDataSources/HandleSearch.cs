namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using System.Linq;
  using Sitecore.ContentSearch.Linq;

  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class HandleSearch
    : SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(GetPresentationDataSourcesArgs args)
    {
      if (args.SearchContext == null)
      {
        args.SearchContext = CS.ContentSearchManager.CreateSearchContext(new CS.SitecoreIndexableItem(SC.Context.Item));

        try
        {
          this.InvokeQuery(args);
        }
        finally
        {
          if (args.SearchContext != null && !args.StoreUISearchResults)
          {
            args.SearchContext.Dispose();
            args.SearchContext = null;
          }
        }

        return;
      }

      this.InvokeQuery(args);
    }

    private void InvokeQuery(
      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args)
    {
      if (args.StoreUISearchResults)
      {
        args.UISearchResults =
          CS.Utilities.LinqHelper.CreateQuery(
            args.SearchContext, SC.Buckets.Util.UIFilterHelpers.ParseDatasourceString(args.RawDataSource))
            .Filter(language => language.Language == SC.Context.Language.CultureInfo.TwoLetterISOLanguageName);
      }

      if (args.StoreSearchResultIDs)
      {
        if (args.UISearchResults != null)
        {
          args.SearchResultIDs = args.UISearchResults.Select(item => item.Id).ToArray();
        }
        else
        {
          args.SearchResultIDs =
            CS.Utilities.LinqHelper.CreateQuery(
              args.SearchContext, SC.Buckets.Util.UIFilterHelpers.ParseDatasourceString(args.RawDataSource))
              .Filter(language => language.Language == SC.Context.Language.CultureInfo.TwoLetterISOLanguageName)
              .Select(item => item.Id)
              .ToArray();
        }
      }

      if (args.SearchResultIDs != null || args.UISearchResults != null)
      {
        return;
      }

      args.DataSourceItems.AddRange(
        CS.Utilities.LinqHelper.CreateQuery(
          args.SearchContext, SC.Buckets.Util.UIFilterHelpers.ParseDatasourceString(args.RawDataSource))
          .Filter(language => language.Language == SC.Context.Language.CultureInfo.TwoLetterISOLanguageName)
          .Select(item => item.GetItem()));
    }
  }
}