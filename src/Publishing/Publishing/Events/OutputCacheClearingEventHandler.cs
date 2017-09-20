namespace Sitecore.Sharedsource.Events
{
  using System;
  using System.Collections.Generic;

  using Sitecore.Diagnostics;
  using Sitecore.Publishing;
  using Sitecore.Sharedsource.Eventing.Remote;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;
  using Sitecore.Sharedsource.Publishing;

  public class OutputCacheClearingEventHandler
  {
    private List<string> _cacheKeyMustContainOne = new List<string>();
    private List<string> _cacheKeyMustNotContainAny = new List<string>();

    /// <summary>
    /// Clear output caches for managed sites 
    /// even if their database does not match the publishing target.
    /// </summary>
    public bool IgnorePublishingTargets { get; set; }

    /// <summary>
    /// Clear output caches for entire sites regardless of the language published
    /// </summary>
    public bool IgnorePublishingLanguage { get; set; }

    public bool IgnoreIntervals { get; set; }

    /// <summary>
    /// Add a string that cache keys must contain.
    /// </summary>
    /// <param name="value">The value that the cache key must contain.</param>
    public void AddMustContain(string value)
    {
      this._cacheKeyMustContainOne.Add(value);
    }

    /// <summary>
    /// Add a string that cache keys must not contain.
    /// </summary>
    /// <param name="value">Gets or sets the value that the cache key must not contain.</param>
    public void AddMustNotContain(string value)
    {
      Assert.ArgumentNotNullOrEmpty(value, "value");
      this._cacheKeyMustNotContainAny.Add(value);
    }

    public void HandleEvent(object sender, EventArgs eventArgs)
    {
      Log.Info(this + " : HandleEvent()", this);

      Sitecore.Events.SitecoreEventArgs scArgs =
        eventArgs as Sitecore.Events.SitecoreEventArgs;

      if (scArgs == null)
      {
        Log.Error(
          this + " : Unexpected everntArgs type in HandleEvent : " + eventArgs.GetType(), 
          this);
        return;
      }

      // if parameters were passed, it was the remote event; handle those options.
      OutputCacheClearingOptions outputCacheClearingOptions = 
        scArgs.Parameters[0] as OutputCacheClearingOptions;

      if (outputCacheClearingOptions != null)
      {
        Log.Info(this + " : handle remote event", this);
        outputCacheClearingOptions.EventName = scArgs.EventName;
        this.HandleIgnoreFlags(outputCacheClearingOptions);
        this.HandleContains(outputCacheClearingOptions);
        new OutputCacheClearer().ClearOutputCaches(outputCacheClearingOptions);
        return;
      }

      // otherwise, handle the local event that indicates what published
      lock(ClearSiteOutputCaches.OutputCacheClearingOptions)
      {
        Log.Info(this + " : handle local event()", this);
        ClearSiteOutputCaches.OutputCacheClearingOptions.EventName = scArgs.EventName;

        // don't raise remote event
        //TODO: have to let index rebuilds fall through here? only check if publisher is defined?
        if (!ClearSiteOutputCaches.OutputCacheClearingOptions.PublishedSomething)
        {
          Log.Info(this + " : !PublishedSomething : Reset() and return", this);
          ClearSiteOutputCaches.Reset();
          return;
        }

        Publisher publisher =
          scArgs.Parameters[0] as Publisher;

        if (publisher == null
          || publisher.Options == null
          || publisher.Options.TargetDatabase == null)
        {
          Log.Warn(this + " : no publisher, Options, or TargetDatabase :  return : " + publisher, this);
          return;
        }

        //TODO: what aboutpublisher.Options.PublishingTargets?
        ClearSiteOutputCaches.OutputCacheClearingOptions.PublishingTargets.Add( 
          publisher.Options.TargetDatabase.Name);

        if (publisher.Options.Language != null
          && !string.IsNullOrEmpty(publisher.Options.Language.Name))
        {
          Log.Info(this + " : publishing language " + publisher.Options.Language, this);
          ClearSiteOutputCaches.OutputCacheClearingOptions.PublishingLanguage
            = publisher.Options.Language.Name;
        }
        else
        {
          Log.Warn(this + " : no publishing language", this);
        }


        Log.Info(this + " : queue remote event for " + publisher.Options.TargetDatabase.Name, this);
        publisher.Options.TargetDatabase.RemoteEvents.Queue.QueueEvent<CustomPublishEndRemoteEvent>(
          new CustomPublishEndRemoteEvent(
             publisher, 
             ClearSiteOutputCaches.OutputCacheClearingOptions));

        this.HandleIgnoreFlags(
          ClearSiteOutputCaches.OutputCacheClearingOptions);
        this.HandleContains(outputCacheClearingOptions);
        new OutputCacheClearer().ClearOutputCaches(
          ClearSiteOutputCaches.OutputCacheClearingOptions);
        ClearSiteOutputCaches.Reset();
      }
    }

    private void HandleIgnoreFlags(OutputCacheClearingOptions outputCacheClearingOptions)
    {
      if (this.IgnoreIntervals)
      {
        outputCacheClearingOptions.IgnoreIntervals = true;
      }

      if (this.IgnorePublishingLanguage)
      {
        outputCacheClearingOptions.IgnorePublishingLanguage = true;
      }

      if (this.IgnorePublishingTargets)
      {
        outputCacheClearingOptions.IgnorePublishingTargets = true;
      }
    }

    private void HandleContains(OutputCacheClearingOptions outputCacheClearingOptions)
    {
      if (this._cacheKeyMustContainOne != null && this._cacheKeyMustContainOne.Count > 0)
      {
        foreach (string entry in this._cacheKeyMustContainOne)
        {
          if (!outputCacheClearingOptions.CacheKeysMustContainOne.Contains(
            entry))
          {
            outputCacheClearingOptions.CacheKeysMustContainOne.Add(
              entry);
          }
        }
      }

      if (this._cacheKeyMustNotContainAny != null && this._cacheKeyMustNotContainAny.Count > 0)
      {
        foreach (string entry in this._cacheKeyMustNotContainAny)
        {
          if (!outputCacheClearingOptions.CacheKeysMustNotContainAny.Contains(entry))
          {
            outputCacheClearingOptions.CacheKeysMustNotContainAny.Add(entry);
          }
        }
      }
    }
  }
}




