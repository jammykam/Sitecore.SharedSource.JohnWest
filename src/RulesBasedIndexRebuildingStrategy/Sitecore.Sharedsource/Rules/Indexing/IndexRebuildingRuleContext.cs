namespace Sitecore.Sharedsource.Rules.Indexing
{
  using System;

  using SC = Sitecore;

  /// <summary>
  /// Sitecore 7 index rebuilding strategy that invokes the rules engine
  /// to determine whether to rebuild an index.
  /// </summary>
  public class IndexRebuildingRuleContext : SC.Rules.RuleContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexRebuildingRuleContext"/> class. 
    /// Constructor requires ISearchIndex.
    /// </summary>
    /// <param name="index">
    /// The search index to investigate.
    /// </param>
    /// <param name="indexedDatabaseName">
    /// The name of the Sitecore database
    /// associated with that search index.
    /// </param>
    public IndexRebuildingRuleContext(
      SC.ContentSearch.ISearchIndex index,
      string indexedDatabaseName = null)
    {
      this.IndexName = index.Name;
      this.IndexedDatabaseName = indexedDatabaseName;
      this.LastUpdated = index.Summary.LastUpdated.ToLocalTime();
      SC.Diagnostics.Log.Info(this + " : LastUpdated : " + this.LastUpdated, this);
    }

    /// <summary>
    /// Gets or sets a value indicating whether rules determined
    /// that Sitecore should rebuild the index (used by rule actions).
    /// </summary>
    public bool ShouldRebuild { get; set; }

    /// <summary>
    /// Gets or sets the name of the index to investigate 
    /// (used by rule conditions).
    /// </summary>
    public string IndexName { get; set; }

    /// <summary>
    /// Gets the date/time when this index was last updated 
    /// (used by rule conditions).
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// Gets or sets the name of the Sitecore database associated with the index.
    /// </summary>
    public string IndexedDatabaseName { get; set; }
  }
}