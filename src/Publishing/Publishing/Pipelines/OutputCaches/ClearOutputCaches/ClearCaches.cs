namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using System;

  using Sitecore;
  using Sitecore.Diagnostics;

  public class ClearCaches : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      //TODO: for republish or when flags indicate, clear caches regardless

      if ((args.OutputCacheClearingOptions.CacheKeysMustContainOne != null
          && args.OutputCacheClearingOptions.CacheKeysMustContainOne.Count > 0)
        || (args.OutputCacheClearingOptions.CacheKeysMustNotContainAny != null
          && args.OutputCacheClearingOptions.CacheKeysMustNotContainAny.Count > 0))
      {
        Log.Info(this + " : contains mismatch : return", this);
        return;
      }

      foreach (OutputCacheSite outputCacheSite in args.OutputCacheSites)
      {
        Log.Info(this + " : clear outut cache! for " + outputCacheSite.SiteContext.Name + " : " + Sitecore.DateUtil.IsoNow + " : " + DateUtil.ToIsoDate(DateTime.Now), this);
        outputCacheSite.HtmlCache.Clear();
        outputCacheSite.HtmlCache.SetHtml(
          args.LastClearedKey,
          DateUtil.ToIsoDate(DateTime.Now));
      }

      args.AbortPipeline();
    }
  }
}