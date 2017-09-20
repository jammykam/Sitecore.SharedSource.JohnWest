namespace Sitecore.Sharedsource.Rules.Indexing.Committing
{
  using System;

  using SC = Sitecore;

  /// <summary>
  /// Rules processing context for use with rules that determine
  /// whether to commit changes to the index to disk.
  /// </summary>
  public class IndexCommittingRuleContext : SC.Rules.RuleContext
  {
    public IndexCommittingRuleContext(
      DateTime lastCommitted,
      int updatesSinceLastCommit)
    {
      this.LastCommitted = lastCommitted;
      this.UpdatesSinceLastCommit = updatesSinceLastCommit;
    }

    /// <summary>
    /// Gets or sets a value that indicates when changes to the index 
    /// were last committed to disk.
    /// </summary>
    public DateTime LastCommitted { get; private set; }

    /// <summary>
    /// Gets or sets a value that indicates the number of items updated
    /// in the index since its last commit to disk.
    /// </summary>
    public int UpdatesSinceLastCommit { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the index commit policy
    /// should commit changes to the index to disk.
    /// </summary>
    public bool ShouldCommit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the index commit policy
    /// should optimize the index.
    /// </summary>
    public bool ShouldOptimize { get; set; }
  }
}