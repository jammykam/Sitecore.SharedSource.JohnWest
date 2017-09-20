namespace Sitecore.Sharedsource.ContentSearch.Maintenance.Strategies
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;

  using SC = Sitecore;

  public class RuleEngineTriggeredIndexRebuild : 
    Sitecore.ContentSearch.Maintenance.Strategies.IIndexUpdateStrategy
  {
    /// <summary>
    /// Alarm clock triggers Handle() at specified interval
    /// </summary>
    private SC.Services.AlarmClock _alarmClock;

    /// <summary>
    /// The index for which to manage rebuilding
    /// (passed by Sitecore to the Initialize() method)
    /// </summary>
    private SC.ContentSearch.ISearchIndex _index;

    /// <summary>
    /// The database containing the rules to invoke
    /// to determine whether to rebuild
    /// </summary>
    private SC.Data.Database _ruleDatabase;

    /// <summary>
    ///  The name of the database associated with the index
    /// </summary>
    private string _indexedDatabaseName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineTriggeredIndexRebuild"/> class.
    /// </summary>
    public RuleEngineTriggeredIndexRebuild()
      : this(null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineTriggeredIndexRebuild"/> class.
    /// </summary>
    /// <param name="interval">
    /// The interval at which to apply the rules.
    /// </param>
    public RuleEngineTriggeredIndexRebuild(string interval)
      : this(interval, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineTriggeredIndexRebuild"/> class.
    /// </summary>
    /// <param name="interval">
    /// The interval at which to apply the rules.
    /// </param>
    /// <param name="ruleDatabaseName">
    /// The name of the Sitecore database containing the rules to apply.
    /// </param>
    public RuleEngineTriggeredIndexRebuild(string interval, string ruleDatabaseName)
      : this(interval, ruleDatabaseName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RuleEngineTriggeredIndexRebuild"/> class.
    /// </summary>
    /// <param name="interval">
    /// The interval at which to apply the rules.
    /// </param>
    /// <param name="ruleDatabaseName">
    /// The name of the Sitecore database containing the rules to apply.
    /// </param>
    /// <param name="indexedDatabaseName">The name of the database associated with the index.</param>
    public RuleEngineTriggeredIndexRebuild(
      string interval = null,
      string ruleDatabaseName = null, 
      string indexedDatabaseName = null)
    {
      if (ruleDatabaseName != null)
      {
        this._ruleDatabase = SC.Configuration.Factory.GetDatabase(ruleDatabaseName);
        SC.Diagnostics.Assert.IsNotNull(this._ruleDatabase, "_ruleDatabase : " + ruleDatabaseName);
      }

      this._indexedDatabaseName = indexedDatabaseName;
      this._alarmClock = new SC.Services.AlarmClock(
        SC.DateUtil.ParseTimeSpan(interval, SC.Configuration.Settings.Indexing.UpdateInterval));
    }

    /// <summary>
    /// Triggered by Sitecore to initialize the index rebuilding strategy.
    /// </summary>
    /// <param name="index">Index associated with the index rebuilding strategy.</param>
    public void Initialize(SC.ContentSearch.ISearchIndex index)
    {
      Assert.ArgumentNotNull(index, "index");

      if (this._ruleDatabase == null && !string.IsNullOrEmpty(index.Name))
      {
        string[] parts = index.Name.Split('_');

        if (parts.Length > 1)
        {
          this._ruleDatabase = SC.Configuration.Factory.GetDatabase(parts[1]);
        }
      }

      SC.Diagnostics.Assert.IsNotNull(this._ruleDatabase, "_ruleDatabase");

      if (string.IsNullOrEmpty(this._indexedDatabaseName))
      {
        this._indexedDatabaseName = this._ruleDatabase.Name;
      }

      SC.ContentSearch.Diagnostics.CrawlingLog.Log.Info(string.Format(
        "[Index={0}] Initializing {1} with interval '{2}'.", 
        index.Name, 
        this.GetType().Name,
        this._alarmClock.Interval));
      this._index = index;
      this._alarmClock.Ring += (sender, args) => this.Handle(args);
    }

    /// <summary>
    /// Triggered at interval to determine whether to rebuild the index.
    /// </summary>
    /// <param name="args">Event arguments (not used by this implementation)</param>
    internal void Handle(EventArgs args)
    {
      Assert.IsNotNull(this._index, "_index");
      string path = "/sitecore/system/Settings/Rules/Indexing/Rebuilding/Rules";

      using (new SC.SecurityModel.SecurityDisabler())
      {
        SC.Data.Items.Item ruleRoot = this._ruleDatabase.GetItem(path);
        SC.Diagnostics.Assert.IsNotNull(
          ruleRoot, 
          string.Format("ruleRoot item {0} in database {1}", path, this._ruleDatabase.Name));
        SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext ruleContext =
          new SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext(this._index)
          {
            IndexedDatabaseName = this._indexedDatabaseName,
          };
        
        SC.Rules.RuleList<SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext> ruleList =
          SC.Rules.RuleFactory.GetRules<SC.Sharedsource.Rules.Indexing.IndexRebuildingRuleContext>(ruleRoot.Axes.GetDescendants(), "Rule");
        Assert.IsNotNull(ruleList, "ruleList");
        Assert.IsTrue(
          ruleList.Count > 0, 
          "ruleList.Count < 1 for " + path + " in database " + this._ruleDatabase.Name);
        ruleList.Run(ruleContext);

        if (ruleContext.ShouldRebuild)
        {
          SC.ContentSearch.Diagnostics.CrawlingLog.Log.Info(
            this + " : rebuild " + this._index.Name);
          this._index.Rebuild();
        }
      }
    }
  }
}