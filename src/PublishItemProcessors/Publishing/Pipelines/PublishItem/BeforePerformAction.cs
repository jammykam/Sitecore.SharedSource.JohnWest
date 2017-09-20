namespace Sitecore.Sharedsource.Publishing.Pipelines.PublishItem
{
  public class BeforePerformAction : 
    Sitecore.Publishing.Pipelines.PublishItem.PublishItemProcessor
  {
    protected void AddMessage(
      Sitecore.Publishing.Pipelines.PublishItem.PublishItemContext context,
      string message)
    {
      context.AddMessage(this + " : " + message);
    }

    protected string GetMessage(
      Sitecore.Publishing.Pipelines.PublishItem.PublishItemContext context)
    {
      string toPublish = "deleted item "
                         + context.ItemId.ToString()
                         + "; current language is "
                         + context.PublishOptions.Language.Name;

      if (context.VersionToPublish != null)
      {
        toPublish = "version "
                    + context.VersionToPublish.Version.Number
                    + " of "
                    + context.VersionToPublish.Paths.FullPath;
      }

      return "About to publish "
             + toPublish
             + " from "
             + context.PublishOptions.SourceDatabase
             + " to "
             + context.PublishOptions.TargetDatabase;
    }

    public override void Process(
      Sitecore.Publishing.Pipelines.PublishItem.PublishItemContext context)
    {
      this.AddMessage(context, this.GetMessage(context));
      Sitecore.Data.Items.Item target = context.PublishOptions.TargetDatabase.GetItem(
        context.ItemId, 
        context.PublishOptions.Language);

      if (target != null)
      {
        string msg = "Item exists in target database before publication and has "
              + target.Versions.Count
              + " versions there in this language (old path is "
              + target.Paths.FullPath
              + ").";
        this.AddMessage(context, msg);
      }
      else
      {
        this.AddMessage(
          context, 
          "Item does not exist in target database before publication.");
      }

      this.AddMessage(
        context, 
        "Result before publishing : " + context.Result);
      this.AddMessage(
        context, 
        "Action before publishing : " + context.Action);
      context.CustomData["whatever"] = new Sitecore.Links.UrlOptions();
    }
  }
}