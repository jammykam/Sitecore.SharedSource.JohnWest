namespace SitecoreJohn.Eventing.Remote
{
  using System.Runtime.Serialization;

  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.Events;

  [DataContract]
  public class LogRemoteEvent
  {
    public LogRemoteEvent(LogEventData logEventData)
    {
      this.LogEventData = logEventData;
    }

    [DataMember]
    public LogEventData LogEventData { get; private set; }

    private static string EventName
    {
      get
      {
        return "custom:log:event:remote";
      }
    }

    public static void Raise(
      LogRemoteEvent remoteLogEvent)
    {
      Assert.ArgumentNotNull(remoteLogEvent, "remoteLogEvent");
      Event.RaiseEvent(
        LogRemoteEvent.EventName,
        new object[] { remoteLogEvent.LogEventData });
    }

    public static void Queue(
      object obj,
      Database db,
      LogEventData logEventData)
    {
      Assert.ArgumentNotNull(db, "db");
      Assert.ArgumentNotNull(logEventData, "logEventData");
      db.RemoteEvents.Queue.QueueEvent<LogRemoteEvent>(
        new LogRemoteEvent(logEventData), 
        true /*addToGlobalQueue*/, 
        true /*addToLocalQueue*/);
    }
  }
}