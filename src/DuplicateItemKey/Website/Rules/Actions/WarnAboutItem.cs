namespace Sitecore.Sharedsource.Rules.Actions
{
  using System;

  using SC = Sitecore;

  // rules engine action to log information about the item
  public class WarnAboutItem<T> :
    SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    public string Message { get; set; }

    public override void Apply(T ruleContext)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.ArgumentNotNull(
        ruleContext.Item, 
        "ruleContext.Item");
      string message = String.Format(
        "{0} : {1} ({2}.{3})",
        this.Message,
        ruleContext.Item.Paths.FullPath,
        ruleContext.Item.Language.Name,
        ruleContext.Item.Version.Number);
      SC.Diagnostics.Log.Warn(message, this);
    }
  }
}