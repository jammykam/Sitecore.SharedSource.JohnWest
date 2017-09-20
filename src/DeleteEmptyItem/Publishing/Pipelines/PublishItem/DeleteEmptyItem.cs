namespace Sitecore.Sharedsource.Publishing.Pipelines.PublishItem
{
  using SC = Sitecore;

  public class DeleteEmptyItem : SC.Publishing.Pipelines.PublishItem.PublishItemProcessor
  {
    public override void Process(
      SC.Publishing.Pipelines.PublishItem.PublishItemContext context)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(context, "context");
      SC.Diagnostics.Assert.ArgumentNotNull(context.ItemId, "ItemId");
      SC.Diagnostics.Assert.ArgumentNotNull(
        context.PublishOptions, 
        "PublishOptions");
      SC.Diagnostics.Assert.ArgumentNotNull(
        context.PublishOptions.TargetDatabase, 
        "TargetDatabase");

      // retrieve the item from the publishing target database
      SC.Data.Items.Item item = context.PublishOptions.TargetDatabase.GetItem(
        context.ItemId);

      // if the item does not exist (or the context user does not have read access)
      if (item == null)
      {
        return;
      }

      foreach(SC.Globalization.Language language in item.Languages)
      {
        SC.Data.Items.Item version = item.Database.GetItem(
          item.ID, 
          language);

        if (version != null && version.Versions.Count > 0)
        {
          return;
        }
      }

      SC.Diagnostics.Log.Warn(
        this + " : delete " + item.Paths.FullPath + " from " + item.Database.Name, 
        this);
      item.Delete();
    }
  }
}