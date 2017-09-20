namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Web.UI;

  using SC = Sitecore;

  public class UserLookup : SC.Web.UI.HtmlControls.Control
  {
    public UserLookup()
    {
      this.Class = "scContentControl";
      this.Activation = true;
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      string err = null;
      output.Write("<select" + this.GetControlAttributes() + ">");
      output.Write("<option value=\"\"></option>");
      IEnumerable<SC.Security.Accounts.User> users = null;
      bool includeAnonymous = false;

      if (!String.IsNullOrEmpty(this.Source))
      {
        NameValueCollection source = SC.Web.WebUtil.ParseUrlParameters(this.Source);

        if (!String.IsNullOrEmpty(source["domain"]))
        {
          SC.Security.Domains.Domain domain = 
            SC.SecurityModel.DomainManager.GetDomain(source["domain"]);

          if (domain != null)
          {
            users = domain.GetUsers();
          }
          else
          {
            err = SC.Globalization.Translate.Text("Security domain " + source["domain"] + " does not exist.");
          }
        }

        if (!String.IsNullOrEmpty(source["anonymous"]))
        {
          includeAnonymous = source["anonymous"] == "true";
        }
      }

      if (String.IsNullOrEmpty(err))
      {
        if (users == null)
        {
          users = SC.Security.Accounts.UserManager.GetUsers();
        }

        bool valueFound = String.IsNullOrEmpty(this.Value);

        foreach (SC.Security.Accounts.User user in users)
        {
          if (includeAnonymous
            || user.Domain.GetAnonymousUser() == null || user.Domain.GetAnonymousUser().Name != user.Name)
          {
            valueFound = valueFound || user.Name == this.Value;
            output.Write(
              String.Format(
                @"<option value=""{0}"" {1}>{2}</option>",
                user.Name,
                this.Value == user.Name ? " selected=\"selected\"" : String.Empty,
                user.Name));
          }
        }

        if (!valueFound)
        {
          err = SC.Globalization.Translate.Text("Value not in the selection list.");
        }
      }

      if (err != null)
      {
        output.Write("<optgroup label=\"" + err + "\">");
        output.Write("<option value=\"" + this.Value + "\" selected=\"selected\">" + this.Value + "</option>");
        output.Write("</optgroup>");
      }

      output.Write("</select>");

      if (err != null)
      {
        output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", err);
      }
    }

    protected override bool LoadPostData(string value)
    {
      this.HasPostData = true;

      if (value == null)
      {
        return false;
      }

      if (this.GetViewStateString("Value") != value)
      {
        UserLookup.SetModified();
      }

      this.SetViewStateString("Value", value);
      return true;
    }

    protected override void OnLoad(EventArgs e)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);

      if (!this.HasPostData)
      {
        this.LoadPostData(string.Empty);
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(e, "e");
      base.OnPreRender(e);
      this.ServerProperties["Value"] = this.ServerProperties["Value"];
    }

    private static void SetModified()
    {
      SC.Context.ClientPage.Modified = true;
    }

    public string Source
    {
      get;
      set;
    }

    public bool HasPostData
    {
      get;
      set;
    }
  }
}  