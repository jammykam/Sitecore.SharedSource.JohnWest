namespace Sitecore.Sharedsource.Publishing.Pipelines.PublishItem
{
  public class AfterPerformAction : 
    Sitecore.Publishing.Pipelines.PublishItem.PublishItemProcessor
  {
    public override void Process(
      Sitecore.Publishing.Pipelines.PublishItem.PublishItemContext context)
    {
      string name = typeof(
        Sitecore.Sharedsource.Publishing.Pipelines.PublishItem.BeforePerformAction).ToString();

      foreach (Sitecore.Pipelines.PipelineMessage message 
        in context.GetMessages())
      {
        if (message.Text.StartsWith(name))
        {
          Sitecore.Diagnostics.Log.Info(
            this + " : " + message.Text, this);
        }
      }

      Sitecore.Diagnostics.Log.Info(
        this + " : Result after publishing : " + context.Result.Operation, 
        this);
      Sitecore.Links.UrlOptions opt = 
        context.CustomData["whatever"] as Sitecore.Links.UrlOptions;

      if (opt != null)
      {
        Sitecore.Diagnostics.Log.Info(
          this + " : " + opt + " came through the pipeline.", 
          this);
      }

      if (context.VersionToPublish == null)
      {
        return;
      }

      Sitecore.Data.Fields.DateField lastPublished = 
        context.VersionToPublish.Fields["LastPublished"];

      if (lastPublished == null)
      {
        return;
      }

      using (new Sitecore.Data.Items.EditContext(
        context.VersionToPublish, false /*updateStatistics*/, false /*silent*/))
      {
        lastPublished.Value = Sitecore.DateUtil.IsoNow;
      }
    }
  }
}