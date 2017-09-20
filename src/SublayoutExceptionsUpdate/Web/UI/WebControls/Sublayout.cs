namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using System.IO;
  using System.Text;
  using System.Web.UI;

  public class Sublayout : Sitecore.Web.UI.WebControls.Sublayout
  {
    public bool DisableErrorManagement
    {
      get; 
      set; 
    }

    protected override void DoRender(System.Web.UI.HtmlTextWriter output)
    {
      if (this.DisableErrorManagement)
      {
        base.Render(output);
        return;
      }

      StringBuilder sb = new StringBuilder();
      HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb));

      try
      {
        base.DoRender(writer);
        writer.Close();
        output.Write(sb.ToString());
      }
      catch (Exception ex)
      {
        Sitecore.Diagnostics.Log.Error(
          this + " : Exception in sublayout " + this.Path + " processing " + Sitecore.Context.RawUrl, 
          ex, 
          this);

        if (ErrorHelper.ShouldRedirect())
        {
          ErrorHelper.Redirect();
        }
        else
        {
          this.RenderError("Could not process sublayout", this.Path, ex.ToString(), output);
          output.WriteLine(this + " : " + this.Cacheable + " : " + this.GetCacheKey());
        }
      }
    }
  }
}