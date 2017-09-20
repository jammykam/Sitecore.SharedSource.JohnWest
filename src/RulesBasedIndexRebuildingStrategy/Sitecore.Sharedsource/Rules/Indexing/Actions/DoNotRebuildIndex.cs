namespace Sitecore.Sharedsource.Rules.Indexing.Actions
{
  using SC = Sitecore;

  /// <summary>
  /// Rules engine action to indicate that Sitecore 
  /// should not rebuild the index at this time.
  /// </summary>
  /// <typeparam name="T">Rules processing context.</typeparam>
  public class DoNotRebuild<T> : SC.Rules.Actions.RuleAction<T>
    where T : SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext
  {
    /// <summary>
    /// Rule action implementation to indicate that Sitecore
    /// should not rebuild the index at this time.
    /// </summary>
    /// <param name="ruleContext">Rules processing context.</param>
    public override void Apply(T ruleContext)
    {
      ruleContext.ShouldRebuild = false;
    }
  }
}