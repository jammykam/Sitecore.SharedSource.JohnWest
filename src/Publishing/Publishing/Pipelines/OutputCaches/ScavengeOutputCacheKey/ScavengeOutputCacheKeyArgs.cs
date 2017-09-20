namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  using Publishing.Pipelines.OutputCaches.ClearOutputCaches;

  using Sitecore.Pipelines;

  public class ScavengeOutputCacheKeyArgs : PipelineArgs
  {
    public ScavengeOutputCacheKeyArgs(
      ClearOutputCachesArgs args)
    {
      this.ClearOutputCachesArgs = args;
    }

    public OutputCacheSite OutputCacheSite { get; set; }

    public string CacheKey { get; set; }

    public ClearOutputCachesArgs ClearOutputCachesArgs { get; private set; }

    public bool Skip { get; set; }
  }
}