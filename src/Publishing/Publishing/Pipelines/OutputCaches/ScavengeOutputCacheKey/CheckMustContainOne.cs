namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  public class CheckMustContainOne : ScavengeOutputCacheKeyProcessor
  {
    protected override void DoProcess(ScavengeOutputCacheKeyArgs args)
    {
      if (args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustContainOne != null
        && args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustContainOne.Count > 0
        && !args.ClearOutputCachesArgs.OutputCacheClearingOptions.CacheKeysMustContainOne.Contains(
          args.CacheKey))
      {
        args.Skip = true;
      }
    }
  }
}