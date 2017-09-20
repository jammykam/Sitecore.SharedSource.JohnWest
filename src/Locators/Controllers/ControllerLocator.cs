namespace Sitecore.Sharedsource.Mvc.Controllers
{
  using System;
  using System.Web.Mvc;

  using SC = Sitecore;

  public class ControllerLocator : SC.Mvc.Controllers.ControllerLocator
  {
    public ControllerLocator(SC.Mvc.Controllers.ControllerLocator innerLocator)
    {
      this.InnerLocator = innerLocator;
    }

    public SC.Mvc.Controllers.ControllerLocator InnerLocator { get; set; }

    public override IController GetController(
      string controllerName, 
      string actionName)
    {
      IController controller = this.InnerLocator.GetController(
        controllerName, 
        actionName);
      string message = String.Format(
        "{0} : IController GetController(controllerName[{1}], actionName[{2}] : {3}",
        this,
        controllerName,
        actionName,
        controller != null ? controller.GetType().ToString() : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return controller;
    }

    public override Tuple<string, string> GetControllerAndAction(
      string controllerName, 
      string actionName)
    {
      Tuple<string, string> tuple = this.InnerLocator.GetControllerAndAction(
        controllerName, 
        actionName);
      string message = String.Format(
        "{0} : GetControllerAndAction(controllerName[{1}], actionName[{2}] : {3}.{4}",
        this,
        controllerName,
        actionName,
        tuple != null && tuple.Item1 != null ? tuple.Item1.ToString() : "null",
        tuple != null && tuple.Item2 != null ? tuple.Item2.ToString() : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return tuple;
    }

    public override SC.Mvc.Controllers.ControllerBuilder GetControllerBuilder(
      string controllerName, 
      string actionName)
    {
      SC.Mvc.Controllers.ControllerBuilder controllerBuilder = this.InnerLocator.GetControllerBuilder(
        controllerName, 
        actionName);
      string message = String.Format(
        "{0} : GetControllerBuilder(controllerName[{1}], actionName[{2}] : {3}",
        this,
        controllerName,
        actionName,
        controllerBuilder != null ? controllerBuilder.GetType().ToString() : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return controllerBuilder;
    }
  }
}