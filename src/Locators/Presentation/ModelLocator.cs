namespace Sitecore.Sharedsource.Mvc.Presentation
{
  using System;

  using SC = Sitecore;

  public class ModelLocator : SC.Mvc.Presentation.ModelLocator
  {
    public ModelLocator(SC.Mvc.Presentation.ModelLocator innerLocator)
    {
      this.InnerLocator = innerLocator;
    }

    public SC.Mvc.Presentation.ModelLocator InnerLocator { get; set; }

    // throwIfNotFound only applies if model contains a vlaue
    public override object GetModel(string model, bool throwIfNotFound)
    {
      object obj = this.InnerLocator.GetModel(model, throwIfNotFound);
      string message = String.Format(
        "{0} : GetModel(model[{1}], throwIfNotFound[{2}]) : {3}",
        this,
        model,
        throwIfNotFound,
        obj != null ? obj.GetType().ToString() : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return obj;
    }
  }
}