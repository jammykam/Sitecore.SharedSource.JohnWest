namespace Sitecore.Sharedsource.Rules.Indexing.Committing.Actions
{
  using SC = Sitecore;

  /// <summary>
  /// Rules engine action to indicate that the commit policy executor should 
  /// not commit changesor optimize the index.
  /// </summary>
  /// <typeparam name="T">Rule processing context type.</typeparam>
  public class DoNotCommit<T> : SC.Rules.Actions.RuleAction<T>
    where T : SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext
  {
    /// <summary>
    /// Rules engine action to indicate that the commit policy executor should 
    /// not commit changes, or optimize the index.
    /// </summary>
    /// <param name="ruleContext">Rule processing context.</param>
    public override void Apply(T ruleContext)
    {
      ruleContext.ShouldCommit = false;
      ruleContext.ShouldOptimize = false;
      ruleContext.Abort();
    }
  }
}