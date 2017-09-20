namespace Sitecore.Sharedsource.Shell.Framework.Commands.System
{
  using Sitecore.Sharedsource.Publishing;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Globalization;

  public class Publish : Sitecore.Shell.Framework.Commands.System.Publish
  {
    public static new void Run(ClientPipelineArgs args)
    {
      if (!PublishingHelper.CanPublishDatabase(Sitecore.Context.ContentDatabase))
      {
        string msg = Translate.Text("You cannot publish this database.");
        Sitecore.Context.ClientPage.ClientResponse.Alert(msg);
        return;
      }

      Sitecore.Shell.Framework.Commands.System.Publish.Run(args);
    }

    public override CommandState QueryState(CommandContext context)
    {
      if (!PublishingHelper.CanPublishDatabase(Sitecore.Context.ContentDatabase))
      {
        return CommandState.Disabled;
      }

      return base.QueryState(context);
    }
  }
}
