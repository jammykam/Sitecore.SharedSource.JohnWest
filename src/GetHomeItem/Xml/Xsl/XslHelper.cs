namespace Sitecore.Sharedsource.Xml.Xsl
{
  using System.Xml.XPath;
  using Sitecore.Sharedsource.Sites.SiteContext;

  public class XslHelper : Sitecore.Xml.Xsl.XslHelper
  {
    public XPathNodeIterator GetHomeItem()
    {
      Sitecore.Data.Items.Item home = Sitecore.Context.Site.GetHomeItem();
      Sitecore.Diagnostics.Assert.IsNotNull(home, "home");
      return this.GetIterator(home);
    }

    public XPathNodeIterator GetIterator(Sitecore.Data.Items.Item item)
    {
      Sitecore.Xml.XPath.ItemNavigator navigator =
        Sitecore.Configuration.Factory.CreateItemNavigator(item);
      Sitecore.Diagnostics.Assert.IsNotNull(navigator, "navigator");
      return navigator.Select(".");
    }
  }
}