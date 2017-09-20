namespace Sitecore.Sharedsource.Shell.Framework.Commands
{
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Sharedsource.Publishing;
  using Sitecore.Globalization;

  public class PublishNow : Sitecore.Shell.Framework.Commands.PublishNow
  {
    public override void Execute(CommandContext context)
    {
      if (context.Items.Length != 1
        || context.Items[0] == null
        || !PublishingHelper.CanPublishItem(context.Items[0], Sitecore.Context.User))
      {
        string msg = Translate.Text("You cannot publish this item.");
        Sitecore.Context.ClientPage.ClientResponse.Alert(msg);
        return;
      }

      base.Execute(context);
    }

    public override CommandState QueryState(CommandContext context)
    {
      if (context.Items.Length == 1
        && context.Items[0] != null
        && !PublishingHelper.CanPublishItem(context.Items[0], Sitecore.Context.User))
      {
        return CommandState.Disabled;
      }

      return base.QueryState(context);
    }
  }
}
