using System.Collections.Generic;

namespace Sitecore.Sharedsource.Web.UI.Layouts
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Text;
  using System.Web.UI;

  public partial class Export : System.Web.UI.Page
  {
    protected bool IncludeFilesystem
    {
      get;
      set;
    }

    protected bool IncludeRevision
    {
      get;
      set;
    }

    protected bool IncludeAPI
    {
      get;
      set;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      this.IncludeRevision = (!String.IsNullOrEmpty(this.Request.QueryString["rev"]));
      this.IncludeFilesystem = (!String.IsNullOrEmpty(this.Request.QueryString["fs"]));
      this.IncludeAPI = (!String.IsNullOrEmpty(this.Request.QueryString["api"]));

      foreach (Sitecore.Data.Database database in Sitecore.Configuration.Factory.GetDatabases())
      {
        if (String.Compare(database.Name, "filesystem", true) == 0 && !this.IncludeFilesystem)
        {
          continue;
        }

        StringBuilder sb = new StringBuilder();

        using (new Sitecore.SecurityModel.SecurityDisabler())
        {
          this.AppendPaths(sb, database.GetRootItem());
        }

        string file = "C:\\temp\\" + Sitecore.Configuration.Settings.InstanceName + "." + database.Name + ".lst";
        File.WriteAllText(file, sb.ToString());
        this.Controls.Add(new LiteralControl(file + " written.<br />"));
      }

      if (this.IncludeAPI)
      {
        foreach (string name in new[] { "sitecore.kernel", "sitecore.client", "sitecore.analytics" })
        {
          try
          {
            this.ExportAssembly(name);
          }
          catch (Exception ex)
          {
            this.Controls.Add(new LiteralControl(ex.Message + "<br />"));
            continue;
          }
        }
      }
    }

    protected void AppendPaths(StringBuilder sb, Sitecore.Data.Items.Item item)
    {
      string line = item.Paths.FullPath;

      if (this.IncludeRevision && !String.IsNullOrEmpty(item.Statistics.Revision))
      {
        line += "-" + item.Statistics.Revision;
      }

      sb.AppendLine(line);

      foreach (Sitecore.Data.Items.Item child in item.Children)
      {
        this.AppendPaths(sb, child);
      }
    }

    protected void ExportAssembly(string name)
    {
      List<string> lines = new List<string>();
      Assembly assembly = Assembly.Load(name);
      Type[] types = assembly.GetTypes();

      foreach (Type t in types)
      {
        lines.Add(t.Namespace + "." + t.Name);

        MethodInfo[] methods = t.GetMethods();
        foreach (MethodInfo m in methods)
        {
          lines.Add(t.Namespace + "." + t.Name + "." + m.Name + "()");
        }

        PropertyInfo[] props = t.GetProperties();
        foreach (PropertyInfo p in props)
        {
          lines.Add(t.Namespace + "." + t.Name + "." + p.Name);
        }

        FieldInfo[] fields = t.GetFields();
        foreach (FieldInfo f in fields)
        {
          lines.Add(t.Namespace + "." + t.Name + "." + f.Name);
        }
      }

      StringBuilder sb = new StringBuilder();
      lines.Sort();

      foreach (string line in lines)
      {
        sb.AppendLine(line);
      }

      string file = "C:\\temp\\" + Sitecore.Configuration.Settings.InstanceName + "." + name + ".lst";
      File.WriteAllText(file, sb.ToString());
      this.Controls.Add(new LiteralControl(file + " written.<br />"));
    }
  }
}