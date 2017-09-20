namespace Sitecore.Sharedsource.Mvc.Pipelines.Response.GetRenderer
{
  using SC = Sitecore;

  public class GetXsltRenderer : 
    SC.Mvc.Pipelines.Response.GetRenderer.GetXsltRenderer
  {
    protected override SC.Mvc.Presentation.Renderer GetRenderer(
      SC.Mvc.Presentation.Rendering rendering, 
      SC.Mvc.Pipelines.Response.GetRenderer.GetRendererArgs args)
    {
      string xsltPath = this.GetXsltPath(rendering, args);

      if (string.IsNullOrEmpty(xsltPath))
      {
          return null;
      }

      return new SC.Sharedsource.Mvc.Presentation.XsltRenderer
      {
        Path = xsltPath, 
        Rendering = rendering
      };
    }
  }
}