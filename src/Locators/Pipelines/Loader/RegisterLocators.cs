namespace Sitecore.Sharedsource.Mvc.Pipelines.Loader
{
  using SC = Sitecore;

  public class RegisterLocators
  {
    public void Process(SC.Pipelines.PipelineArgs args)
    {
      this.RegisterItemLocator();
      this.RegisterModelLocator();
      this.RegisterControllerLocator();
    }

    internal void RegisterItemLocator()
    {
      SC.Mvc.Data.ItemLocator innerLocator = 
        SC.Mvc.Configuration.MvcSettings.GetRegisteredObject<SC.Mvc.Data.ItemLocator>(); 
      SC.Mvc.Configuration.MvcSettings.RegisterObject<SC.Mvc.Data.ItemLocator>(
        () => new SC.Sharedsource.Mvc.Data.ItemLocator(innerLocator)); 
    }

    internal void RegisterModelLocator()
    {
      SC.Mvc.Presentation.ModelLocator innerLocator =
        SC.Mvc.Configuration.MvcSettings.GetRegisteredObject<SC.Mvc.Presentation.ModelLocator>();
      SC.Mvc.Configuration.MvcSettings.RegisterObject<SC.Mvc.Presentation.ModelLocator>(
        () => new SC.Sharedsource.Mvc.Presentation.ModelLocator(innerLocator));
    }

    internal void RegisterControllerLocator()
    {
      SC.Mvc.Controllers.ControllerLocator innerLocator =
        SC.Mvc.Configuration.MvcSettings.GetRegisteredObject<SC.Mvc.Controllers.ControllerLocator>();
      SC.Mvc.Configuration.MvcSettings.RegisterObject<SC.Mvc.Controllers.ControllerLocator>(
        () => new SC.Sharedsource.Mvc.Controllers.ControllerLocator(innerLocator));
    }
  }
}