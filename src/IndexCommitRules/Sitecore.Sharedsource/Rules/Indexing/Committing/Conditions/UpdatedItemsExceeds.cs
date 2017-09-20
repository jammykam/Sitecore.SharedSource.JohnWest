namespace Sitecore.Sharedsource.Rules.Indexing.Committing.Conditions
{
  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to determine whether the number of items updated
  /// since the last commit of changes to the search index to disk exceeds
  /// a given threshold.
  /// </summary>
  /// <typeparam name="T">Rule context type.</typeparam>
  public class UpdatedItemsExceeds<T> :
    SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext
  {
    /// <summary>
    /// Gets or sets the threshold.
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// Rules engine condition to determine whether the number of items updated
    /// since the last commit of changes to the search index to disk exceeds
    /// a given threshold.
    /// </summary>
    /// <param name="ruleContext">Rule processing context.</param>
    /// <returns>True if the number of indexed items updated since the last commit 
    /// exceeds the given threshold.</returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.IsTrue(this.Threshold > 0, "Threshold: " + this.Threshold);
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      bool result = ruleContext.UpdatesSinceLastCommit > this.Threshold;
      return result;
    }
  }
}
