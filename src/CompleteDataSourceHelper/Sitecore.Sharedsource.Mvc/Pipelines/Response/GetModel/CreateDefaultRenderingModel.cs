namespace Sitecore.Sharedsource.Mvc.Pipelines.Response.GetModel
{
  using SC = Sitecore;

  public class CreateDefaultRenderingModel : 
    SC.Mvc.Pipelines.Response.GetModel.CreateDefaultRenderingModel
  {
    protected override object CreateDefaultModel(
      SC.Mvc.Presentation.Rendering rendering, 
      SC.Mvc.Pipelines.Response.GetModel.GetModelArgs args)
    {
      return new SC.Sharedsource.Mvc.Presentation.RenderingModel();
    }
  }
}