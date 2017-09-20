namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;
  using System.Web.UI;
  using System.Xml.XPath;

  public class XmlLookup : Sitecore.Web.UI.HtmlControls.Control
  {
    public XmlLookup()
    {
      this.Class = "scContentControl";
      this.Activation = true;
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      string err = null;
      output.Write("<select" + this.GetControlAttributes() + ">");
      output.Write("<option value=\"\"></option>");

      if (String.IsNullOrEmpty(this.Source))
      {
        err = "No source property specified for field.";
      }
      else
      {
        XPathDocument xdoc = new XPathDocument(this.Source);
        XPathNavigator xnav = xdoc.CreateNavigator();
        XPathNodeIterator xiter = xnav.Select("/*/*");
        bool valueFound = String.IsNullOrEmpty(this.Value);

        while (xiter.MoveNext())
        {
          string title = xiter.Current.GetAttribute("title", String.Empty);
          string value = xiter.Current.GetAttribute("value", String.Empty);
          valueFound = valueFound || value == this.Value;

          output.Write(String.Format(
            @"<option value=""{0}"" {1}>{2}</option>",
            value,
            this.Value == value ? " selected=\"selected\"" : String.Empty,
            title));
        }

        if (!valueFound)
        {
          err = Sitecore.Globalization.Translate.Text("Value not in the selection list.");
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

      Sitecore.Diagnostics.Log.Info(this + " : Field : " + this.GetViewStateString("Field"), this);
      Sitecore.Diagnostics.Log.Info(this + " : FieldName : " + this.GetViewStateString("FieldName"), this);

      if (this.GetViewStateString("Value") != value)
      {
        XmlLookup.SetModified();
      }

      this.SetViewStateString("Value", value);
      return true;
    }

    protected override void OnLoad(EventArgs e)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);

      if (!this.HasPostData)
      {
        this.LoadPostData(string.Empty);
      }
    }

    protected override void OnPreRender(EventArgs e)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(e, "e");
      base.OnPreRender(e);
      this.ServerProperties["Value"] = this.ServerProperties["Value"];
    }

    private static void SetModified()
    {
      Sitecore.Context.ClientPage.Modified = true;
    }

    // The ID of the item selected in the Content Editor (in Sitecore.Context.ContentDatabase).
    public string ItemId
    {
      get;
      set;
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