namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using System.ServiceModel.Syndication;

  using Sitecore.StringExtensions;

  public class NewsFeedRenderer : Sitecore.Web.UI.WebControls.FeedRenderer
  {
    public override SyndicationItem RenderItem()
    {
      Sitecore.Data.Items.Item item = this.GetItem();
      Sitecore.Diagnostics.Assert.IsNotNull(item, "item");
      Sitecore.Syndication.FeedRenderingParameters feedParams = 
        Sitecore.Syndication.FeedRenderingParameters.Parse(this.Parameters);

      SyndicationItem syndicationItem = new SyndicationItem
      {
        Id = item.ID.ToString()
      };

      if (!String.IsNullOrEmpty(feedParams.TitleField))
      {
        syndicationItem.Title = new TextSyndicationContent(
          RenderField(item, feedParams.TitleField), 
          TextSyndicationContentKind.Plaintext);
      }

      if (!String.IsNullOrEmpty(feedParams.BodyField))
      {
        syndicationItem.Content = new TextSyndicationContent(
          RenderField(item, feedParams.BodyField), 
          TextSyndicationContentKind.Html);
      }

      this.AddLink(item, syndicationItem);

      if (!String.IsNullOrEmpty(feedParams.DateField))
      {
        this.RenderDate(item, feedParams, syndicationItem);
      }

      if (!String.IsNullOrEmpty(feedParams.AuthorField))
      {
        syndicationItem.Authors.Add(new SyndicationPerson(item[feedParams.AuthorField]));
      }

      if (!String.IsNullOrEmpty(feedParams.EnclosureField))
      {
        this.AddEnclosure(syndicationItem, item, feedParams);
      }

      return syndicationItem;
    }

    private void AddEnclosure(
      SyndicationItem syndicationItem, 
      Sitecore.Data.Items.Item item, 
      Sitecore.Syndication.FeedRenderingParameters parameters)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(syndicationItem, "syndicationItem");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(item, "item");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(parameters, "parameters");
      Sitecore.Data.Fields.LinkField field = item.Fields[parameters.EnclosureField];

      if (field != null)
      {
        Sitecore.Data.Items.MediaItem targetItem = field.TargetItem;

        if (targetItem != null)
        {
          Sitecore.Resources.Media.MediaUrlOptions options = new Sitecore.Resources.Media.MediaUrlOptions
          {
            AbsolutePath = true,
            UseItemPath = true
          };

          string mediaUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(item, options);

          if (!String.IsNullOrEmpty(mediaUrl))
          {
            mediaUrl = Sitecore.Web.WebUtil.GetFullUrl(mediaUrl);
            string size = targetItem.Size.ToString();
            string mimeType = targetItem.MimeType;
            syndicationItem.ElementExtensions.Add(
              String.Format("enclosure url=\"{0}\" length=\"{1}\" type=\"{2}\"", mediaUrl, size, mimeType), String.Empty, String.Empty);
          }
        }
      }
    }

    private void RenderDate(
      Sitecore.Data.Items.Item item, 
      Sitecore.Syndication.FeedRenderingParameters feedParams, 
      SyndicationItem syndicationItem)
    {
      DateTimeOffset minValue;
      DateTime dateTime = DateUtil.IsoDateToDateTime(item[feedParams.DateField], DateTime.MaxValue);

      if (dateTime == DateTime.MaxValue)
      {
        Sitecore.Diagnostics.Log.Warn(
          "RSS couldn't parse date from the '{0}' field on the {1} item. Using item updated date instead.".FormatWith(new object[] { feedParams.DateField, item.Uri }), this);
        dateTime = item.Statistics.Updated;
      }

      try
      {
        if (dateTime == DateTime.MinValue)
        {
          minValue = DateTimeOffset.MinValue;
        }
        else if (dateTime == DateTime.MaxValue)
        {
          minValue = DateTimeOffset.MaxValue;
        }
        else
        {
          minValue = new DateTimeOffset(dateTime);
        }
      }
      catch (ArgumentOutOfRangeException)
      {
        minValue = DateTimeOffset.Now;
      }

      syndicationItem.PublishDate = minValue;
    }

    private void AddLink(Sitecore.Data.Items.Item item, SyndicationItem syndicationItem)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(item, "item");

      if (item.TemplateID.ToString() == "{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}")
      {
        item = item.Parent;
      }

      Sitecore.Links.UrlOptions defaultOptions = Sitecore.Links.UrlOptions.DefaultOptions;
      defaultOptions.AlwaysIncludeServerUrl = true;
      string itemUrl = Sitecore.Links.LinkManager.GetItemUrl(item, defaultOptions);
      syndicationItem.Links.Add(SyndicationLink.CreateAlternateLink(new Uri(itemUrl)));
    }
  }
}