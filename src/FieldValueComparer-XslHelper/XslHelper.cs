namespace Sitecore.Sharedsource.Xml.Xsl
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using System.Xml.XPath;
  using Sitecore.Data.Items;
  using Sitecore.Sharedsource.Data.Comparers.ItemComparers;

  public class XslHelper : Sitecore.Xml.Xsl.XslHelper
  {
    public string AddDays(string date, int days)
    {
      DateTime when = Sitecore.DateUtil.IsoDateToDateTime(date);
      return Sitecore.DateUtil.ToIsoDate(when.AddDays(days));
    }

    public virtual XPathNodeIterator SortListField(
      string listField,
      string sortField, 
      XPathNodeIterator ni)
    {
      return this.SortListField(listField, sortField, ni, false);
    }

    public virtual XPathNodeIterator SortListField(
      string listField,
      string sortField, 
      XPathNodeIterator ni, 
      bool reverse)
    {
      Sitecore.Xml.Packet packet = new Sitecore.Xml.Packet("values");
      XPathNodeIterator iterator = ni.Clone();

      if (iterator.MoveNext())
      {
        Sitecore.Data.Items.Item item = GetItem(iterator);

        if (item != null)
        {
          Sitecore.Data.Fields.MultilistField fieldList = item.Fields[listField];

          if (fieldList != null)
          {
            List<Item> itemList = new List<Item>();

            foreach (Sitecore.Data.Items.Item pointer in fieldList.GetItems())
            {
              if (pointer != null)
              {
                itemList.Add(pointer);
              }
            }

            Item[] items = itemList.ToArray();
            Array.Sort(items, new FieldValueComparer(sortField));

            if (reverse)
            {
              Array.Reverse(items);
            }

            foreach (Sitecore.Data.Items.Item reference in items)
            {
              packet.AddElement("value", reference.ID.ToString());
            }
          }
        }
      }

      XPathNavigator navigator = packet.XmlDocument.CreateNavigator()
        ?? new XmlDocument().CreateNavigator();
      navigator.MoveToRoot();
      navigator.MoveToFirstChild();
      return navigator.SelectChildren(XPathNodeType.Element);
    }
  }
}