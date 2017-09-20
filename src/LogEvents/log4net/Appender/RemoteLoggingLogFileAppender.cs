namespace SitecoreJohn.log4net.Appender
{
  using System;
  using System.Web;

  using global::log4net.Appender;
  using global::log4net.spi;

  using Sitecore.Data;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.Web;

  using SitecoreJohn.Eventing.Remote;

  public class RemoteLoggingLogFileAppender : SitecoreLogFileAppender
  {
    protected override void Append(LoggingEvent loggingEvent)
    {
      // assume we don't care about the requested URL
      // for anything lower than warning
      if (loggingEvent.Level != Level.WARN
        && loggingEvent.Level != Level.ERROR
        && loggingEvent.Level != Level.FATAL)
      {
        base.Append(loggingEvent);
        return;
      }

      string url = null;

      if (HttpContext.Current != null)
      {
        // includes the protocol and hostname, so try that first
        url = HttpContext.Current.Request.Url.AbsoluteUri;
      }
      else if (!string.IsNullOrEmpty(Sitecore.Context.RawUrl))
      {
        // this does not, but is probably always empty
        // if HttpContext.Current is null
        url = Sitecore.Context.RawUrl;
      }

      LoggingEventData data = 
        loggingEvent.GetLoggingEventData();

      if (!string.IsNullOrWhiteSpace(url))
      {
        data.Message += " URL: " + url;
      }

      if (Sitecore.Context.User != null)
      {
        // context user might be relevant 
        // for diagnosing permission issues
        data.Message += " (" + Sitecore.Context.User.Name + ')';
      }

      LoggingEvent ev = new LoggingEvent(data);
      base.Append(ev);

      // avoid infinite recursion!
      if (data.Message.Contains("error raising remote event"))
      {
        base.Append(loggingEvent);
        return;
      }

      try
      {
        Database db = Sitecore.Context.Database;

        if (db == null)
        {
          SiteInfo siteInfo = Sitecore.Configuration.Factory.GetSiteInfo(
            Sitecore.Configuration.Settings.Preview.DefaultSite);
          Assert.IsNotNull(
            siteInfo,
            "siteInfo : " + Sitecore.Configuration.Settings.Preview.DefaultSite);
          db = Sitecore.Configuration.Factory.GetDatabase(
            siteInfo.Database);
          Assert.IsNotNull(db, "db : " + siteInfo.Database);
        }

        ValidatorResult result = ValidatorResult.CriticalError;

        if (loggingEvent.Level == Level.WARN)
        {
          result = ValidatorResult.Warning;
        }
        else if (loggingEvent.Level == Level.FATAL)
        {
          result = ValidatorResult.FatalError;
        }

        LogEventData logEventData = new LogEventData(
          result,
          DateTime.Now)
        {
          Message = data.Message,
          Url = url,
          User = Sitecore.Context.User != null ? Sitecore.Context.User.Name : string.Empty,
          Site = Sitecore.Context.Site != null ? Sitecore.Context.Site.Name : string.Empty
        };

        if (!string.IsNullOrWhiteSpace(data.ExceptionString))
        {
          logEventData.Message = data.ExceptionString;
        }

        LogRemoteEvent.Queue(
          this,
          db,
          logEventData);
      }
      catch (Exception ex)
      {
        LoggingEventData log4netEventData =
          new LoggingEventData();
        log4netEventData.Message = 
          this + " : error raising remote event : " + ex.Message;
        log4netEventData.LoggerName = this.ToString();
        log4netEventData.Level = Level.CRITICAL;
        log4netEventData.ExceptionString = 
          ex.GetType() + " : " + ex.Message + " : " + ex.StackTrace;
        LoggingEvent log4netEvent = new LoggingEvent(
          log4netEventData);

        if (!string.IsNullOrWhiteSpace(url))
        {
          data.Message += " URL: " + url;
        }

        if (Sitecore.Context.User != null)
        {
          // context user might be relevant for diagnosing permission issues
          data.Message += " (" + Sitecore.Context.User.Name + ')';
        }

        base.Append(log4netEvent);
        throw ex;
      }
    }
  }
}