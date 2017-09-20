namespace Sitecore.Sharedsource.Web.UI.Sublayouts
{
  using System;
  using System.Collections.Specialized;
  using System.Web.UI;

  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class SublayoutParameterHelper
  {
    public SC.Sharedsource.Web.UI.DataSourceHelper DataSource;

    public SublayoutParameterHelper(
      UserControl control,
      bool applyProperties)
    {
      this.BindingControl = control.Parent as SC.Web.UI.WebControls.Sublayout;

      if (this.BindingControl == null)
      {
        return;
      }

      this.DataSource = new SC.Sharedsource.Web.UI.DataSourceHelper(
        this.BindingControl.DataSource,
        SC.Context.Item)
      {
        StoreUISearchResults = true,
      };

      this.Parameters = SC.Web.WebUtil.ParseUrlParameters(
        this.BindingControl.Parameters);

      if (applyProperties)
      {
        this.ApplyProperties(control);
      }
    }

    protected NameValueCollection Parameters
    {
      get;
      private set;
    }

    protected SC.Web.UI.WebControls.Sublayout BindingControl
    {
      get;
      private set;
    }

    public string GetParameter(string key)
    {
      if (this.Parameters == null)
      {
        return string.Empty;
      }

      string result = this.Parameters[key];

      if (String.IsNullOrEmpty(result))
      {
        return string.Empty;
      }

      return result;
    }

    protected void ApplyProperties(UserControl control)
    {
      foreach (string key in this.Parameters.Keys)
      {
        SC.Reflection.ReflectionUtil.SetProperty(
          control,
          key,
          this.Parameters[key]);
      }
    }
  }
}
