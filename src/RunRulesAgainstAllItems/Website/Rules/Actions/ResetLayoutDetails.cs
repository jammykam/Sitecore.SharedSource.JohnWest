namespace Sitecore.Sharedsource.Rules.Actions
{
  using SC = Sitecore;

  // rules engine action to reset layout details to their standard value
  public class ResetLayoutDetails<T> :
    SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    public override void Apply(T ruleContext)
    {
      SC.Diagnostics.Assert.IsNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.IsNotNull(ruleContext.Item, "ruleContext.Item");
      SC.Data.Fields.Field field =
        ruleContext.Item.Fields[Sitecore.FieldIDs.LayoutField];

      if (!field.ContainsStandardValue)
      {
        using (new Sitecore.Data.Items.EditContext(ruleContext.Item))
        {
          field.Reset();
        }
      }
    }
  }
}