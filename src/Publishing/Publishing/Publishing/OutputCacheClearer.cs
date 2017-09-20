namespace Sitecore.Sharedsource.Publishing
{
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;

  using global::Publishing.Pipelines.OutputCaches.ClearOutputCaches;

  public class OutputCacheClearer
  {
    public void ClearOutputCaches(
      OutputCacheClearingOptions outputCacheClearingOptions)
    {
      Assert.ArgumentNotNull(
        outputCacheClearingOptions, 
        "outputCacheClearingOptions");
      outputCacheClearingOptions.InfoDump();
      ClearOutputCachesArgs clearOutputCacheArgs =
        new ClearOutputCachesArgs(outputCacheClearingOptions);
      CorePipeline.Run("clearOutputCaches", clearOutputCacheArgs);
    }
  }
}