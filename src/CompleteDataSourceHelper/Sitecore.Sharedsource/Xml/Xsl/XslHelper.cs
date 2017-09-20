namespace Sitecore.Sharedsource.Xml.Xsl
{
  using System.Xml;
  using System.Xml.XPath;

  using SC = Sitecore;

  public class XslHelper : SC.Xml.Xsl.XslHelper
  {
    public virtual XPathNodeIterator GetDataSourceIDs(
      string dataSource, 
      XPathNodeIterator iterator)
    {
      if (string.IsNullOrEmpty(dataSource))
      {
        return this.CreateEmptyIterator();
      }

      SC.Xml.Packet packet = new SC.Xml.Packet("values", new string[0]);
      iterator.MoveNext();
      SC.Sharedsource.Web.UI.DataSourceHelper helper = 
        new SC.Sharedsource.Web.UI.DataSourceHelper(dataSource, this.GetItem(iterator))
        {
          StoreUISearchResults = true,
        };

      if (helper.Items != null)
      {
        foreach (SC.Data.Items.Item item in helper.Items)
        {
          if (item != null)
          {
            packet.AddElement("value", item.ID.ToString());
          }
        }
      }

      return this.GetChildIterator(packet);
    }

    private XPathNodeIterator GetChildIterator(SC.Xml.Packet packet)
    {
      XPathNavigator navigator = packet.XmlDocument.CreateNavigator();
      navigator.MoveToRoot();
      navigator.MoveToFirstChild();
      return navigator.SelectChildren(XPathNodeType.Element);
    }

    private XPathNodeIterator CreateEmptyIterator()
    {
      return new XmlDocument().CreateNavigator().Select("*");
    }
  }
}