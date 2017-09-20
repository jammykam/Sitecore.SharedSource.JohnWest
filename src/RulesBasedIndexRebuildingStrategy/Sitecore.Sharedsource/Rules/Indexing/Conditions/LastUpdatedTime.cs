namespace Sitecore.Sharedsource.Rules.Indexing.Conditions
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to determine whether a search index
  /// has been updated recently (within the specified number
  /// of minutes).
  /// </summary>
  /// <typeparam name="T">Rules processing context.</typeparam>
  public class LastUpdatedTime<T> : SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext
  {
    /// <summary>
    /// Gets or sets the number of minutes for comparison.
    /// </summary>
    public int? CompareLastUpdatedAgainstMinutes { get; set; }

    /// <summary>
    /// Rules engine condition implementation to determine whether a search index
    /// has been updated within the specified number of minutes.
    /// </summary>
    /// <param name="ruleContext">Rules processing context.</param>
    /// <returns>True if the index has not been updated 
    /// within the specified number of minutes.</returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      Assert.ArgumentNotNull(this.CompareLastUpdatedAgainstMinutes, "CompareLastUpdatedAgainstMinutes");
      return DateTime.Compare(
        ruleContext.LastUpdated.AddMinutes(this.CompareLastUpdatedAgainstMinutes.Value),
        DateTime.Now) < 0;
    }
  }
}