namespace Sitecore.Sharedsource.Shell.Applications
{
  public class ImageSourceHelper
  {
    public const string SITE_SOURCE_PREFIX = "site";
    public const string MEDIA_ROOT = "/sitecore/media library";

    public static string GetSiteMediaPath(Sitecore.Data.Items.Item item)
    {
      Sitecore.Data.Items.Item site = item;

      while (site != null && site.Parent.Key != "content")
      {
        site = site.Parent;
      }

      if (site == null)
      {
        return null;
      }

      Sitecore.Data.Items.Item path = item.Database.GetItem(MEDIA_ROOT + "/" + site.Name);

      if (path == null)
      {
        return null;
      }

      return path.Paths.FullPath;
    }
  }
}