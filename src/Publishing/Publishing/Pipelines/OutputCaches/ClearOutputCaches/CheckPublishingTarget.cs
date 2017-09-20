namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using Sitecore.Diagnostics;

  public class CheckPublishingTarget : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      if (args.OutputCacheClearingOptions.IgnorePublishingTargets
        || args.OutputCacheClearingOptions.PublishingTargets == null
        || args.OutputCacheClearingOptions.PublishingTargets.Count < 1)
      {
        Log.Info(this + " : IgnorePublishingTargets or PublishingTarget null or empty : return : " + args.OutputCacheClearingOptions.IgnorePublishingTargets + " : " + args.OutputCacheClearingOptions.PublishingTargets, this);
        return;
      }

      foreach (OutputCacheSite outputCacheSite
        in args.OutputCacheSites.ToArray())
      {
        bool isRelevant = false;

        foreach (string target in args.OutputCacheClearingOptions.PublishingTargets)
        {
          if (outputCacheSite.IsPublishingTargetRelevant(target))
          {
            isRelevant = true;
            break;
          }
        }

        if (!isRelevant)
        {
          args.OutputCacheSites.Remove(outputCacheSite);
        }
      }
    }
  }
}