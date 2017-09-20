namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System.Xml.Xsl;

  using SC = Sitecore;
  
  public class XslFile : SC.Web.UI.WebControls.XslFile
  {
    protected override void AddParameters(
      XsltArgumentList list, 
      SC.Data.Items.Item item)
    {
      string[] keys = new string[Attributes.Keys.Count];
      this.Attributes.Keys.CopyTo(keys, 0);

      foreach (string key in keys)
      {
        // remove custom namespaces 
        if (key.Contains(":"))
        {
          this.Attributes.Remove(key);
        }
      }

      base.AddParameters(list, item);
      list.AddParam("sc_datasource", string.Empty, this.DataSource);
    }

    protected override void DoRender(System.Web.UI.HtmlTextWriter output, SC.Data.Items.Item item)
    {
      // without this, Sitecore won't run the rendering
      if (item == null)
      {
        item = SC.Context.Item;
      }

      base.DoRender(output, item);
    }
  }
}