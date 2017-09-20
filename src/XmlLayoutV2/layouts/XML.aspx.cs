namespace Sitecore.Sharedsource.layouts
{
  using System;
  using System.Web.UI;
  using System.Xml;

  public partial class XML : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Response.ContentType = "text/xml";
    }

    protected override void Render(HtmlTextWriter writer)
    {
      Sitecore.Data.Items.Item root = Sitecore.Context.Item;
      Sitecore.Diagnostics.Assert.ArgumentNotNull(Sitecore.Context.Item, "Sitecore.Context.Item");
      string strRoot = Sitecore.Web.WebUtil.GetQueryString("root");

      if (!String.IsNullOrEmpty(strRoot))
      {
        root = root.Database.GetItem(strRoot);
        Sitecore.Diagnostics.Assert.IsNotNull(root, strRoot);
      }

      string strDeep = Sitecore.Web.WebUtil.GetQueryString("deep").ToLower();
      bool deep = (!String.IsNullOrEmpty(strDeep)) && !(strDeep.Equals("false") || strDeep.Equals("0") || strDeep.Equals("no"));
      string strXsl = Sitecore.Web.WebUtil.GetQueryString("xsl").ToLower();

      if (String.IsNullOrEmpty(strXsl) || strXsl == "false" || strXsl == "0" || strXsl == "no")
      {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(root.GetOuterXml(deep));
        writer.WriteLine(doc.OuterXml);
      }
      else
      {
        //NOTE: XSL format is always deep
        writer.WriteLine(Sitecore.Configuration.Factory.CreateItemNavigator(root).OuterXml);
      }

      base.Render(writer);
    }
  }
}