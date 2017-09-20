namespace Sitecore.Sharedsource.Eventing.Remote
{
  using System;

  using Sitecore.Eventing;
  using Sitecore.Events.Hooks;

  public class RegisterCustomPublishEndRemoteEvent : IHook
  {
    public void Initialize()
    {
      EventManager.Subscribe<CustomPublishEndRemoteEvent>(
        new Action<CustomPublishEndRemoteEvent>(CustomPublishEndRemoteEvent.Raise));
    }
  }
}