namespace Sitecore.Sharedsource.Data
{
  using System.Collections;
  using System.Xml;

  public interface IExample
  {
    string StringProperty { get; set; }
    int IntProperty { get; set; }
    ArrayList PublicList { get; }

    void AddToProtectedList(string value);
    void ArbitraryMethod(XmlNode config);
  }
}