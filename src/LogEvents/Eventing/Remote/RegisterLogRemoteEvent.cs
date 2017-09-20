namespace SitecoreJohn.Eventing.Remote
{
  using System;

  using Sitecore.Eventing;
  using Sitecore.Events.Hooks;

  public class RegisterLogRemoteEvent : IHook
  {
    public void Initialize()
    {
      EventManager.Subscribe<LogRemoteEvent>(
        new Action<LogRemoteEvent>(LogRemoteEvent.Raise));
    }
  }
}