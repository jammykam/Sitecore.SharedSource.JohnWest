namespace Sitecore.Sharedsource.Publishing
{
  using Sitecore.Sharedsource.Pipelines.OutputCaches;

  /// <summary>
  /// Store information collected by the TrackPublishing
  /// processor in the publishItem pipeline.
  /// </summary>
  public static class ClearSiteOutputCaches
  {
    public static OutputCacheClearingOptions
      OutputCacheClearingOptions { get; private set; }

    static ClearSiteOutputCaches()
    {
      ClearSiteOutputCaches.Reset();
    }

    public static void Reset()
    {
      ClearSiteOutputCaches.OutputCacheClearingOptions 
        = new OutputCacheClearingOptions();
    }
  }
}