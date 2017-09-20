namespace Sitecore.Sharedsource.Rules.Conditions
{
  using System;

  using SC = Sitecore;

  [UsedImplicitly]
  public class DuplicateItemKey<T> : Sitecore.Rules.Conditions.WhenCondition<T>
    where T : Sitecore.Rules.RuleContext
  {
    protected override bool Execute(T ruleContext)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.ArgumentNotNull(
        ruleContext.Item,
        "ruleContext.Item");

      string query = String.Format(
        "../*[@@key='{0}' and @@id!='{1}']",
        ruleContext.Item.Key,
        ruleContext.Item.ID);
      return ruleContext.Item.Axes.SelectSingleItem(query) != null;
    }
  }
}