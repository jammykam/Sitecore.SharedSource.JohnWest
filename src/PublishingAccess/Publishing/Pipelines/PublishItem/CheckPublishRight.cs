namespace Sitecore.Sharedsource.Publishing.Pipelines.PublishItem
{
  using System;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Publishing;
  using Sitecore.Publishing.Pipelines.PublishItem;
  using Sitecore.Globalization;

  public class CheckPublishRight : PublishItemProcessor
  {
    public override void Process(PublishItemContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      Item item = context.PublishHelper.GetSourceItem(context.ItemId);

      if (item != null
        && !PublishingHelper.CanPublishItem(item, Sitecore.Context.User))
      {
        string msg = String.Format(
            "{0} does not have permission to publish {1}",
            Sitecore.Context.User.Name,
            item.Paths.FullPath);
        context.AbortPipeline(
          PublishOperation.Skipped,
          PublishChildAction.Allow,
          Translate.Text(msg));
      }
    }
  }
}
