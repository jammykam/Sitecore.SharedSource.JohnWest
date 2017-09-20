namespace Sitecore.Sharedsource.Rules.Indexing.Conditions
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to determine if the name of the 
  /// database associated with an index matches a specific value.
  /// </summary>
  /// <typeparam name="T">Rules processing context.</typeparam>
  public class IndexedDatabaseNameMatches<T> : SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext
  {
    /// <summary>
    /// Gets a value to match against the indexed database name.
    /// </summary>
    public string MatchIndexedDatabaseNameAgainst { get; private set; }

    /// <summary>
    /// Rules engine condition implementation to determine 
    /// if the name of the database associated with an index
    /// matches a specific value.
    /// </summary>
    /// <param name="ruleContext">Rules processing context.</param>
    /// <returns>Returns true if the name of the database associated with the index
    /// matches the specified value.</returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      Assert.ArgumentNotNull(ruleContext.IndexedDatabaseName, "ruleContext.IndexedDatabaseName)");
      Assert.ArgumentNotNullOrEmpty(this.MatchIndexedDatabaseNameAgainst, "MatchIndexedDatabaseNameAgainst");
      return string.Equals(
        ruleContext.IndexedDatabaseName,
        this.MatchIndexedDatabaseNameAgainst,
        StringComparison.InvariantCultureIgnoreCase);
    }
  }
}