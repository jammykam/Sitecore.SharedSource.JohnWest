using Sitecore.Data.Items;

namespace Sitecore.Sharedsource.Rules.Actions 
{
  public class SetContextDeviceToThisDevice<T> : 
    Sitecore.Rules.Actions.RuleAction<T>
    where T : Sitecore.Sharedsource.Rules.SetContextDeviceRuleContext
  {
    public override void Apply(T ruleContext)
    {
      ruleContext.ContextDevice = ruleContext.EvaluateDevice;
    }
  }
}