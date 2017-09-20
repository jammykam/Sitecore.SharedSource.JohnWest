namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System.Web.UI;
  using System.Xml;

  public class ExampleTester : Sitecore.Web.UI.WebControl
  {
    protected override void DoRender(HtmlTextWriter output)
    {
      string path = "exampleSection/exampleType";
      XmlNode config = Sitecore.Configuration.Factory.GetConfigNode(path);
      Sitecore.Diagnostics.Assert.IsNotNull(config, path);
      Sitecore.Sharedsource.Data.IExample example =
        Sitecore.Configuration.Factory.CreateObject<Sitecore.Sharedsource.Data.IExample>(config);
      XmlDocument doc = new XmlDocument();
      doc.LoadXml("<hard-coded>Value hard-coded in " + this + "</hard-coded>");
      example.ArbitraryMethod(doc);
    }
  }
}