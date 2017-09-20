namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using System.Collections.Generic;

  using Assert = Sitecore.Diagnostics.Assert;
  using SC = Sitecore;

  public class HandleEmptyDataSource : 
    SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(
      SC.Sharedsource.Pipelines.GetPresentationDataSources.GetPresentationDataSourcesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!string.IsNullOrEmpty(args.RawDataSource))
      {
        return;
      }

      if (args.DataSourceItems == null)
      {
        args.DataSourceItems = new List<SC.Data.Items.Item>();
      }

      args.DataSourceItems.Add(SC.Context.Item);
    }
  }
}