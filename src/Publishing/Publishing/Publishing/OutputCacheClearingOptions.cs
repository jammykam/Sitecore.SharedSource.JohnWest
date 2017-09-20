namespace Sitecore.Sharedsource.Pipelines.OutputCaches
{
  using System;
  using System.Collections.Generic;

  using Sitecore.Diagnostics;
  using Sitecore.Web;

  [Serializable]
  public class OutputCacheClearingOptions
  {
    public List<string> CacheKeysMustContainOne { get; set; }

    public List<string> CacheKeysMustNotContainAny { get; set; }

    public bool ClearAllOutputCachesCompletely { get; set; }

    public List<string> IgnoreSites { get; private set; }

    public List<string> ClearOutputCachesForSitesNamed { get; private set; }

    public void ClearCacheForSite(string site)
    {
      if (this.ClearOutputCachesForSitesNamed == null)
      {
        this.ClearOutputCachesForSitesNamed = new List<string>();
      }

      if (!this.ClearOutputCachesForSitesNamed.Contains(site))
      {
        this.ClearOutputCachesForSitesNamed.Add(site);
      }
    }

    public bool PublishedSomething;

    public List<string> PublishingTargets { get; private set; }
 
    public OutputCacheClearingOptions()
    {
      this.PublishingTargets = new List<string>();
      this.SetIgnoreSites();
      this.ClearOutputCachesForSitesNamed = new List<string>();
    }

    private void SetIgnoreSites()
    {
      this.IgnoreSites = new List<string>();

      foreach (SiteInfo siteInfo
        in Configuration.Factory.GetSiteInfoList())
      {
        if (!siteInfo.CacheHtml)
        {
          this.IgnoreSites.Add(siteInfo.Name.ToLowerInvariant());
        }
      }
    }

    public string PublishingLanguage { get; set; }

    public bool IgnorePublishingLanguage { get; set; }

    public bool IgnoreIntervals { get; set; }

    public bool IgnorePublishingTargets { get; set; }

    public string EventName { get; set; }

    public void InfoDump()
    {
      Log.Info(this + " : EventName : " + this.EventName, this);
      Log.Info(this + " : ClearAllOutputCachesCompletely : " + this.ClearAllOutputCachesCompletely, this);
      Log.Info(this + " : PublishedSomething : " + this.PublishedSomething, this);
      Log.Info(this + " : PublishingLanguage : " + this.PublishingLanguage, this);
      Log.Info(this + " : IgnorePublishingLanguage : " + this.IgnorePublishingLanguage, this);
      Log.Info(this + " : IgnoreIntervals : " + this.IgnoreIntervals, this);
      Log.Info(this + " : IgnorePublishingTargets : " + this.IgnorePublishingTargets, this);

      if (this.CacheKeysMustContainOne == null || this.CacheKeysMustContainOne.Count < 1)
      {
        Log.Info(this + " : CacheKeysMustContainOne null or empty : " + this.CacheKeysMustContainOne, this);
      }
      else
      {
        foreach (string key in this.CacheKeysMustContainOne)
        {
          Log.Info(this + " : CacheKeysMustContainOne : " + key, this);
        }
      }

      if (this.CacheKeysMustNotContainAny == null || this.CacheKeysMustNotContainAny.Count < 1)
      {
        Log.Info(this + " : CacheKeysMustNotContainAny null or empty : " + this.CacheKeysMustNotContainAny, this);
      }
      else
      {
        foreach (string key in this.CacheKeysMustNotContainAny)
        {
          Log.Info(this + " : CacheKeysMustNotContainAny : " + key, this);
        }
      }

      if (this.ClearOutputCachesForSitesNamed == null || this.ClearOutputCachesForSitesNamed.Count < 1)
      {
        Log.Info(this + " : _clearOutputCachesForSites null or empty : " + this.ClearOutputCachesForSitesNamed, this);
      }
      else
      {
        foreach (string key in this.ClearOutputCachesForSitesNamed)
        {
          Log.Info(this + " : _clearOutputCachesForSites : " + key, this);
        }
      }

      if (this.PublishingTargets == null || this.PublishingTargets.Count < 1)
      {
        Log.Info(this + " : PublishingTargets null or empty : " + this.PublishingTargets, this);
      }
      else
      {
        foreach (string key in this.PublishingTargets)
        {
          Log.Info(this + " : PublishingTarget : " + key, this);
        }
      }
    }
  }
}