using System.Web.UI;

namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  public class HiddenErrorControl : Sitecore.Web.UI.WebControls.ErrorControl
  {
    public override Sitecore.Web.UI.WebControls.ErrorControl Clone()
    {
      return new HiddenErrorControl();
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      return;
    }
  }
}





