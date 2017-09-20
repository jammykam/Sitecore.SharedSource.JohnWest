namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using Sitecore.Caching;
  using Sitecore.Diagnostics;
  using Sitecore.Sites;
  using Sitecore.Web;

  public class GetOutputCacheSites : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      foreach (SiteInfo siteInfo 
        in Sitecore.Configuration.Factory.GetSiteInfoList())
      {
//        Log.Info(this + " : checking siteInfo for " + siteInfo.Name, this);

        if (!siteInfo.CacheHtml)
        {
//          Log.Info(this + " : CacheHtml false; continue : " + siteInfo.Name, this);
          continue;
        }

        SiteContext siteContext = 
          Sitecore.Configuration.Factory.GetSite(siteInfo.Name);
        Assert.IsNotNull(siteContext, "siteContext: " + siteInfo.Name);
        HtmlCache htmlCache = CacheManager.GetHtmlCache(siteContext);

        if (htmlCache == null 
          || htmlCache.InnerCache.Count < 1
          || (htmlCache.InnerCache.Count < 2
            && htmlCache.InnerCache.ContainsKey(args.LastClearedKey)))
        {
          Log.Info(this + " : no html cache, cache count less than one, or only contains last cleared key : " + siteInfo.Name + " : " + htmlCache, this);
          continue;
        }

        Log.Info(this + " : AddOutputCacheSite! : " + siteInfo.Name, this);
        args.AddOutputCacheSite(new OutputCacheSite(
          siteContext, 
          htmlCache, 
          args.LastClearedKey));
      }
    }
  }
}