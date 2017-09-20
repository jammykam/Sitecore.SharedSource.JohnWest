namespace Sitecore.Sharedsource.Rules.Indexing.Conditions
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to determine if the name of an index
  /// matches a specific value.
  /// </summary>
  /// <typeparam name="T">Rules processing context.</typeparam>
  public class IndexNameMatches<T> : SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext
  {
    /// <summary>
    /// Gets or sets a value to match against the index name.
    /// </summary>
    public string MatchIndexNameAgainst { get; set; }

    /// <summary>
    /// Rules engine condition implementation to determine if the name of an index
    /// contains a specific value.
    /// </summary>
    /// <param name="ruleContext">Rules processing context.</param>
    /// <returns>Returns true if the name of the index matches the specified value.</returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      Assert.ArgumentNotNull(ruleContext.IndexName, "ruleContext.IndexName)");
      Assert.ArgumentNotNull(this.MatchIndexNameAgainst, "MatchIndexNameAgainst");
      return string.Equals(
        ruleContext.IndexName,
        this.MatchIndexNameAgainst,
        StringComparison.InvariantCultureIgnoreCase);
    }
  }
}