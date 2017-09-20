namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using SC = Sitecore;

  public abstract class GetDataSourcesPipelineProcessor
  {
    public abstract void DoProcess(
      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args);

    public virtual void Process(
      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args)
    {
      if (args.UISearchResults != null 
        || args.SearchResultIDs != null 
        || args.DataSourceItems != null)
      {
        return;
      }

      this.DoProcess(args);
    }
  }
}