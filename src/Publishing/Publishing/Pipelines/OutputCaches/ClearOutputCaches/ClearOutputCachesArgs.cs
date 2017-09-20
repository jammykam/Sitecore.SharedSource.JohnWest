namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using System.Collections.Generic;

  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;

  public class ClearOutputCachesArgs : PipelineArgs
  {
    public List<OutputCacheSite> OutputCacheSites { get; private set; }
    public string EventName { get; set; }
    public OutputCacheClearingOptions OutputCacheClearingOptions { get; private set; }

    private string _lastClearedKey;

    public string LastClearedKey
    {
      get
      {
        if (string.IsNullOrEmpty(this._lastClearedKey))
        {
          this._lastClearedKey = this
            + "."
            + Sitecore.Configuration.Settings.InstanceName;

          if (!string.IsNullOrEmpty(this.EventName))
          {
            this._lastClearedKey += "." + this.EventName;
          }
        }

        return this._lastClearedKey;
      }
    }

    public ClearOutputCachesArgs(OutputCacheClearingOptions outputCacheClearingOptions)
    {
      this.OutputCacheSites = new List<OutputCacheSite>();
      this.OutputCacheClearingOptions = outputCacheClearingOptions;
    }

    public void AddOutputCacheSite(OutputCacheSite outputCacheSite)
    {
      Assert.ArgumentNotNull(outputCacheSite, "outputCacheSite");
      this.OutputCacheSites.Add(outputCacheSite);
    }

    public void RemoveOutputCacheSite(OutputCacheSite outputCacheSite)
    {
      Assert.ArgumentNotNull(outputCacheSite, "outputCacheSite");
      Assert.IsTrue(
        this.OutputCacheSites.Contains(outputCacheSite), 
        "collection does not contain site : " + outputCacheSite.SiteContext.Name);
      this.OutputCacheSites.Remove(outputCacheSite);
    }
  }
}