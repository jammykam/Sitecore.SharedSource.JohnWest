namespace Sitecore.Sharedsource.Rules.Conditions
{
  public class ContextItemHasLayoutDetailsForThisDevice<T> : 
    Sitecore.Rules.Conditions.OperatorCondition<T>
    where T : Sitecore.Sharedsource.Rules.SetContextDeviceRuleContext
  {
    protected override bool Execute(T ruleContext)
    {
      return ruleContext.ContextItem.Visualization.GetLayout(
        ruleContext.EvaluateDevice) != null;
    }
  }
}