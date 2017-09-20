namespace Sitecore.Sharedsource.Eventing.Remote
{
  using Sitecore.Data.Events;
  using Sitecore.Diagnostics;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;

  public class CustomPublishEndRemoteEventArgs : PublishEndRemoteEventArgs
  {
    public CustomPublishEndRemoteEventArgs(
      CustomPublishEndRemoteEvent customPublishEndRemoteEvent)
      : base(customPublishEndRemoteEvent)
    {
      Log.Debug(this + " : constructor");
      this.OutputCacheClearingOptions = 
        customPublishEndRemoteEvent.OutputCacheClearingOptions;
    }

    public OutputCacheClearingOptions OutputCacheClearingOptions { get; private set; }
  }
}