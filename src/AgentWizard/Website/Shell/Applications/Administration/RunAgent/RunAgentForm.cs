namespace Sitecore.Sharedsource.Shell.Applications.Administration.RunAgent
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using System.Reflection;

  public class RunAgentForm : Sitecore.Web.UI.Pages.WizardForm
  {
    // the list of agents
    public Sitecore.Web.UI.HtmlControls.Combobox SelectedAgent;

    // the list of thread priorities
    public Sitecore.Web.UI.HtmlControls.Combobox Priority;

    // result shown on final wizard page
    public Sitecore.Web.UI.HtmlControls.Literal Result;

    public Sitecore.Web.UI.HtmlControls.Literal Status;
    // status shown on job processing page

    // persist job handle associated with running agent
    protected string JobHandle
    {
      get
      {
        return StringUtil.GetString(base.ServerProperties["JobHandle"]);
      }
      set
      {
        Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(
          value, 
          "value");
        base.ServerProperties["JobHandle"] = value;
      }
    }

    // retrieve the values of the name attributes of the 
    // /configuraiton/sitecore/scheduling/agent elements
    // in the Web.config file
    protected String[] GetAgentNames()
    {
      List<string> names = new List<string>();

      foreach (XmlNode node in 
        Sitecore.Configuration.Factory.GetConfigNodes("scheduling/agent[@name!='']"))
      {
        string name = node.Attributes["name", String.Empty].Value;

        if (names.Contains(name))
        {
          throw new Exception("Duplicate agent " + name + " in web.config");
        }

        names.Add(name);
      }

      Sitecore.Diagnostics.Assert.IsTrue(names.Count > 0, "names < 1");
      return names.ToArray();
    }

    protected override void OnLoad(System.EventArgs e)
    {
      Sitecore.Diagnostics.Assert.IsNotNull(e, "e");
      base.OnLoad(e);

      // if the drop-down is not yet populated, 
      // populate it with the names of the agents
      if (this.SelectedAgent.Controls.Count < 1)
      {
        foreach (string agent in this.GetAgentNames())
        {
          Sitecore.Web.UI.HtmlControls.ListItem entry =
            new Sitecore.Web.UI.HtmlControls.ListItem();
          entry.Header = agent;
          entry.Value = agent;
          this.SelectedAgent.Controls.Add(entry);
        }

        foreach(string name 
          in Enum.GetNames(typeof(System.Threading.ThreadPriority)))
        {
          Sitecore.Web.UI.HtmlControls.ListItem entry =
            new Sitecore.Web.UI.HtmlControls.ListItem();
          entry.Header = name;
          entry.Value = name;
          this.Priority.Controls.Add(entry);
          this.Priority.Value = 
            System.Threading.ThreadPriority.BelowNormal.ToString();
        }
      }
    }

    protected override void ActivePageChanged(string page, string oldPage)
    {
      base.ActivePageChanged(page, oldPage);

      // if the wizard has reached the page that invokes the agent
      if (page == "Running")
      {
        // the name attribute of the agent to invoke
        string agent = Sitecore.Context.ClientPage.ClientRequest.Form["SelectedAgent"];
        Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(agent, "agent");

        // don't let the user do anything while the agent runs
        base.NextButton.Disabled = true;
        base.BackButton.Disabled = true;
        base.CancelButton.Disabled = true;

        // retrieve the agent definition from the Web.config file
        XmlNode node = Sitecore.Configuration.Factory.GetConfigNode(
          "scheduling/agent[@name='" + agent + "']");
        Sitecore.Diagnostics.Assert.IsNotNull(node, "node");

        // create an object that represents the agent
        object toRun = Sitecore.Configuration.Factory.CreateObject(node, true);

        // invoke this method of the object that represents the agent
        string method = StringUtil.GetString(
          new string[]
          {
            Sitecore.Xml.XmlUtil.GetAttribute("method", node), "Execute"
          });

        // if a managed site named scheduler exists, 
        // invoke the agent under that context site, 
        // otherwise the currentcontext site
        string contextSite = Sitecore.Context.Site.Name;

        if (Sitecore.Configuration.Factory.GetSite("scheduler") != null)
        {
          contextSite = "scheduler";
        }

        // specify the agent to invoke
        Sitecore.Jobs.JobOptions jobOptions = new Sitecore.Jobs.JobOptions(
          agent, /* job name */
          "Interactive agent", /* job category*/
          contextSite, /* context site in which job runs */
          toRun, /* object containing method to invoke as a job */
          method, /* name of the method to invoke as a job */
          new object[] {} /* arguments to pass to the method */)
        {
          ContextUser = Sitecore.Context.User,
          AfterLife = TimeSpan.FromMinutes(5),
          WriteToLog = true,
        };

        Type enumType = typeof(System.Threading.ThreadPriority);
        object priorityValue = Enum.Parse(
          typeof (System.Threading.ThreadPriority),
          this.Priority.Value,
          true /*ignore the character case of the string to parse*/);
        PropertyInfo priorityProperty = 
          jobOptions.GetType().GetProperty("Priority");  
        priorityProperty.SetValue(
          jobOptions,
          priorityValue,
          null /*index required only for indexed properties*/);  

        // start the agent
        Sitecore.Jobs.Job job = Sitecore.Jobs.JobManager.Start(jobOptions);

        // store the handle of the job that invoked the agent
        this.JobHandle = job.Handle.ToString();

        // refresh the wizard UI for the Running page in 100ms
        Sitecore.Web.UI.Sheer.SheerResponse.Timer("CheckStatus", 100);
      }
    }

    // refresh the wizard UI for the Running page
    public void CheckStatus()
    {
      // retrieve the job that invoked the agent
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(this.JobHandle, "JobHandle");
      Sitecore.Handle handle = Sitecore.Handle.Parse(this.JobHandle);
      Sitecore.Diagnostics.Assert.IsNotNull(handle, "handle");
      Sitecore.Jobs.Job job = Sitecore.Jobs.JobManager.GetJob(handle);
      Sitecore.Diagnostics.Assert.IsNotNull(job, "job");

      // if the job agent finished
      if (job.Status.State != Sitecore.Jobs.JobState.Running)
      {
        // if the job or agent failed
        if (job.Status.Failed)
        {
          this.Result.Text = "Job failed.";
        }
        else
        {
          // otherwise assume the agent succeeded
          this.Result.Text = "Job succeeded.";
        }

        // report the most recent message written by the agent
        if (job.Status.Messages.Count > 0)
        {
          this.Result.Text += " Last status message: " + job.Status.Messages[job.Status.Messages.Count - 1];
        }

        // move the wizard to its final page
        base.Active = "LastPage";
      }
      else
      {
        // the agent is still running (or queueud?)
        // report the most recent message written by the agent
        if (job.Status.Messages.Count > 0)
        {
          this.Status.Text = "Last status message: " + job.Status.Messages[job.Status.Messages.Count - 1];
        }
        else
        {
          this.Status.Text = "No status messages available.";
        }

        // refresh the wizard UI for the Running page in 100ms
        Sitecore.Web.UI.Sheer.SheerResponse.Timer("CheckStatus", 100);
      }
    }
  }
}