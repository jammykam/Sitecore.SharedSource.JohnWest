namespace Sitecore.Sharedsource.Analytics.Pipelines.ParseReferrer
{
  using System;
  using System.Collections.Generic;

  using Sitecore.Analytics;
  using Sitecore.Analytics.Pipelines.ParseReferrer;

  public class ParsePath
  {
    public List<string> Hostnames
    {
      get;
      private set;
    }

    public ParsePath()
    {
      this.Hostnames = new List<string>();
    }

    public void Process(ParseReferrerArgs args) 
    {
      Sitecore.Diagnostics.Log.Info(this + " : Process()", this);

      foreach (string hostname in this.Hostnames)
      {
        Sitecore.Diagnostics.Log.Info(this + " : hostname : " + hostname, this);
        
        if (args.UrlReferrer.DnsSafeHost.IndexOf(
          hostname, 
          StringComparison.CurrentCultureIgnoreCase) > -1)
        {
          Sitecore.Diagnostics.Log.Info(this + " : match!", this);

          string keywords = args.UrlReferrer.AbsolutePath.Trim('/');
          int slash = keywords.IndexOf("/");

          if (slash > -1)
          {
            keywords = keywords.Substring(0, slash);
          }

          keywords = System.Web.HttpUtility.UrlDecode(keywords);
          args.Visit.Keywords = Tracker.Visitor.DataContext.GetKeywords(
            keywords);
          return;
        }
      }
    }
  }
}