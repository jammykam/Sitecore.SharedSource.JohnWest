namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using System.Text;
  using System.Web.UI;

  public class LanguageFlags : Sitecore.Web.UI.WebControl
  {
    public LanguageFlags()
    {
      this.Separator = " ";
    }

    public string Separator
    {
      get;
      set;
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      Sitecore.Data.Items.Item item = this.GetItem();
      Sitecore.Diagnostics.Assert.IsNotNull(item, "item");
      StringBuilder sb = new StringBuilder();

      foreach (Sitecore.Globalization.Language lang in 
        Sitecore.Data.Managers.LanguageManager.GetLanguages(item.Database))
      {
        Sitecore.Data.Items.Item ver = item.Database.GetItem(item.ID, lang);

        if (ver != null && ver.Versions.Count > 0)
        {
          sb.Append(this.GetMarkup(item, lang) + this.Separator);
        }
      }

      if (sb.Length > 0)
      {
        sb.Length = sb.Length - this.Separator.Length;
      }

      output.Write(sb.ToString());
    }

    private string GetMarkup(Sitecore.Data.Items.Item item, Sitecore.Globalization.Language lang)
    {
      if (lang != item.Language)
      {
        Sitecore.Links.UrlOptions opts = Sitecore.Links.LinkManager.GetDefaultUrlOptions();
        opts.Language = lang;
        return String.Format(
          @"<a href=""{0}""><img border=""0"" src=""{1}"" alt=""{2}"" /></a>",
          Sitecore.Links.LinkManager.GetItemUrl(item, opts),
          Sitecore.Resources.Images.GetThemedImageSource(lang.GetIcon(item.Database)),
          lang.GetDisplayName());
      }
      else
      {
        return String.Format(
          @"<img src=""{0}"" alt=""{1}"" />",
          Sitecore.Resources.Images.GetThemedImageSource(lang.GetIcon(item.Database)),
          lang.GetDisplayName());
      }
    }
  }
}