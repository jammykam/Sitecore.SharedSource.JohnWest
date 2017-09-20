namespace Sitecore.Sharedsource.Rules.Indexing.Actions
{
  using SC = Sitecore;

  /// <summary>
  /// Do not process additional rules for the current rule processing context.
  /// </summary>
  /// <typeparam name="T">Rules processing context.</typeparam>
  public class SkipAdditionalRules<T> : SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    /// <summary>
    /// Rules engine action to prevent processing of additional rules
    /// for the current context.
    /// </summary>
    /// <param name="ruleContext">Rules processing context.</param>
    public override void Apply(T ruleContext)
    {
      ruleContext.Abort();
    }
  }
}