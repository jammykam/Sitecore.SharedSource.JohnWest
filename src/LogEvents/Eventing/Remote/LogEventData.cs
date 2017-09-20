namespace SitecoreJohn.Eventing.Remote
{
  using System;

  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;

  [Serializable]
  public class LogEventData
  {
    public LogEventData(
      ValidatorResult errorLevel,
      DateTime when,
      Exception exception = null)
    {
      Assert.ArgumentNotNull(errorLevel, "errorLevel");
      this.Instance = 
        Sitecore.Configuration.Settings.InstanceName;
      this.ErrorLevel = errorLevel;
      this.When = when;
      this.Exception = exception;
    }

    public string Instance { get; private set; }

    public DateTime When { get; private set; }

    public Exception Exception { get; private set; }

    public ValidatorResult ErrorLevel { get; private set; }

    public string Url { get; set; }

    public string User { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public string Site { get; set; }
  }
}