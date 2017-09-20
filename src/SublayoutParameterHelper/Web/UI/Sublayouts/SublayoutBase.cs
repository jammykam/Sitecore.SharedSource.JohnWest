namespace Sitecore.Sharedsource.Web.UI.Sublayouts
{
  using SC = Sitecore;

  public abstract class SublayoutBase : System.Web.UI.UserControl
  {
    private SC.Sharedsource.Web.UI.Sublayouts.SublayoutParameterHelper _dataSourceHelper;

    public SC.Sharedsource.Web.UI.Sublayouts.SublayoutParameterHelper ParameterHelper { get; private set; }

    protected override void OnInit(System.EventArgs e)
    {
      // it could be slightly more efficient to
      // eliminate this method and 
      // make the DataSourceHelper property lazy-load.
      // But we can't be sure every sublayout will
      // always access that property and we want to set 
      // properties now.
      this.ParameterHelper =
        new Sitecore.Sharedsource.Web.UI.Sublayouts.SublayoutParameterHelper(this, true /*applyProperties*/);
    }
  }
}