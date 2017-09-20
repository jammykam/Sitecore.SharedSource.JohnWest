namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor.FieldTypes
{
  using System.IO;
  using System.Web.UI;

  using SC = Sitecore;

  public class TreelistEx : SC.Shell.Applications.ContentEditor.FieldTypes.TreelistEx
  {
    public string ItemID
    {
      get
      {
        return StringUtil.GetString(this.ViewState["ItemID"]);
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.ViewState["ItemID"] = value;
      }
    }

    public new string ItemLanguage
    {
      get
      {
        return StringUtil.GetString(this.ViewState["ItemLanguage"]);
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.ViewState["ItemLanguage"] = value;
        base.ItemLanguage = value;
      }
    }

    public string ItemVersion
    {
      get
      {
        return StringUtil.GetString(this.ViewState["ItemVersion"]);
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.ViewState["ItemVersion"] = value;
      }
    }

    protected override void Render(HtmlTextWriter output)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(output, "output");
      output.Write(
        "<div id=\"" 
        + this.ID 
        + "\" class=\"scContentControl\" style=\"height:80px;overflow:auto;padding:4px\" ondblclick=\"javascript:return scForm.postEvent(this,event,'treelist:edit(id=" 
        + this.ID 
        + ")')\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
      this.RenderItems(output);
      output.Write("</div>");
    }

    protected new void Edit(SC.Web.UI.Sheer.ClientPipelineArgs args)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (this.Enabled)
      {
        if (args.IsPostBack)
        {
          if ((args.Result != null) && (args.Result != "undefined"))
          {
            string result = args.Result;

            if (result == "-")
            {
              result = string.Empty;
            }
            else if (this.Value != result)
            {
              SC.Context.ClientPage.Modified = true;
            }

            this.Value = result;
            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
            this.RenderItems(output);
            SC.Web.UI.Sheer.SheerResponse.SetInnerHtml(
              this.ID,
              output.InnerWriter.ToString());
          }
        }
        else
        {
          SC.Text.UrlString urlString = new SC.Text.UrlString(
            UIUtil.GetUri("control:TreeListExEditor"));
          SC.Web.UrlHandle handle = new SC.Web.UrlHandle();
          string str3 = this.Value;

          if (str3 == "__#!$No value$!#__")
          {
            str3 = string.Empty;
          }

          handle["value"] = str3;
          handle["source"] = this.Source;
          handle["language"] = this.ItemLanguage;
          handle["itemid"] = this.ItemID;
          handle["itemlanguage"] = this.ItemLanguage;
          handle["itemversion"] = this.ItemVersion;
          handle.Add(urlString);
          SC.Web.UI.Sheer.SheerResponse.ShowModalDialog(
            urlString.ToString(), 
            "800px", 
            "500px", 
            string.Empty, 
            true);
          args.WaitForPostBack();
        }
      }
    }

    protected string GetHeader(SC.Data.Items.Item item, string display)
    {
      string result = item.DisplayName;

      if (!string.IsNullOrEmpty(display))
      {
        if (display == "@name")
        {
          result = item.Name;
        }
        else if (display == "@path")
        {
          result = item.Paths.FullPath;
        }
        else if (!string.IsNullOrEmpty(item[display]))
        {
          result = item[display];
        }
      }

      return result;
    }

    private void RenderItems(HtmlTextWriter output)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(output, "output");
      SC.Collections.SafeDictionary<string> parameters =
        SC.Web.WebUtil.ParseQueryString(this.Source);
      string display = parameters["Display"];

      foreach (string str in this.Value.Split(new char[] { '|' }))
      {
        if (!string.IsNullOrEmpty(str))
        {
          SC.Data.Items.Item item = this.Database.GetItem(str);
          SC.Resources.ImageBuilder builder = new SC.Resources.ImageBuilder
          {
            Width = 0x10,
            Height = 0x10,
            Margin = "0px 4px 0px 0px",
            Align = "absmiddle"
          };

          if (item == null)
          {
            builder.Src = "Applications/16x16/forbidden.png";
            output.Write("<div>");
            builder.Render(output);
            output.Write(SC.Globalization.Translate.Text("Item not found: {0}", new object[] { str }));
            output.Write("</div>");
          }
          else
          {
            builder.Src = item.Appearance.Icon;
            output.Write("<div title=\"" + item.Paths.ContentPath + "\">");
            builder.Render(output);
            output.Write(this.GetHeader(item, display));
            output.Write("</div>");
          }
        }
      }
    }
  }
}
