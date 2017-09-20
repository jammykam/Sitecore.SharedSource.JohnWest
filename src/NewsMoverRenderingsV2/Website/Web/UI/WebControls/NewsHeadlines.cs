namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using System.Collections.Generic;
  using System.Web.UI;

  public class NewsHeadlines : Sitecore.Web.UI.WebControl
  {
    public NewsHeadlines()
    {
      this.DateField = "__created";
      this.HeadlineField = "Title";
      this.MaxHeadlines = 5;
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

    public int MaxHeadlines
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
      List<Sitecore.Data.Items.Item> list = new List<Sitecore.Data.Items.Item>();
      Sitecore.Collections.ChildList years = root.Children;

      for (int y = 0; y < years.Count && list.Count < this.MaxHeadlines; y++) 
      {
        Sitecore.Collections.ChildList months = years[y].Children;

        for (int m = 0; m < months.Count && list.Count < this.MaxHeadlines; m++)
        {
          Sitecore.Collections.ChildList days = months[m].Children;

          for (int d = 0; d < days.Count && list.Count < this.MaxHeadlines; d++)
          {
            Sitecore.Collections.ChildList articles = days[d].Children;

            for (int a = 0; a < articles.Count && list.Count < this.MaxHeadlines; a++)
            {
              if (articles[a].Fields[this.DateField] != null)
              {
                list.Add(articles[a]);
              }
            }
          }
        }
      }

      if (list.Count != 0)
      {
        output.Write("<ul>");

        foreach (Sitecore.Data.Items.Item item in list)
        {
          DateTime when = Sitecore.DateUtil.IsoDateToDateTime(item[this.DateField]);
          string date = when.ToShortDateString() + " " + when.ToShortTimeString();
          string markup = String.Format(
            @"<li>{0} : <a href=""{1}"">{2}</a></li>",
            date,
            Sitecore.Links.LinkManager.GetItemUrl(item),
            item[this.HeadlineField]);
          output.Write(markup);
        }

        output.Write("</ul>");
      }
    }
  }
}

