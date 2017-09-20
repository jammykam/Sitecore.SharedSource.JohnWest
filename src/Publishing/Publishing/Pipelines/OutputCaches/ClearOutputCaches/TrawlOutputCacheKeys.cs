namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using Sitecore.Pipelines;

  public class TrawlOutputCacheKeys : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      ScavengeOutputCacheKey.ScavengeOutputCacheKeyArgs innerArgs =
        new ScavengeOutputCacheKey.ScavengeOutputCacheKeyArgs(args);

      foreach (OutputCacheSite outputCacheSite in args.OutputCacheSites)
      {
        innerArgs.OutputCacheSite = outputCacheSite;

        foreach (string key in outputCacheSite.HtmlCache.InnerCache.GetCacheKeys())
        {
          innerArgs.CacheKey = key;
          CorePipeline.Run("scavengeOutputCacheKey", innerArgs);
        }
      }
    }
  }
}