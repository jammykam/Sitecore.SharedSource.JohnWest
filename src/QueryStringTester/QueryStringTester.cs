using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.UI;

namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  public class QueryStringTester : Sitecore.Web.UI.WebControl
  {
    protected override void DoRender(HtmlTextWriter output)
    {
      Sitecore.Data.Items.Item item = Sitecore.Context.Item;

      if (item == null)
      {
        return;
      }

      Sitecore.Data.Fields.ImageField field = item.Fields["Logo"];

      if (field == null || field.MediaItem == null)
      {
        return;
      }

      string icon = Sitecore.StringUtil.EnsurePrefix('/',
        field.MediaItem.Appearance.Icon);
      output.WriteLine(String.Format(
        @"Icon: <img src=""{0}""><br />",
        icon));

      string thumbnail = Sitecore.StringUtil.EnsurePrefix(
        '/', 
        Sitecore.Resources.Media.MediaManager.GetThumbnailUrl(field.MediaItem));
      output.WriteLine(String.Format(
        @"Thumbnail: <img src=""{0}""><br />",
        thumbnail));

      Sitecore.Resources.Media.MediaUrlOptions options =
        new Sitecore.Resources.Media.MediaUrlOptions
        {
          Database = field.MediaItem.Database,
          Language = field.MediaItem.Language,
          Version = field.MediaItem.Version,
          DisableBrowserCache = true,
          BackgroundColor = Color.DarkGreen,
          Height = 100,
          Width = 100,
          AllowStretch = true,
        };
      string url = Sitecore.StringUtil.EnsurePrefix('/', 
        Sitecore.Resources.Media.MediaManager.GetMediaUrl(field.MediaItem, options));
      output.WriteLine(String.Format(
        @"Stretch: <img src=""{0}""><br />",
        url));
    }
  }
}