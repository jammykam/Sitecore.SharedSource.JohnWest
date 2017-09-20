namespace Sitecore.Sharedsource.Web.UI
{
  using System.Collections.Specialized;

  using SC = Sitecore;

  public class XslControlRenderingType : SC.Web.UI.XslControlRenderingType
  {
    public override System.Web.UI.Control GetControl(NameValueCollection parameters, bool assert)
    {
      SC.Sharedsource.Web.UI.WebControls.XslFile xslFile = 
        new SC.Sharedsource.Web.UI.WebControls.XslFile();

      foreach (string key in parameters.Keys)
      {
        string value = parameters[key];
        SC.Reflection.ReflectionUtil.SetProperty(xslFile, key, value);
      }

      return xslFile;
    }
  }
}