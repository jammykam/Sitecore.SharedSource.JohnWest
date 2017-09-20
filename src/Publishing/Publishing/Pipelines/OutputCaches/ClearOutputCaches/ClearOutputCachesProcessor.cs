namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using Sitecore.Diagnostics;

  public abstract class ClearOutputCachesProcessor
  {
    public void Process(ClearOutputCachesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(
        args.OutputCacheSites, 
        "args.OutputCacheSites");
      Log.Info(this + " : DoProcess()", this);
      this.DoProcess(args);

      if (args.OutputCacheSites.Count < 1)
      {
        Log.Info(this + " args.OutputCacheSites.Count < 1 : AbortPipeline()", this);
        args.AbortPipeline();
      }
    }

    protected abstract void DoProcess(ClearOutputCachesArgs args);
  }
}