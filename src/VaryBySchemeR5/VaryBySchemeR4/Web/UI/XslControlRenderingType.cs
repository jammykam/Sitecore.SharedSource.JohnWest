using System;
using System.Collections.Specialized;
using System.Web.UI;

namespace Sitecore.Sharedsource.Web.UI
{
  public class XslControlRenderingType : Sitecore.Web.UI.XslControlRenderingType
  {
    public override Control GetControl(
      NameValueCollection parameters, bool assert)
    {
      Sitecore.Sharedsource.Web.UI.WebControls.XslFile xsl =
        new Sitecore.Sharedsource.Web.UI.WebControls.XslFile();

      foreach (string str in parameters.Keys)
      {
        Sitecore.Reflection.ReflectionUtil.SetProperty(xsl, str, parameters[str]);
      }

      return xsl;
    }
  }
}