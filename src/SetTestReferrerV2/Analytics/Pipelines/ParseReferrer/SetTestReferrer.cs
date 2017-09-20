namespace Sitecore.Sharedsource.Analytics.Pipelines.ParseReferrer
{
  using System;
  using System.Collections.Generic;

  public class SetTestReferrer
  {
    private List<string> _prefixes = new List<string>();
    private List<string> _keywords = new List<string>();

    public List<string> Prefixes
    {
      get
      {
        return this._prefixes;
      }
    }

    public List<string> Keywords
    {
      get
      {
        return this._keywords;
      }
    }

    public bool AppendTicks
    {
      get; 
      set;
    }

    public void Process(
      Sitecore.Analytics.Pipelines.ParseReferrer.ParseReferrerArgs args)
    {
      Sitecore.Diagnostics.Assert.IsNotNull(args, "args");
      string term = this.Keywords[new Random().Next(this.Keywords.Count)];

      if (this.AppendTicks)
      {
        term += ":" + DateTime.Now.Ticks;
      }

      args.UrlReferrer = new Uri(
        this.Prefixes[new Random().Next(this.Prefixes.Count)] + term);

      Sitecore.Diagnostics.Log.Info(this + " : referrer set to " + args.UrlReferrer, this);
    }
  }
}
