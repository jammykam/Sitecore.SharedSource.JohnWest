namespace SitecoreJohn.Eventing.Remote
{
  using System;

  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Events;
  using Sitecore.SecurityModel;

  public class LogEventHandler
  {
    public string Database { get; set; }

    public string Instance { get; set; }

    public void HandleEvent(object sender, EventArgs eventArgs)
    {
      if (!(string.IsNullOrWhiteSpace(this.Instance)
        || this.Instance.Replace('-', '_').Equals(
          Sitecore.Configuration.Settings.InstanceName.Replace('-', '_'),
          StringComparison.InvariantCultureIgnoreCase)))
      {
        return;
      }

      SitecoreEventArgs scArgs =
        eventArgs as SitecoreEventArgs;

      if (scArgs == null)
      {
        Log.Error(
          this + " : Unexpected everntArgs type in HandleEvent : " + eventArgs.GetType(),
          this);
        return;
      }

      LogEventData remoteLogEventData =
        scArgs.Parameters[0] as LogEventData;

      if (remoteLogEventData == null)
      {
        Log.Info(this + " : unexpected scArgs.Parameter[0] in HandleEvent : " + remoteLogEventData, this);
        return;
      }

      this.HandleLogEvent(remoteLogEventData);
    }

    private void HandleLogEvent(LogEventData logEventData)
    {
      try
      {
        Assert.ArgumentNotNull(logEventData, "logEventData");

        if (logEventData.Message.Contains("Unable to connect to server localhost"))
        {
          //TODO: there are just too many of these - ignore them for now.
          return;
        }

        Database db = Sitecore.Configuration.Factory.GetDatabase(
          this.Database);
        Assert.IsNotNull(db, "db: " + this.Database);
        Item item = db.GetItem(ItemIDs.SystemRoot);
        Assert.IsNotNull(
          item,
          "item : " + ItemIDs.SystemRoot + " in " + this.Database);
        TemplateItem logEventTemplate =
          db.Templates["User Defined/LogEvent"];
        Assert.IsNotNull(logEventTemplate, "logEventTemplate");

        if (item.Children["Events"] != null)
        {
          item = item.Children["Events"];
        }
        else
        {
          using (new SecurityDisabler())
          {
            item = item.Add("Events", new TemplateID(
              TemplateIDs.Folder));
            Assert.IsNotNull(
              item, 
              "item : Creating events folder");

            using (new EditContext(item))
            {
              item.Publishing.NeverPublish = true;
            }
          }
        }

        string instance = logEventData.Instance.Replace(
          '-', 
          '_');

        if (item.Children[instance] != null)
        {
          item = item.Children[instance];
        }
        else
        {
          using (new SecurityDisabler())
          {
            item = item.Add(
              instance, 
              new TemplateID(TemplateIDs.Folder));
            Assert.IsNotNull(
              item, 
              "item : Creating " + logEventData.Instance);

            using (new EditContext(item))
            {
              item[Sitecore.Buckets.Util.Constants.IsBucket] = "1";
            }
          }
        }

        this.CreateLogItem(item, logEventData, logEventTemplate);
      }
      catch (Exception ex)
      {
        // WARNING: using Warning() or above here 
        // could cause an infinite loop?
        Log.Info(
          this + " : " + ex.GetType() + " : " + ex.Message + " : " + ex.StackTrace, 
          this);
      }
    }

    private Item CreateLogItem(
      Item parent,
      LogEventData logEventData,
      TemplateItem logEventTemplate)
    {
      using (new SecurityDisabler())
      {
        string name = DateUtil.ToIsoDate(logEventData.When)
          + '_'
          + new Random().Next(int.MaxValue);
        Item item = parent.Add(
          name,
          logEventTemplate);
        Assert.IsNotNull(
          item, 
          "item : creating " + name);

        using (new EditContext(item))
        {
          item["When"] = DateUtil.ToIsoDate(logEventData.When);
          item["Instance"] = logEventData.Instance;
          item["Message"] = logEventData.Message; //TODO: 
//          item["Message"] = "\"" + logEventData.Message + "\""; 
          item["Error Level"] = logEventData.ErrorLevel.ToString();
          item["Site"] = logEventData.Site;
          item["URL"] = logEventData.Url;

          if (logEventData.Exception != null)
          {
            item["Exception Message"] = logEventData.Exception.Message;
            item["Exception Type"] =
              logEventData.Exception.GetType().ToString();
            item["Stack Trace"] = logEventData.Exception.StackTrace;

            ////TODO: log nested exceptions?
            //
            // Exception ex = logEventData.Exception.InnerException;
            //
            //while (ex != null)
            //{
            //  ex = ex.InnerException;
            //}
          }
          else
          {
            item["Stack Trace"] = logEventData.StackTrace;
          }

          this.ParseExceptionDetailsFromMessage(
            item, 
            logEventData.Message);
          return item;
        }
      }
    }

    private void ParseExceptionDetailsFromMessage(
      Item item,
      string message)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNullOrEmpty(message, "message");

      if (message.StartsWith("Exception:")
        && message.Contains("Message:")
        && message.Contains("Source:"))
      {
        int pos = message.IndexOf(
          "Message:",
          StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(item["Exception Type"]))
        {
          item["Exception Type"] = message.Substring(
            11,
            pos - 13).TrimEnd();
        }

        if (string.IsNullOrWhiteSpace(item["Exception Message"]))
        {
          item["Exception Message"] = message.Substring(
            pos + 9,
            message.IndexOf("Source:") - (pos + 10)).TrimEnd();
        }
      }
    }
  }
}