namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using Assert = Sitecore.Diagnostics.Assert;
  using SC = Sitecore;

  public class HandleEmptyDataSource
    : SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(
      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!string.IsNullOrEmpty(args.RawDataSource))
      {
        return;
      }

      args.DataSourceItems.Add(SC.Context.Item);
    }
  }
}