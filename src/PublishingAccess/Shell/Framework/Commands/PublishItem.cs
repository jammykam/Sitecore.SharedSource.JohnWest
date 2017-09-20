namespace Sitecore.Sharedsource.Shell.Framework.Commands
{
  using Sitecore.Sharedsource.Publishing;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;

  public class PublishItem : Sitecore.Shell.Framework.Commands.PublishItem
  {
    public override CommandState QueryState(CommandContext context)
    {
      Assert.IsNotNull(context, "context");

      if (PublishingHelper.CanPublishDatabase(Sitecore.Context.ContentDatabase)
        && context.Items.Length == 1 
        && context.Items[0] != null
        && (PublishingHelper.CanPublishItem(context.Items[0], Sitecore.Context.User) || context.Items[0].HasChildren))
      {
        return base.QueryState(context);
      }

      return CommandState.Disabled;
    }

    protected new void Run(ClientPipelineArgs args)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args.Parameters["id"], "id");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args.Parameters["language"], "language");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args.Parameters["version"], "version");

      if (!SheerResponse.CheckModified())
      {
        return;
      }

      Item item = Context.ContentDatabase.GetItem(
        args.Parameters["id"], 
        Language.Parse(args.Parameters["language"]), 
        Version.Parse(args.Parameters["version"]));

      if (item == null 
        || PublishingHelper.CanPublishItem(item, Sitecore.Context.User))
      {
        base.Run(args);
        return;
      }

      if (args.IsPostBack)
      {
        if (args.Result == "yes")
        {
          base.Run(args);
        }

        return;
      }
      else
      {
        string msg = "You cannot publish this item.\n\nDo you want to publish any descendants that you can publish?";
        SheerResponse.Confirm(Translate.Text(msg));
        args.WaitForPostBack();
        return;
      }
    }
  }
}