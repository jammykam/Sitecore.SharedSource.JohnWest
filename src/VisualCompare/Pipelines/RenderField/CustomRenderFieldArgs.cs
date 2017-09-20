namespace Sitecore.Sharedsource.Pipelines.RenderField
{
  using Sitecore.Pipelines.RenderField;

  // to avoid infinite recursion, this class indicates when 
  // a renderField processor calls the renderField pipeline

  public class CustomRenderFieldArgs : RenderFieldArgs
  {
  }
}