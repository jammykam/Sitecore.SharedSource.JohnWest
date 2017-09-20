namespace Sitecore.Sharedsource.Eventing.Remote
{
  using System.Runtime.Serialization;

  using Sitecore.Diagnostics;
  using Sitecore.Eventing.Remote;
  using Sitecore.Events;
  using Sitecore.Publishing;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;

  [DataContract]
  public class CustomPublishEndRemoteEvent : PublishEndRemoteEvent
  {
    [DataMember]
    public OutputCacheClearingOptions OutputCacheClearingOptions { get; private set; }

    private static string EventName
    {
      get
      {
        return "custom:publish:end:remote";
      }
    }

    public CustomPublishEndRemoteEvent(
      Publisher publisher, 
      OutputCacheClearingOptions outputCacheClearingOptions) : base(publisher)
    {
      Log.Info(this + " : constructor", this);
      Assert.ArgumentNotNull(outputCacheClearingOptions, "outputCacheClearingOptions");
      this.OutputCacheClearingOptions = outputCacheClearingOptions;
      //TODO: handle multiple publishing targets: this.PublishingTargets ?
    }

    public static void Raise(
      CustomPublishEndRemoteEvent customPublishEndRemoteEvent)
    {
      Log.Info(customPublishEndRemoteEvent + " : Raise()", customPublishEndRemoteEvent);
      Event.RaiseEvent(
        CustomPublishEndRemoteEvent.EventName, 
        new object[] { customPublishEndRemoteEvent.OutputCacheClearingOptions });
    }  
  }
}