namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using System.Collections.Generic;
  using System.Web.UI;

  public class NewsArchive : Sitecore.Web.UI.WebControl
  {
    public NewsArchive()
    {
      this.DateField = "__created";
      this.HeadlineField = "Title";
    }

    public string DateField
    {
      get; 
      set;
    }

    public string HeadlineField
    {
      get; 
      set;
    }

    protected override string GetCachingID()
    {
      return this.GetType().FullName;
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      Sitecore.Data.Items.Item root = this.GetItem();
      Sitecore.Diagnostics.Assert.IsNotNull(root, "root");
      output.Write("Root: " + root.Paths.FullPath);
      List<Sitecore.Data.Items.Item> list = new List<Sitecore.Data.Items.Item>();
      Sitecore.Data.Items.Item[] years = root.Children.ToArray();
      string selectedYear = Sitecore.Web.WebUtil.GetQueryString("year", String.Empty);

      if (!String.IsNullOrEmpty(selectedYear) && root.Children[selectedYear] != null)
      {
        years = new[] { root.Children[selectedYear] };
      }

      foreach (Sitecore.Data.Items.Item year in years)
      {
        output.Write("<h3>");

        if (year.Name != selectedYear)
        {
          output.Write(@"<a href=""?year=" + year.Name + @""">");
        }

        output.Write(year.Name);

        if (year.Name != selectedYear)
        {
          output.Write("</a>");
        }

        output.Write("</h3>");

        foreach (Sitecore.Data.Items.Item month in year.Children)
        {
          output.Write("<strong>" + month.Name + "</strong><ul>");

          foreach (Sitecore.Data.Items.Item day in month.Children)
          {
            foreach (Sitecore.Data.Items.Item article in day.Children)
            {
              if (article.Fields[this.DateField] != null)
              {
                DateTime when = Sitecore.DateUtil.IsoDateToDateTime(article[this.DateField]);
                string markup = String.Format(
                  @"<li>{0} : <a href=""{1}"">{2}</a></li>",
                  when.ToShortDateString() + " " + when.ToShortTimeString(),
                  Sitecore.Links.LinkManager.GetItemUrl(article),
                  article.Fields[this.HeadlineField]);
                output.Write(markup);
              }
            }
          }

          output.Write("</ul>");
        }
      }
    }
  }
}

