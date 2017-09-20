namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;

  public class SiteImage : Sitecore.Shell.Applications.ContentEditor.Image
  {
    public string ItemID
    {
      get;
      set;
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      string source = this.ServerProperties["Source"].ToString();

      if (String.IsNullOrEmpty(this.ItemID)
        || String.IsNullOrEmpty(source)
        || !source.StartsWith(ImageSourceHelper.SITE_SOURCE_PREFIX))
      {
        return;
      }

      this.SetSource(String.Empty);
      Sitecore.Data.Items.Item current = Sitecore.Context.ContentDatabase.GetItem(this.ItemID);

      if (current == null)
      {
        return;
      }

      source = ImageSourceHelper.GetSiteMediaPath(current);

      if (source == null)
      {
        return;
      }

      this.SetSource(source);
    }

    protected void SetSource(string source)
    {
      base.ServerProperties["Source"] = source;
      this.Source = source;
    }
  }
}