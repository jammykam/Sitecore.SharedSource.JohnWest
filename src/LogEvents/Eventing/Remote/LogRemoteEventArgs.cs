namespace SitecoreJohn.Eventing.Remote
{
  using Sitecore.Events;

  public class LogRemoteEventArgs : IPassNativeEventArgs
  {
    public LogRemoteEventArgs(LogRemoteEvent @event)
    {
      this.LogEventData = @event.LogEventData;
    }

    public LogEventData LogEventData { get; private set; }
  }
}