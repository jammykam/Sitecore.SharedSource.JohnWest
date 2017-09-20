namespace Sitecore.Sharedsource.Mvc.Pipelines.Response.GetModel
{
  using System.Collections.Specialized;
  using System.Web;

  using SC = Sitecore;

  public class ApplyProperties : 
    SC.Mvc.Pipelines.Response.GetModel.GetModelProcessor
  {
    public override void Process(
      SC.Mvc.Pipelines.Response.GetModel.GetModelArgs args)
    {
      if (args.Result == null
        || SC.Mvc.Presentation.RenderingContext.Current == null
        || SC.Mvc.Presentation.RenderingContext.Current.Rendering == null
        || string.IsNullOrEmpty(
          SC.Mvc.Presentation.RenderingContext.Current.Rendering["Parameters"]))
      {
        return;
      }

      NameValueCollection parameters = SC.Web.WebUtil.ParseUrlParameters(
        SC.Mvc.Presentation.RenderingContext.Current.Rendering["Parameters"]);

      foreach (string key in parameters.Keys)
      {
        SC.Reflection.ReflectionUtil.SetProperty(
          args.Result,
          key,
          HttpUtility.UrlDecode(parameters[key]));
      }
    }
  }
}