namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using SC = Sitecore;

  public class HandleStaticItems
    : SC.Sharedsource.Pipelines.GetPresentationDataSources.GetDataSourcesPipelineProcessor
  {
    public override void DoProcess(GetPresentationDataSourcesArgs args)
    {
      if (!(args.RawDataSource.StartsWith("/") 
        || args.RawDataSource.StartsWith("{")))
      {
        return;
      }

      foreach (string identifier in args.RawDataSource.Split('|'))
      {
        string[] parts = identifier.Split(':');
        SC.Diagnostics.Assert.IsTrue(parts.Length > 0, "parts > 0");
        SC.Diagnostics.Assert.IsTrue(parts.Length < 3,  "parts < 3");
        SC.Data.Items.Item item = args.ContextItem.Database.GetItem(parts[0]);

        if (item != null)
        {
          if (parts.Length > 1)
          {
            SC.Data.Fields.MultilistField field = item.Fields[parts[1]];

            if (field == null || string.IsNullOrEmpty(field.Value))
            {
              continue;
            }

            args.DataSourceItems.AddRange(field.GetItems());
            return;
          }

          args.DataSourceItems.Add(item);
        }
      }
    }
  }
}