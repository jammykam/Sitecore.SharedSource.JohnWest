
namespace Sitecore.Sharedsource.Pipelines.HttpRequestBegin
{
  using System;

  public class LanguageResolver
  {
    private int _fallbackDepthLimit = 5;

    public int FallbackDepthLimit
    {
      get
      {
        return this._fallbackDepthLimit;
      }

      set
      {
        this._fallbackDepthLimit = value;
      }
    }

    public bool SetCulture
    {
      get;
      set;
    }

    public bool PersistLanguage
    {
      get;
      set;
    }

    public void Process(Sitecore.Pipelines.HttpRequest.HttpRequestArgs args)
    {
      if (Sitecore.Context.Item == null)
      {
        string message = String.Format(
          "{0} : context item null : {1}", 
          this.GetType().ToString(),
          Sitecore.Web.WebUtil.GetRawUrl());
        Sitecore.Diagnostics.Log.Error(message, this);
      }
      else if (Sitecore.Context.Site == null)
      {
        string message = String.Format(
          "{0} : context site null : {1}", 
          this.GetType().ToString(),
          Sitecore.Web.WebUtil.GetRawUrl());
        Sitecore.Diagnostics.Log.Error(message, this);
      }
      else if (this.LanguageSetFromUrlPath()
               || this.LanguageSetFromQueryString()
               || this.LanguageSetFromCookie()
               || this.LanguageSetFromBrowserPreferences()
               || this.LanguageSetFromContextSite()
               || this.LanguageSetFromDefaultSetting()
               || this.LanguageSetToFirstExistingLanguage())
      {
        if (this.SetCulture)
        {
          System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(Sitecore.Context.Language.Name);
          System.Threading.Thread.CurrentThread.CurrentCulture =
            System.Globalization.CultureInfo.CreateSpecificCulture(Sitecore.Context.Language.Name);
        }
      }
      else
      {
        string message = String.Format(
          "{0} : Unable to determine valid context language : {1}",
          this.GetType().ToString(), 
          Sitecore.Web.WebUtil.GetRawUrl());
        Sitecore.Diagnostics.Log.Error(message, this);
      }
    }

    private bool LanguageSetFromUrlPath()
    {
      return Sitecore.Context.Data.FilePathLanguage != null 
        && this.SetLanguage(Sitecore.Context.Data.FilePathLanguage.Name, false, true, 0);
    }

    private bool LanguageSetFromQueryString()
    {
      return this.SetLanguage(System.Web.HttpContext.Current.Request.QueryString["sc_lang"], true, true, 0);
    }

    private bool LanguageSetFromCookie()
    {
      return this.SetLanguage(
        Sitecore.Web.WebUtil.GetCookieValue(Sitecore.Context.Site.GetCookieKey("lang")), false, true, 0);
    }

    private bool LanguageSetFromBrowserPreferences()
    {
      string langs = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];

      if (!String.IsNullOrEmpty(langs))
      {
        foreach (string lang in Sitecore.StringUtil.Split(langs, ',', true))
        {
          string langName = lang;

          if (lang.IndexOf(';') > -1)
          {
            langName = lang.Substring(0, lang.IndexOf(';'));
          }

          if (this.SetLanguage(langName, false, false, 0))
          {
            return true;
          }
        }
      }

      return false;
    }

    private bool LanguageSetFromContextSite()
    {
      return this.SetLanguage(Sitecore.Context.Site.Language, false, true, 0);
    }

    private bool LanguageSetFromDefaultSetting()
    {
      return this.SetLanguage(Sitecore.Configuration.Settings.DefaultLanguage, false, true, 0);
    }

    private bool LanguageSetToFirstExistingLanguage()
    {
      foreach (Sitecore.Globalization.Language language in Sitecore.Context.Item.Database.Languages)
      {
        if (this.SetLanguage(language.Name, false, false, 0))
        {
          return true;
        }
      }

      return false;
    }

    private bool SetLanguage(string languageName, bool spanRequests, bool fallback, int fallbackDepth)
    {
      if (!String.IsNullOrEmpty(languageName))
      {
        foreach (Sitecore.Globalization.Language compare in Sitecore.Context.Item.Database.Languages)
        {
          if (languageName.Equals(compare.Name, StringComparison.InvariantCultureIgnoreCase))
          {
            if (this.HasVersionInLanguage(Sitecore.Context.Item, compare))
            {
              this.SetContextLanguage(compare, spanRequests);
              return true;
            }
            else if (fallback)
            {
              Sitecore.Data.Items.Item languageItem =
                Sitecore.Context.Item.Database.GetItem("/sitecore/system/languages" + compare.Name);

              if (languageItem != null)
              {
                string fallbackLanguageName = languageItem["fallback language"];

                if (!String.IsNullOrEmpty(fallbackLanguageName))
                {
                  if (fallbackLanguageName == languageName)
                  {
                    string message = String.Format(
                      "{0} : invalid fallback language {1} in {2}",
                      this.GetType().ToString(), 
                      fallbackLanguageName,
                      languageItem.Paths.FullPath);
                    Sitecore.Diagnostics.Log.Error(message, this);
                  }
                  else
                  {
                    if (fallbackDepth < this.FallbackDepthLimit)
                    {
                      return this.SetLanguage(
                        fallbackLanguageName, 
                        spanRequests, 
                        fallback, 
                        fallbackDepth++);
                    }
                    else
                    {
                      string message = String.Format(
                        "{0} : Fallback depth limit {1} exceeded processing {2}",
                        this.GetType().ToString(), 
                        this.FallbackDepthLimit, 
                        fallbackLanguageName);
                      Sitecore.Diagnostics.Log.Warn(message, this);
                    }
                  }
                }
              }
            }
          }
        }
      }

      return false;
    }

    private bool HasVersionInLanguage(
      Sitecore.Data.Items.Item item, 
      Sitecore.Globalization.Language language)
    {
      Sitecore.Data.Items.Item langItem = Sitecore.Context.Item.Database.GetItem(Sitecore.Context.Item.ID, language);
      return langItem.Versions.Count > 0;
    }

    private void SetContextLanguage(Sitecore.Globalization.Language language, bool spanRequests)
    {
      Sitecore.Context.SetLanguage(language, spanRequests);
      Sitecore.Context.Item = Sitecore.Context.Item.Database.GetItem(Sitecore.Context.Item.ID, language);

      if (spanRequests && this.PersistLanguage)
      {
        string cookieName = Sitecore.Context.Site.GetCookieKey("lang");
        Sitecore.Web.WebUtil.SetCookieValue(cookieName, language.Name, DateTime.MaxValue);
      }
    }
  }
}