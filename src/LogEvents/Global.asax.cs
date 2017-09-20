namespace SitecoreJohn
{
  using System;

  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;

  using SitecoreJohn.Eventing.Remote;

  public class Application : Sitecore.Web.Application
  {
    protected void Application_Error(object sender, EventArgs e)
    {
      try
      {
        if (Sitecore.Context.Database == null)
        {
          Log.Warn(this + " : Sitecore.Context.Database null", this);
          return;
        }

        Exception exception = Server.GetLastError();
        Assert.IsNotNull(exception, "exception");
        Log.Error(
          this + " : Exception processing " + Sitecore.Context.RawUrl, 
          exception, 
          this);
        LogEventData logEventData = new LogEventData(
          ValidatorResult.CriticalError,
          DateTime.Now,
          exception)
        {
          Message = exception.Message,
          Url = Sitecore.Context.RawUrl,
          User = Sitecore.Context.User != null ? Sitecore.Context.User.Name : string.Empty,
          Site = Sitecore.Context.Site != null ? Sitecore.Context.Site.Name : string.Empty
        };

        LogRemoteEvent.Queue(
          this, 
          Sitecore.Context.Database, logEventData);
      }
      catch(Exception ex)
      {
        try
        {
          Log.Error(
            this + " ; exception queuing log entry remote event : " + ex.Message, 
            ex, 
            this);
        }
        catch (Exception)
        {
        }
      }
    }
  }
}
