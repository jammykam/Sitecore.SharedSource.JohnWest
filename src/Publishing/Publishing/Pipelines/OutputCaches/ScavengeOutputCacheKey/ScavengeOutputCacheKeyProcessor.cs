namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  using Sitecore.Diagnostics;

  public abstract class ScavengeOutputCacheKeyProcessor
  {
    public void Process(ScavengeOutputCacheKeyArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(
        args.ClearOutputCachesArgs,
        "args.ClearOutputCachesArgs");

      if (args.Skip)
      {
        return;
      }

      this.DoProcess(args);
    }

    protected abstract void DoProcess(ScavengeOutputCacheKeyArgs args);
  }
}