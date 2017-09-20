namespace Publishing.Pipelines.OutputCaches.ClearOutputCaches
{
  using System;
  using System.IO;
  using System.Xml.Serialization;

  using Sitecore;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Diagnostics;

  public class CheckInterval : ClearOutputCachesProcessor
  {
    protected override void DoProcess(ClearOutputCachesArgs args)
    {
      if (args.OutputCacheClearingOptions.IgnoreIntervals)
      {
        Log.Info(this + " : IgnoreIntervals.", this);
        return;
      }

      foreach (OutputCacheSite outputCacheSite in args.OutputCacheSites.ToArray())
      {
        if (!outputCacheSite.CacheHasExpired)
        {
          //TODO: schedule cache clearing for last clearance time + interval
          Log.Info(this + " : remove site " + outputCacheSite.SiteContext.Name + " because cache has not expired.", this);
          args.RemoveOutputCacheSite(outputCacheSite);
          Item command = outputCacheSite.SiteContext.Database.GetItem(
            "/sitecore/system/Tasks/Commands/ProcessOutputCaches");
          Item schedule = outputCacheSite.SiteContext.Database.GetItem(
            "/sitecore/system/Tasks/Schedules");
          Template template = TemplateManager.GetTemplate(
            "User Defined/Tasks/Schedule/ProcessOutputCaches",
            outputCacheSite.SiteContext.Database);
          
          if (command == null
            || schedule == null
            || template == null
            || template.GetField("OutputCacheClearingOptions") == null
            || template.GetField("Auto Remove") == null
            || template.GetField("Command") == null
            || template.GetField("Instance") == null
            || template.GetField("Schedule") == null)
          {
            Log.Warn(this + " : template for scheduling cache clearing operations does not exist or does not contain the correct fields.", this);
            return;
          }

          if (schedule.Axes.SelectSingleItem(
            ".//*[@Instance='" + Sitecore.Configuration.Settings.InstanceName +
            "' and @Command='" + command.ID + "']") != null)
          {
            Log.Info(this + " : cache clearing operation already scheduled for this instance.", this);
            return;
          }

            //TODO: only create if no schedule for same command exists for instance
          Item task = schedule.Add(
            string.Join(
              ".",
              DateUtil.ToIsoDate(DateTime.Now),
              Sitecore.Configuration.Settings.InstanceName,
              new Random().Next()).Replace(":", "_").Replace("-", "_").Replace(".", "_"),
            new TemplateID(template.ID));

          using (new EditContext(task))
          {
            XmlSerializer serializer = new XmlSerializer(
              args.OutputCacheClearingOptions.GetType());
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, args.OutputCacheClearingOptions);
            task["OutputCacheClearingOptions"] = sw.ToString();
            task["Command"] = command.ID.ToString();
            task["Auto Remove"] = "1";
            task["Instance"] = Sitecore.Configuration.Settings.InstanceName;
            task["Schedule"] = string.Join(
              "|",
              DateUtil.ToIsoDate(DateTime.MinValue).Substring(0, 8), 
              DateUtil.ToIsoDate(DateTime.MaxValue).Substring(0, 8),
              "127",
              "00:01:00");
          }
        }
      }
    }
  }
}