namespace Sitecore.Sharedsource.Shell.Applications.WebEdit.Commands
{
  using System;
  using Sitecore.Data.Items;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;
  using Sitecore.Exceptions;
  using Sitecore.Globalization;
  using Sitecore.Resources.Media;
  using Sitecore.Shell.Applications.Dialogs.MediaBrowser;
  using Sitecore.Shell.Applications.WebEdit;
  using Sitecore.Shell.Applications.WebEdit.Commands;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Shell.Applications.ContentEditor;

  public class ChooseImage : Sitecore.Shell.Applications.WebEdit.Commands.ChooseImage
  {
    protected new static void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Item itemNotNull = Client.GetItemNotNull(args.Parameters["itemid"]);
      itemNotNull.Fields.ReadAll();
      Field field = itemNotNull.Fields[args.Parameters["fieldid"]];
      Assert.IsNotNull(field, "field");
      ImageField imageField = new ImageField(field, field.Value);
      string controlid = args.Parameters["controlid"];

      if (args.IsPostBack)
      {
        if (args.Result != "undefined")
        {
          string rawValue;

          if (!string.IsNullOrEmpty(args.Result))
          {
            MediaItem item = Client.ContentDatabase.Items[args.Result];

            if (item != null)
            {
              MediaUrlOptions mediaOptions = new MediaUrlOptions();
              string mediaUrl = MediaManager.GetMediaUrl(item, mediaOptions);
              imageField.SetAttribute("mediaid", item.ID.ToString());
              imageField.SetAttribute("mediapath", item.MediaPath);
              imageField.SetAttribute("src", mediaUrl);

              if (!string.IsNullOrEmpty(args.Parameters["fieldValue"]))
              {
                XmlValue xmlValue = new XmlValue(args.Parameters["fieldValue"], "image");
                string height = xmlValue.GetAttribute("height");

                if (!String.IsNullOrEmpty(height))
                {
                  imageField.Height = height;
                }

                string width = xmlValue.GetAttribute("width");

                if (!String.IsNullOrEmpty(width))
                {
                  imageField.Width = width;
                }
              }
            }
            else
            {
              SheerResponse.Alert("Item not found.", new string[0]);
            }

            rawValue = imageField.Value;
          }
          else
          {
            rawValue = string.Empty;
          }

          string markup = WebEditImageCommand.RenderImage(args, rawValue);
          SheerResponse.SetAttribute("scHtmlValue", "value", markup);
          SheerResponse.SetAttribute("scPlainValue", "value", rawValue);
          SheerResponse.Eval("scSetHtmlValue('" + rawValue + "')");
        }
      }
      else
      {
        string text = StringUtil.GetString(new string[] { field.Source, ImageSourceHelper.MEDIA_ROOT });

        if (text.StartsWith(ImageSourceHelper.SITE_SOURCE_PREFIX))
        {
          string path = ImageSourceHelper.GetSiteMediaPath(itemNotNull);

          if (path != null)
          {
            text = path;
          }
        }

        string mediaid = imageField.GetAttribute("mediaid");
        
        if (text.StartsWith("~"))
        {
          if (string.IsNullOrEmpty(mediaid))
          {
            mediaid = StringUtil.Mid(text, 1);
          }

          text = ImageSourceHelper.MEDIA_ROOT;
        }

        Language language = itemNotNull.Language;
        MediaBrowserOptions mediaOptions = new MediaBrowserOptions();
        Item item3 = Client.ContentDatabase.GetItem(text, language);

        if (item3 == null)
        {
            throw new ClientAlertException("The source of this Image field points to an item that does not exist.");
        }

        mediaOptions.Root = item3;
        mediaOptions.AllowEmpty = true;

        if (!string.IsNullOrEmpty(mediaid))
        {
          Item selected = Client.ContentDatabase.GetItem(mediaid, language);

          if (selected != null)
          {
            mediaOptions.SelectedItem = selected;
          }
        }

        SheerResponse.ShowModalDialog(mediaOptions.ToUrlString().ToString(), true);
        args.WaitForPostBack();
      }
    }
  }
}