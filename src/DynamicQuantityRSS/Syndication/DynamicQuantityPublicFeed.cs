namespace Sitecore.Sharedsource.Syndication
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.ServiceModel.Syndication;

  public class DynamicQuantityPublicFeed : Sitecore.Syndication.PublicFeed
  {
    public new IEnumerable<Sitecore.Data.Items.Item> GetItems()
    {
      IEnumerable<Sitecore.Data.Items.Item> sourceItems = this.GetSourceItems();
      List<DatedItem> list = new List<DatedItem>();

      foreach (Sitecore.Data.Items.Item item in sourceItems)
      {
        DateTime? itemDate = this.GetItemDate(item);

        if (itemDate.HasValue && (!Sitecore.Configuration.Settings.Feeds.ItemExpiration.HasValue || ((DateTime.Now - itemDate.Value) <= Sitecore.Configuration.Settings.Feeds.ItemExpiration.Value)))
        {
          list.Add(new DatedItem
          {
            Date = itemDate.Value,
            Item = item
          });
        }
      }

      int max = Sitecore.Configuration.Settings.Feeds.MaximumItemsInFeed;

      if (this.FeedItem != null && !String.IsNullOrEmpty(this.FeedItem["MaxEntries"]))
      {
        max = Int32.Parse(this.FeedItem["MaxEntries"]);
      }

      return (from i in
                (from item in list
                 orderby (TimeSpan)(DateTime.MaxValue - item.Date)
                 select item).Take<DatedItem>(max)
              select i.Item);
    }

    private bool IsCacheable()
    {
      return MainUtil.GetBool(this.FeedItem["Cacheable"], false);
    }

    private Sitecore.Caching.Cache.CacheEntry GetCacheEntry()
    {
      return Sitecore.Syndication.FeedManager.Cache.GetEntry(this.FeedItem.Uri.ToString(), true);
    }

    private TimeSpan GetCacheDuration()
    {
      string str = this.FeedItem["Cache Duration"];
      if (!string.IsNullOrEmpty(str))
      {
        return TimeSpan.Parse(str);
      }
      return TimeSpan.FromDays(1.0);
    }

    protected override DateTime? GetItemDate(Sitecore.Data.Items.Item item)
    {
      System.Web.UI.Control feedRendering = Sitecore.Syndication.FeedUtil.GetFeedRendering(item);
      if (feedRendering == null)
      {
        return null;
      }
      using (new Sitecore.Data.Items.ContextItemSwitcher(item))
      {
        if (feedRendering is Sitecore.Web.UI.WebControls.FeedRenderer)
        {
          Sitecore.Web.UI.WebControls.FeedRenderer renderer = 
            feedRendering as Sitecore.Web.UI.WebControls.FeedRenderer;
          renderer.Database = (Context.ContentDatabase ?? Context.Database).Name;
          return new DateTime?(renderer.GetDate());
        }
      }

      throw new InvalidOperationException(
        "FeedRenderer rendering must be of Sitecore.Web.UI.WebControls.FeedRenderer type");
    }

    public new SyndicationFeed Render()
    {
      if (this.IsCacheable())
      {
        Sitecore.Caching.Cache.CacheEntry cacheEntry = this.GetCacheEntry();

        if (cacheEntry != null)
        {
          return Sitecore.Diagnostics.Assert.ResultNotNull<SyndicationFeed>(
            cacheEntry.Data as SyndicationFeed, "cached feed");
        }
      }

      SyndicationFeed feed = new SyndicationFeed();
      this.SetupFeed(feed);
      IEnumerable<Sitecore.Data.Items.Item> items = this.GetItems();
      List<SyndicationItem> list = new List<SyndicationItem>();

      foreach (Sitecore.Data.Items.Item item in items)
      {
        SyndicationItem item2 = this.RenderItem(item);

        if (item2 != null)
        {
          list.Add(item2);
        }
      }

      feed.Items = list;
      Sitecore.Syndication.FeedManager.Cache.Add(
        this.FeedItem.Uri.ToString(), feed, (long)(feed.Items.Count<SyndicationItem>() * 0xfd0), 
        DateTime.Now + this.GetCacheDuration());
      return feed;
    }
  }
} 