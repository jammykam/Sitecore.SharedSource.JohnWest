namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  public class ClearEntry : ScavengeOutputCacheKeyProcessor
  {
    protected override void DoProcess(ScavengeOutputCacheKeyArgs args)
    {
      if (!args.Skip)
      {
        args.OutputCacheSite.HtmlCache.InnerCache.Remove(args.CacheKey);
      }
    }
  }
}
