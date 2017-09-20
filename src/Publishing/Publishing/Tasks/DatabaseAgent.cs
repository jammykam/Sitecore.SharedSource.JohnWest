namespace Sitecore.Sharedsource.Tasks
{
  using System;
  using System.Collections;

  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.Jobs;
  using Sitecore.Tasks;
  using Sitecore.Data.Items;

  public class DatabaseAgent
  {
    private string m_databaseName;
    private bool m_logActivity = true;
    private string m_scheduleRoot;

    public DatabaseAgent(string databaseName, string scheduleRoot)
    {
      Error.AssertString(databaseName, "databaseName", false);
      Error.AssertString(scheduleRoot, "scheduleRoot", false);
      this.m_databaseName = databaseName;
      this.m_scheduleRoot = scheduleRoot;
    }

    private ScheduleItem[] GetSchedules()
    {
      Log.Info(this + " : GetSchedules", this);
      Item item = this.Database.GetItem(this.ScheduleRoot);

      if (item == null)
      {
        return new ScheduleItem[0];
      }

      ArrayList list = new ArrayList();

      foreach (Item item2 in item.Axes.GetDescendants())
      {
        if (item2.TemplateID == TemplateIDs.Schedule
          || item2.Fields["OutputCacheClearingOptions"] != null)
        {
          list.Add(new ScheduleItem(item2));
        }
      }
      return (list.ToArray(typeof(ScheduleItem)) as ScheduleItem[]);
    }

    public void Run()
    {
      this.LogInfo("Scheduling.DatabaseAgent started. Database: " + this.m_databaseName);
      Job job = Context.Job;
      ScheduleItem[] schedules = this.GetSchedules();
      this.LogInfo("Examining schedules (count: " + schedules.Length + ")");
  
      if (this.IsValidJob(job))
      {
        job.Status.Total = schedules.Length;
      }

      foreach (ScheduleItem item in schedules)
      {
        try
        {
          if (item.IsDue)
          {
            this.LogInfo("Starting: " + item.Name + (item.Asynchronous ? " (asynchronously)" : string.Empty));
            item.Execute();
            this.LogInfo("Ended: " + item.Name);
          }
          else
          {
            this.LogInfo("Not due: " + item.Name);
          }
          if (item.AutoRemove && item.Expired)
          {
            this.LogInfo("Schedule is expired. Auto removing schedule item: " + item.Name);
            item.Remove();
          }
        }
        catch(Exception ex)
        {
          Log.Error(this.ToString(), ex, this);
        }

        if (this.IsValidJob(job))
        {
          JobStatus status = job.Status;
          status.Processed += 1L;
        }
      }
    }

    private void LogInfo(string message)
    {
      if (this.LogActivity)
      {
        Log.Info(this + " : " + message, this);
      }
    }

    private bool IsValidJob(Job job)
    {
      return ((job != null) && (job.Category == "schedule"));
    }

    public Database Database
    {
      get
      {
        Database database = Factory.GetDatabase(
          this.m_databaseName);
        Error.Assert(
          database != null, 
          "Could not find database: " + this.m_databaseName);
        return database;
      }
    }

    public bool LogActivity
    {
      get
      {
        return this.m_logActivity;
      }
      set
      {
        this.m_logActivity = value;
      }
    }

    public string ScheduleRoot
    {
      get
      {
        return this.m_scheduleRoot;
      }
    }
  }
}