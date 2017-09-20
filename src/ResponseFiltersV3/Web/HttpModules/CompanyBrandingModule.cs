namespace Sitecore.Sharedsource.Web.HttpModules
{
  using System;
  using System.Web;

  public class CompanyBrandingModule : IHttpModule
  {
    private HttpApplication _httpApp;

    public void Init(HttpApplication httpApp)
    {
      this._httpApp = httpApp;
      httpApp.PostReleaseRequestState += PostReleaseRequestState;
    }

    public void Dispose()
    {
    }

    protected void PostReleaseRequestState(object obj, EventArgs args)
    {
      this._httpApp.Response.Filter = new IO.CompanyBrandingEnforcer(
        this._httpApp.Response.Filter);
    }
  }
}