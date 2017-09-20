namespace Sitecore.Sharedsource.Publishing.PublishItem
{
  using System.Collections.Generic;

  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Publishing;
  using Sitecore.Publishing.Pipelines.PublishItem;
  using Sitecore.Web;

  /// <summary>
  /// Store information about published items in
  /// ClearSiteOutputCaches.OutputCacheClearingOptions.ClearAllOutputCachesCompletely.
  /// </summary>
  public class TrackPublishing : PublishItemProcessor
  {
    public override void Process(PublishItemContext publishItemContext)
    {
      Assert.ArgumentNotNull(publishItemContext, "publishItemContext");
      Assert.ArgumentNotNull(
        publishItemContext.Result, 
        "publishItemContext.Result");

      // if publishing did not touch this item in the target database
      // then there is no need to track this item. If caches need 
      // to be cleared completely, there is no need to track any items.
      if (publishItemContext.Result.Operation == PublishOperation.Skipped
        || publishItemContext.Action == PublishAction.None 
        || ClearSiteOutputCaches.OutputCacheClearingOptions.ClearAllOutputCachesCompletely)
      {
        return;
      }

      // output caches may need to be cleared or scavenged
      ClearSiteOutputCaches.OutputCacheClearingOptions.PublishedSomething
        = true;

      // the item that was published
      Item item = publishItemContext.PublishOptions.SourceDatabase.GetItem(
        publishItemContext.ItemId);

      // the item must have been deleted so we can't get information about it; 
      // all caches need to be completely cleared.
      if (item == null)
      {
        Log.Info(this + " : " + publishItemContext.ItemId + " does not exist : set ClearAllOutputCachesCompletely and return", this);
        ClearSiteOutputCaches.OutputCacheClearingOptions.ClearAllOutputCachesCompletely
          = true;
        return;
      }

      // the sites for which the item is relevant.
      string[] siteNames = this.GetSiteNamesForItem(item);

      // if no sites are relevant, assume all sites are relevant.
      if (siteNames == null || siteNames.Length < 1)
      {
        Log.Info(this + " : no sites for " + item.Paths.FullPath + " : set ClearAllOutputCaches and return", this);
        ClearSiteOutputCaches.OutputCacheClearingOptions.ClearAllOutputCachesCompletely
          = true;
        return;
      }

      // ClearCacheForSite() will prevent the same entry from appearing twice
      foreach (string siteName in siteNames)
      {
        Log.Info(this + " : site for " + item.Paths.FullPath + " : " + siteName, this);
        ClearSiteOutputCaches.OutputCacheClearingOptions.ClearCacheForSite(
          siteName);
      }
    }
    
    private string[] GetSiteNamesForItem(Item item)
    {
      Assert.ArgumentNotNull(item, "item");
      List<string> sites = new List<string>();

      foreach (string site in this.GetSiteNamesForItemRecursive(item))
      {
        sites.Add(site);
      }

      // add site definitions for which the start item matches the item path
      foreach (SiteInfo siteInfo in Configuration.Factory.GetSiteInfoList())
      {
        if ((!ClearSiteOutputCaches.OutputCacheClearingOptions.IgnoreSites.Contains(siteInfo.Name.ToLowerInvariant()))
            && item.Paths.FullPath.ToLower().StartsWith((siteInfo.RootPath + siteInfo.StartItem).ToLower()))
        {
          if (!sites.Contains(siteInfo.Name.ToLowerInvariant()))
          {
            sites.Add(siteInfo.Name.ToLowerInvariant());
          }
        }
      }

      return sites.ToArray();
    }

    private IEnumerable<string> GetSiteNamesForItemRecursive(Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      // add sites specified in a pipe-separated list in a field in the item and its ancestors
      string siteNames = item["Sites"];

      if (!string.IsNullOrEmpty(siteNames))
      {
        foreach (string siteName in siteNames.Split('|'))
        {
          yield return siteName.ToLowerInvariant();
        }
      }

      // look for sites that match the item
      // and its ancestors
      if (item.Parent != null)
      {
        foreach (string siteName in this.GetSiteNamesForItem(item.Parent))
        {
          yield return siteName;
        }
      }
    }
  }
}