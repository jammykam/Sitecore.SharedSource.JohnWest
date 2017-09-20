namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using Sitecore.Diagnostics;

  public class CheckPublishingLanguage : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      if (args.OutputCacheClearingOptions.IgnorePublishingLanguage
        || string.IsNullOrEmpty(args.OutputCacheClearingOptions.PublishingLanguage))
      {
        Log.Info(this + " args.IgnorePublishingLanguage : " + args.OutputCacheClearingOptions.IgnorePublishingLanguage + " : return", this); 
        return;
      }

      foreach (OutputCacheSite outputCacheSite
        in args.OutputCacheSites.ToArray())
      {
        if (!outputCacheSite.IsLanguageRelevant(
          args.OutputCacheClearingOptions.PublishingLanguage))
        {
          Log.Info(this + " : " + outputCacheSite.SiteContext.Name + " not relevant to " + args.OutputCacheClearingOptions.PublishingLanguage + " : return", this); 
          args.RemoveOutputCacheSite(outputCacheSite);
        }
      }
    }
  }
}