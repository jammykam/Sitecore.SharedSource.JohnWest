namespace Sitecore.Sharedsource.Syndication
{
  using System.Web.UI;
  using System.ServiceModel.Syndication;

  public class NewsFeed : Sitecore.Syndication.PublicFeed
  {
    protected new virtual SyndicationItem RenderItem(Sitecore.Data.Items.Item item)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(item, "item");
      Control control = Sitecore.Syndication.FeedUtil.GetFeedRendering(item);

      if (control == null)
      {
        return null;
      }

      using (new Sitecore.Data.Items.ContextItemSwitcher(item))
      {
        Sitecore.Sharedsource.Web.UI.WebControls.NewsFeedRenderer newsRenderer =
          control as Sitecore.Sharedsource.Web.UI.WebControls.NewsFeedRenderer;
        
        if (newsRenderer != null)
        {
          newsRenderer.Database = (Sitecore.Context.ContentDatabase ?? Context.Database).Name;
          return newsRenderer.RenderItem();
        }

        Sitecore.Web.UI.WebControls.FeedRenderer renderer = 
          control as Sitecore.Web.UI.WebControls.FeedRenderer;
        Sitecore.Diagnostics.Assert.IsNotNull(renderer, "renderer");
        renderer.Database = (Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database).Name;
        return renderer.RenderItem();
      }
    }
  }
}