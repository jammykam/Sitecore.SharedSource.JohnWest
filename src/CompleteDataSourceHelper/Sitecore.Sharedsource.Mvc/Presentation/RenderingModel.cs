namespace Sitecore.Sharedsource.Mvc.Presentation 
{
  using SC = Sitecore;

  public class RenderingModel : Sitecore.Mvc.Presentation.RenderingModel
  {
    private SC.Sharedsource.Web.UI.DataSourceHelper _dataSource;

    public SC.Sharedsource.Web.UI.DataSourceHelper DataSource
    {
      get
      {
        if (this._dataSource == null)
        {
          this._dataSource = new SC.Sharedsource.Web.UI.DataSourceHelper(
            this.Rendering.DataSource, 
            this.Item ?? this.PageItem);
        }

        return this._dataSource;
      }
    }
  }
}