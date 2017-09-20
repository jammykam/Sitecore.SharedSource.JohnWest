namespace Publishing.Pipelines.OutputCaches
{
  using System;

  using Sitecore;
  using Sitecore.Caching;
  using Sitecore.Diagnostics;
  using Sitecore.Sites;

  /// <summary>
  ///  Represents information about a managed site and its output cache.
  /// </summary>
  public class OutputCacheSite
  {
    public OutputCacheSite(
      SiteContext siteContext, 
      HtmlCache htmlCache, 
      string lastClearedKey)
    {
      Assert.ArgumentNotNull(siteContext, "siteContext");
      Assert.ArgumentNotNull(htmlCache, "htmlCache");
      Assert.ArgumentNotNullOrEmpty(lastClearedKey, "lastClearedKey");
      this.SiteContext = siteContext;
      this.HtmlCache = htmlCache;
      this.LastClearedKey = lastClearedKey;
    }

    private TimeSpan? _interval;

    private DateTime? _lastCleared;

    private string _languages;

    public SiteContext SiteContext { get; private set; }

    public HtmlCache HtmlCache { get; private set; }

    public string LastClearedKey { get; private set; }

    private TimeSpan Interval
    {
      get
      {
        if (!this._interval.HasValue)
        {
          string outputCacheMinimimInterval = 
            this.SiteContext.Properties["outputCacheMinimimInterval"];

          if (!string.IsNullOrEmpty(outputCacheMinimimInterval))
          {
            this._interval = TimeSpan.Parse(outputCacheMinimimInterval);
          }
          else
          {
            this._interval = TimeSpan.MinValue;
          }
        }

        return this._interval.Value;
      }
    }

    private string Languages
    {
      get
      {
        if (this._languages == null)
        {
          this._languages = this.SiteContext.Properties["clearOutputCacheAfterPublishingLanguages"];

          if (this._languages != null)
          {
            this._languages = this._languages.ToLowerInvariant();
          }
          else
          {
            this._languages = String.Empty;
          }
        }

        return this._languages;
      }
    }

    private DateTime LastCleared
    {
      get
      {
        if (!this._lastCleared.HasValue)
        {
          string lastCleared = this.HtmlCache.GetHtml(this.LastClearedKey);

          if (!string.IsNullOrEmpty(lastCleared))
          {
            Log.Info(this + " : Setting lastClared from " + lastCleared, this);
            this._lastCleared = DateUtil.IsoDateToDateTime(lastCleared);
          }
          else
          {
            this._lastCleared = DateTime.MinValue;
          }
        }

        return this._lastCleared.Value;
      }
    }

    public bool CacheHasExpired
    {
      get
      {
        Log.Info(this + " : LastCleared : " + this.LastCleared + " : Interval : " + this.Interval + " : " + (this.LastCleared == DateTime.MinValue || this.LastCleared.Add(this.Interval) < DateTime.Now), this);
        return this.LastCleared == DateTime.MinValue
          || this.LastCleared.Add(this.Interval) < DateTime.Now;
      }
    }

    public bool IsLanguageRelevant(string language)
    {
      Assert.ArgumentNotNullOrEmpty(language, "language");

      if (string.IsNullOrEmpty(this.Languages))
      {
        return true;
      }

      return ('|' + this.Languages + '|').Contains("|" + language.ToLowerInvariant() + "|");
    }

    public bool IsPublishingTargetRelevant(string target)
    {
      Assert.ArgumentNotNullOrEmpty(target, "target");
      return this.SiteContext.Database.Name.ToLowerInvariant() == target.ToLowerInvariant();
    }
  }
}