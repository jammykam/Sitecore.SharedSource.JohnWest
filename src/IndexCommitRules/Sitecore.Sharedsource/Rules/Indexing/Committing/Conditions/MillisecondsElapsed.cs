namespace Sitecore.Sharedsource.Rules.Indexing.Committing.Conditions
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  /// <summary>
  /// Rules engine condition to determine 
  /// whether a given number of milliseconds has elapsed
  /// since the last commit of changes to the search index to disk.
  /// </summary>
  /// <typeparam name="T">Rule context type.</typeparam>
  public class MillisecondsSinceCommit<T> :
    SC.Rules.Conditions.OperatorCondition<T>
    where T : SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext
  {
    /// <summary>
    /// Gets or sets the threshold in milliseconds.
    /// </summary>
    public int Threshold { get; set; }

    /// <summary>
    /// Rules engine condition to determine whether a given number
    /// of milliseconds has elapsed since the last commit of changes
    /// to a search engine to disk.
    /// </summary>
    /// <param name="ruleContext">Rule context type.</param>
    /// <returns>True if the number of milliseconds has updates </returns>
    protected override bool Execute(T ruleContext)
    {
      Assert.IsTrue(this.Threshold > 0, "Threshold: " + this.Threshold);
      Assert.ArgumentNotNull(ruleContext, "ruleContext");
      bool result = DateTime.Now.CompareTo(ruleContext.LastCommitted.AddMilliseconds(this.Threshold)) > 0;
      return result;
    }
  }
}
