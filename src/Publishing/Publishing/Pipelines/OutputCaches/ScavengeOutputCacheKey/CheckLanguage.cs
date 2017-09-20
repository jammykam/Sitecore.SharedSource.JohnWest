namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  public class CheckLanguage : ScavengeOutputCacheKeyProcessor
  {
    protected override void DoProcess(ScavengeOutputCacheKeyArgs args)
    {
      if (args.ClearOutputCachesArgs.OutputCacheClearingOptions.IgnorePublishingLanguage)
      {
        return;
      }

      if (!args.CacheKey.Contains(
        "_#lang:" + args.ClearOutputCachesArgs.OutputCacheClearingOptions.PublishingLanguage.ToUpperInvariant()))
      {
        args.Skip = true;
      }
    }
  }
}