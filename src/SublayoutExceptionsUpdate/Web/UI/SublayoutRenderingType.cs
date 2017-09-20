using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace Sitecore.Sharedsource.Web.UI
{
  public class SublayoutRenderingType : Sitecore.Web.UI.SublayoutRenderingType
  {
    public override Control GetControl(
      NameValueCollection parameters, bool assert)
    {
      Sitecore.Sharedsource.Web.UI.WebControls.Sublayout sublayout = 
        new Sitecore.Sharedsource.Web.UI.WebControls.Sublayout();

      foreach (string str in parameters.Keys)
      {
        Sitecore.Reflection.ReflectionUtil.SetProperty(sublayout, str, parameters[str]);
      }

      return sublayout;
    }
  }
}