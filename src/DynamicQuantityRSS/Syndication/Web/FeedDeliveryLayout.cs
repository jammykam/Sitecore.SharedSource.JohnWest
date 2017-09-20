namespace Sitecore.Sharedsource.Syndication.Web
{
  using System;

  public class FeedDeliveryLayout : System.Web.UI.Page
  {
    protected override void OnInit(EventArgs e)
    {
      Sitecore.Shell.Feeds.FeedUrlOptions options;
      Sitecore.Diagnostics.Assert.ArgumentNotNull(e, "e");
      Sitecore.Context.Site.SetDisplayMode(
        Sitecore.Sites.DisplayMode.Normal, 
        Sitecore.Sites.DisplayModeDuration.Temporary);
      Sitecore.Data.Items.Item item = Sitecore.Context.Item;
      Sitecore.Diagnostics.Assert.IsNotNull(item, "current item");
      base.Response.Clear();
      base.Response.ContentType = "text/xml";

      try
      {
        options = Sitecore.Shell.Feeds.FeedUrlOptions.ParseQueryString();
      }
      catch (Sitecore.Exceptions.SyndicationUrlHashMismatchException exception)
      {
        Sitecore.Diagnostics.Log.Error(
          "Sitecore RSS failed to authenticate user requesting the RSS feed", 
          exception, this);
        throw;
      }

      Sitecore.Security.Accounts.UserSwitcher switcher = null;

      if (options.UseUrlAuthentication)
      {
        switcher = new Sitecore.Security.Accounts.UserSwitcher(options.Username, true);
      }

      try
      {
        Sitecore.Sharedsource.Syndication.DynamicQuantityPublicFeed dFeed =
          Sitecore.Syndication.FeedManager.GetFeed(item) as Sitecore.Sharedsource.Syndication.DynamicQuantityPublicFeed;

        if (dFeed != null)
        {
          base.Response.Output.Write(Sitecore.Syndication.FeedManager.Render(dFeed.Render()));
          return;
        }

        Sitecore.Syndication.PublicFeed feed = Sitecore.Syndication.FeedManager.GetFeed(item);
        Sitecore.Diagnostics.Assert.IsNotNull(feed, "feed");
        base.Response.Output.Write(Sitecore.Syndication.FeedManager.Render(feed.Render()));
      }
      finally
      {
        if (switcher != null)
        {
          switcher.Dispose();
        }
      }
      base.Response.End();
    }
  }
}