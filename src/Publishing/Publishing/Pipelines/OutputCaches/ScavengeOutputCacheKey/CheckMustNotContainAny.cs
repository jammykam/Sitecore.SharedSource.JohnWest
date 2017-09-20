namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  //TODO: consistent naming globally - CheckMustNotContainAny/
  public class CheckMustNotContainAny : ScavengeOutputCacheKeyProcessor
  {
    protected override void DoProcess(ScavengeOutputCacheKeyArgs args)
    {
      if (args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustNotContainAny != null
        && args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustNotContainAny.Count > 0
        && args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustNotContainAny.Contains(
          args.CacheKey))
      {
        args.Skip = true;
      }
    }
  }
}