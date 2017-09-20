namespace Sitecore.Sharedsource.layouts
{
  using System;
  using System.Collections.Generic;
  using System.Web.UI;

  public partial class Query : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(this.txtQuery.Text))
      {
        this.txtQuery.Text = 
          "/*/content//*[tolower(@Title) = 'sitecore' and versioncount(.) > 0]";
        return;
      }

      Sitecore.Sharedsource.Data.Query.Query query =
        new Sitecore.Sharedsource.Data.Query.Query(this.txtQuery.Text);
      Sitecore.Diagnostics.Assert.IsNotNull(query, "query");
      object results = query.Execute();

      Sitecore.Data.Query.QueryContext context = 
        results as Sitecore.Data.Query.QueryContext;
      Sitecore.Data.Query.QueryContext[] contextArray = results 
        as Sitecore.Data.Query.QueryContext[];
      List<Sitecore.Data.Items.Item> items = new List<Sitecore.Data.Items.Item>();

      if (context != null)
      {
        items.Add(Sitecore.Context.Database.Items[context.ID]);
      }
      else if (contextArray != null)
      {
        for (int i = 0; i < contextArray.Length; i++)
        {
          items.Add(Sitecore.Context.Database.Items[contextArray[i].ID]);
        }
      }

      foreach(Sitecore.Data.Items.Item item in items)
      {
        this.Controls.Add(new LiteralControl(item.Paths.FullPath 
          + " : " 
          + item.Language.Name 
          + " : " 
          + item.Versions.Count + "<br />"));
      }
    }
  }
}