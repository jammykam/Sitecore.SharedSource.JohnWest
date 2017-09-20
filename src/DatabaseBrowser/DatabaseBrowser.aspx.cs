namespace Sitecore.Sharedsource.Web.UI.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;

  using SC = Sitecore;

  public partial class DatabaseBrowser : Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.AccessDenied())
      {
        return;
      }

      SC.Data.Items.Item item = this.GetItem();
      this.Initialize(item);
      this.BuildNavigation(item);
      this.HandleMedia(item);
      this.AddRowsForFields(item);
    }

    private bool AccessDenied()
    {
      if (SC.Context.User != null && SC.Context.User.IsAdministrator)
      {
        return false;
      }

      string url = !String.IsNullOrEmpty(SC.Context.Site.LoginPage)
        ? SC.Context.Site.LoginPage
        : SC.Configuration.Settings.NoAccessUrl;

      if (SC.Configuration.Settings.RequestErrors.UseServerSideRedirect)
      {
        this.Server.Transfer(url);
      }
      else
      {
        this.Response.Redirect(url);
      }

      return true;
    }

    private void Initialize(SC.Data.Items.Item item)
    {
      this.Title = SC.Globalization.Translate.Text(this.Title) + " : " + item.Paths.FullPath;
      this.Heading.InnerText = item.Paths.FullPath;
      HtmlTableRow header = new HtmlTableRow();
      header.Cells.Add(new HtmlTableCell() { InnerHtml = SC.Globalization.Translate.Text("Path") });
      header.Cells.Add(new HtmlTableCell() { InnerHtml = item.Paths.FullPath });
      this.Metadata.Rows.Add(header);
    }

    private void BuildNavigation(SC.Data.Items.Item item)
    {
      // for media folders, include a link to the parent
      // (should be equivalent to the browser back button)
      if (item.Parent != null)
      {
        if (item.Parent.ID != item.Database.GetRootItem().ID)
        {
          this.LinkList.Controls.Add(this.GetLink(
            item.Parent,
            ".. (" + item.Parent.Name + ")"));
        }
        else
        {
          string layout = SC.Configuration.Settings.DefaultLayoutFile;

          if (!string.IsNullOrEmpty(layout))
          {
            this.LinkList.Controls.Add(new LiteralControl(string.Format(
              "<li><img src=\"{0}\"> <a href=\"{1}\">.. (/sitecore)</a></li>",
              this.GetIcon(item.Database.GetRootItem()),
              layout)));
          }
        }
      }

      // if the folder has no contents
      if (!item.HasChildren)
      {
        this.Heading.InnerText += " (" + SC.Globalization.Translate.Text("contains no items" + ")");
      }
      else
      {
        foreach (SC.Data.Items.Item child in item.Children)
        {
          this.LinkList.Controls.Add(this.GetLink(child));
        }
      }
    }

    private void HandleMedia(SC.Data.Items.Item item)
    {
      // for media itemns
      if (item.Paths.IsMediaItem && item.TemplateID != SC.TemplateIDs.MediaFolder)
      {
        // add a link to open the media item in a new window or tab
        HtmlTableRow linkRow = new HtmlTableRow();
        linkRow.Cells.Add(new HtmlTableCell()
        {
          ColSpan = 2,
          InnerHtml = string.Format(
            "<a target=\"_blank\" href=\"{0}\">{0}</a>",
            SC.Resources.Media.MediaManager.GetMediaUrl(item))
        });
        this.Metadata.Rows.Add(linkRow);

        // for images, add an <img> element with a link that opens the image in a new window or tab
        if (!string.IsNullOrEmpty(item["extension"])
          && SC.Configuration.Settings.ImageTypes.Contains("|" + item["extension"] + "|"))
        {
          HtmlTableRow previewRow = new HtmlTableRow();
          previewRow.Cells.Add(new HtmlTableCell() { InnerHtml = "&#160;" });
          previewRow.Cells.Add(new HtmlTableCell()
          {
            InnerHtml = string.Format(
              "<a target=\"_blank\" href=\"{0}\"><img src=\"{0}\" alt=\"{1}\" /></a>",
              SC.Resources.Media.MediaManager.GetMediaUrl(item, new SC.Resources.Media.MediaUrlOptions() { MaxWidth = 400 }),
              item["alt"])
          });
          this.Metadata.Rows.Add(previewRow);
        }
      }
    }

    private void AddRowsForFields(SC.Data.Items.Item item)
    {
      // list of keys of fields not to display
      List<string> exclude = new List<string>();

      // render these fields first, even if they don't have content
      foreach (string fieldName in new[] { "alt", "width", "height", "extension" })
      {
        exclude.Add(fieldName);
        SC.Data.Fields.Field field = item.Fields[fieldName];

        if (field == null)
        {
          continue;
        }

        HtmlTableRow row = new HtmlTableRow();
        row.Cells.Add(new HtmlTableCell() { InnerHtml = this.GetFieldTitle(field) });
        string value = SC.Web.UI.WebControls.FieldRenderer.Render(item, field.Name);
        row.Cells.Add(new HtmlTableCell() { InnerHtml = string.IsNullOrEmpty(value) ? "&#160;" : value });
        this.Metadata.Rows.Add(row);
      }

      // if the Size field specifies a number we can parse, then show it
      SC.Data.Fields.Field size = item.Fields["size"];

      if (size != null && !string.IsNullOrEmpty(size.Value))
      {
        long temp;
        
        if (long.TryParse(size.Value, out temp))
        {
          exclude.Add("size");
          HtmlTableRow row = new HtmlTableRow();
          row.Cells.Add(new HtmlTableCell() { InnerHtml = this.GetFieldTitle(size) });
          row.Cells.Add(new HtmlTableCell() { InnerHtml = SC.MainUtil.FormatSize(temp) });
          this.Metadata.Rows.Add(row);
        }
      }

      // output the values of any remaining fields
      foreach (SC.Data.Fields.Field field in item.Fields)
      {
        if (exclude.Contains(field.Key))
        {
          continue;
        }

        HtmlTableRow row = new HtmlTableRow();
        row.Cells.Add(new HtmlTableCell() { InnerHtml = this.GetFieldTitle(field) });
        string html = field.Value;

        if (field.Type == "rich text" || field.Type == "image")
        {
          html = SC.Web.UI.WebControls.FieldRenderer.Render(item, field.Key);
        }

        if (string.IsNullOrEmpty(html))
        {
          continue;
        }

        row.Cells.Add(new HtmlTableCell() { InnerHtml = html });
        this.Metadata.Rows.Add(row);
      }
    }

    // retrieves the context item, defaulting to the root of the media library
    // if the DefaultLayoutFile setting is absent, or the root of the context 
    // database otherwise. Reads and sorts the fields of the item.
    private SC.Data.Items.Item GetItem()
    {
      SC.Data.Items.Item item = SC.Context.Item;

      // if the path in the URL does not specify an item
      if (item == null)
      {
        SC.Diagnostics.Assert.IsNotNull(SC.Context.Database, "Sitecore.Context.Database");

        // default to the root of the media library
        // for example if the user accessed /layouts/MediaLibraryBrowser.aspx directly

        if (string.IsNullOrEmpty(SC.Configuration.Settings.DefaultLayoutFile))
        {
          item = SC.Context.Database.GetItem(SC.ItemIDs.MediaLibraryRoot);
        }
        else
        {
          item = SC.Context.Database.GetRootItem();
        }

        SC.Diagnostics.Assert.IsNotNull(item, "item");
      }

      item.Fields.ReadAll();
      item.Fields.Sort();
      return item;
    }

    private Control GetLink(SC.Data.Items.Item item, string name = null)
    {
      // by default, use paths as item URLs
      string url = item.Paths.FullPath;


      // if the DefaultLayoutFile setting is absent,
      // for media items that do not specify a layout,
      // link to the media handler.
      if (string.IsNullOrEmpty(SC.Configuration.Settings.DefaultLayoutFile)
        && item.Paths.IsMediaItem
        && item.Visualization.GetLayout(SC.Context.Device) == null)
      {
        url = SC.Resources.Media.MediaManager.GetMediaUrl(item);
      }

      return new LiteralControl(string.Format(
        "<li><img src=\"{0}\"> <a href=\"{1}\">{2}</a>{3}</li>",
        this.GetIcon(item),
        url,
        string.IsNullOrEmpty(name) ? item.DisplayName : name,
        string.IsNullOrEmpty(item["extension"]) ? string.Empty : " (" + item["extension"] + ")"));
    }
    
    // retrieve the title of the field.
    private string GetFieldTitle(SC.Data.Fields.Field field)
    {
      return string.IsNullOrEmpty(field.Title) ? field.DisplayName : field.Title;
    }

    // retrieve the URL of the icon associated with the item
    protected string GetIcon(SC.Data.Items.Item item)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(item, "item");
      string icon = item.Appearance.Icon;

      // exapnd icons that are not already absolute 
      if (!(icon.StartsWith("/") || icon.Contains(":")))
      {
        icon = SC.Resources.Images.GetThemedImageSource(icon);
      }

      // remove prefixes such as /sitecore/shell/themes/standard
      int index = icon.IndexOf(
        SC.Configuration.Settings.Media.MediaLinkPrefix,
        StringComparison.Ordinal);

      if (index > -1)
      {
        icon = icon.Substring(index - 1);
      }

      return icon.Replace("32x32", "16x16");
    }
  }
}