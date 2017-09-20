namespace Sitecore.Sharedsource.Web.UI.Layouts
{
  using System.Web;

  using SC = Sitecore;

  using System;
  using System.Web.UI;

  public partial class DMSCSV : System.Web.UI.Page
  {
    private HtmlTextWriter _output;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected override void Render(HtmlTextWriter writer)
    {
      if (!Sitecore.Context.User.IsAdministrator)
      {
        this.message.Text = "Not administrator.";
      }
      else if (this.IsPostBack
        && !string.IsNullOrEmpty(this.query.Text))
      {
        try
        {
          this._output = writer;
          SC.Analytics.Data.DataAccess.DataAdapters.DataAdapterManager.Sql.ReadMany(
            query.Text,
            this.RenderCSV,
            new object[0]);

          if (this.csv.Checked)
          {
            this.Response.ContentType = "text/csv";
            this.Response.AddHeader("Content-Disposition", "inline; filename=DMSCSV.csv");
            return;
          }
        }
        catch (Exception ex)
        {
          this.message.Text = ex.Message;
        }
      }

      base.Render(writer);
    }

    private void RenderCSV(
      Sitecore.Data.DataProviders.Sql.DataProviderReader reader)
    {
      this.message.Text = "<table border=\"1\">";

      while (reader.Read())
      {
        this.message.Text += "<tr>";
        int i = 0;

        while (true)
        {
          if (i != 0)
          {
            if (this.csv.Checked)
            {
              this._output.Write(",");
            }
          }

          string value;

          try
          {
            value = SC.Analytics.Data.DataAccess.DataAdapters.DataAdapterManager.Sql.GetString(
              i, 
              reader);
          }
          catch (InvalidCastException)
          {
            value = SC.Analytics.Data.DataAccess.DataAdapters.DataAdapterManager.Sql.GetInt(
              i, 
              reader).ToString();
          }
          catch (IndexOutOfRangeException)
          {
            break;
          }

          this.message.Text += "<td>" + HttpUtility.HtmlEncode(value) + "</td>";

          if (this.csv.Checked)
          {
            if (value.IndexOf('\"') > -1)
            {
              value = '"' + value.Replace("\"", "\\\"") + '"';
            }

            this._output.Write(value);
          }

          i++;
        }

        this.message.Text += "</tr>";

        if (this.csv.Checked)
        {
          this._output.Write(Environment.NewLine);
        }
      }

      this.message.Text += "</table>";
    }
  }
}