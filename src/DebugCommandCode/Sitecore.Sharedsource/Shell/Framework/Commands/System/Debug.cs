namespace Sitecore.Sharedsource.Shell.Framework.Commands.System
{
  using global::System;
  using global::System.Linq;

  using SC = Sitecore;

  [Serializable]
  public class Debug : SC.Shell.Framework.Commands.Command
  {
    public override void Execute(SC.Shell.Framework.Commands.CommandContext context)
    {
      SC.Diagnostics.Assert.IsNotNull(context, "context");
      SC.Text.UrlString webSiteUrl = new SC.Text.UrlString("/");
      webSiteUrl.Add("sc_debug", "1");
      webSiteUrl.Add("sc_prof", "1");
      webSiteUrl.Add("sc_trace", "1");
      webSiteUrl.Add("sc_ri", "1");
      SC.Data.Database db = SC.Context.ContentDatabase;
      SC.Globalization.Language lang = SC.Context.ContentLanguage;

      // if the context for invoking the command specifies exactly one item
      if (context.Items != null
        && context.Items.Length == 1
        && context.Items[0] != null)
      {
        // ensure the user has saved the selected item
        SC.Context.ClientPage.ClientResponse.CheckModified(false);
        SC.Data.Items.Item item = context.Items[0];
        db = item.Database;
        lang = item.Language;
        webSiteUrl.Add("sc_itemid", item.ID.ToString());
      }

      if (db != null)
      {
        webSiteUrl.Add("sc_database", db.Name);
      }

      if (lang != null)
      {
        webSiteUrl.Add("sc_lang", lang.ToString());
      }

      // without this, I didn't see the ribbon
      SC.Publishing.PreviewManager.RestoreUser();
      SC.Context.ClientPage.ClientResponse.Eval(
        "window.open('" + webSiteUrl + "', '_blank')");
    }

    public override SC.Shell.Framework.Commands.CommandState QueryState(
      SC.Shell.Framework.Commands.CommandContext context)
    {
      SC.Diagnostics.Assert.IsNotNull(context, "context");

      // if the context does not indicate exactly one item
      // then this must be the Debug command on the Sitecore menu
      // in which case permissions alone control access
      if (context.Items == null 
        || context.Items.Length != 1
        || context.Items[0] == null)
      {
        return base.QueryState(context);
      }

      SC.Data.Items.Item item = context.Items[0];

      if (item.Paths.IsContentItem
        && (item.Database.Resources.Devices.GetAll().Where(compare => compare.ID != SC.Syndication.FeedUtil.FeedDeviceId
        || !SC.Syndication.FeedUtil.IsFeed(item)).Any(compare => item.Visualization.GetLayout(compare) != null)))
      {
        // the selected item has a URL
        return SC.Shell.Framework.Commands.CommandState.Enabled;
      }

      // the selected item does not have a URL
      return SC.Shell.Framework.Commands.CommandState.Disabled;
    }
  }
}