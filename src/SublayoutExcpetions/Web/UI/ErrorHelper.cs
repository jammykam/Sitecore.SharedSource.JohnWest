namespace Sitecore.Sharedsource.Web.UI
{
  using System;
  using System.Web.Configuration;

  public static class ErrorHelper
  {
    private static CustomErrorsSection _config = null;
    private static string _error500url = null;

    public static CustomErrorsSection Config
    {
      get
      {
        if (_config == null)
        {
          System.Configuration.Configuration config =
            WebConfigurationManager.OpenWebConfiguration("/");
          _config =
            (CustomErrorsSection) config.GetSection("system.web/customErrors");
          Sitecore.Diagnostics.Assert.IsNotNull(_config, "customErrors");
        }

        return _config;
      }
    }

    public static string Error500Url
    {
      get
      {
        if (_error500url == null && Config != null)
        {
          CustomError error500 =
            Config.Errors.Get(500.ToString(System.Globalization.CultureInfo.InvariantCulture));

          if (error500 != null && !String.IsNullOrEmpty(error500.Redirect))
          {
            _error500url = error500.Redirect;
          }

          if (String.IsNullOrEmpty(_error500url))
          {
            _error500url = Config.DefaultRedirect;
          }

          if (_error500url == null)
          {
            _error500url = String.Empty;
          }
        }

        return _error500url;
      }
    }

    public static bool ShouldRedirect()
    {
      if (Config == null
          || Config.Mode == System.Web.Configuration.CustomErrorsMode.On
          || (Config.Mode == System.Web.Configuration.CustomErrorsMode.RemoteOnly
              && !System.Web.HttpContext.Current.Request.IsLocal))
      {
        return true;
      }

      return false;
    }

    public static void Redirect()
    {
      if (!String.IsNullOrEmpty(Error500Url))
      {
        Sitecore.Web.WebUtil.Redirect(
          Error500Url + "?aspxerrorpath=" + Sitecore.Web.WebUtil.GetLocalPath(Sitecore.Context.RawUrl));
      }
      else
      {
        Sitecore.Web.WebUtil.RedirectToErrorPage(
          "//TODO: replace with friendly error message");
      }
    }
  }
}