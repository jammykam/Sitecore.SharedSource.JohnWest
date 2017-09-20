namespace Sitecore.Sharedsource.ContentSearch
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;
  using CS = Sitecore.ContentSearch;
  using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;
  using SC = Sitecore;

  public class RulesBasedCommitPolicy : SC.ContentSearch.ICommitPolicy, SC.ContentSearch.ISearchIndexInitializable
  {
    private int _millisecondsThreshold = 100;

    private int _updatesThreshold = 100;

    private SC.Data.Items.Item _ruleRoot;

    /// <summary>
    /// When the last commit occurred
    /// </summary>
    private DateTime _lastCommit = DateTime.Now;

    /// <summary>
    /// The number of items updated since the last commit.
    /// </summary>
    private int _count = 0;

    /// <summary>
    /// Gets a value indicating whether the commit policy executor
    /// should commit changes to the index to disk.
    /// </summary>
    public bool ShouldCommit
    {
      get
      {
        if (SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately)
        {
          return true;
        }

        try
        {
          if (this._count < 1
            || ((this.MillisecondsThreshold < 1 || this._lastCommit.AddMilliseconds(this.MillisecondsThreshold).CompareTo(DateTime.Now) < 0)
            && (this.UpdatesThreshold < 1 || this._count < this.UpdatesThreshold)))
          {
            return false;
          }

          SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext ruleContext =
            new SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext(this._lastCommit, this._count);
          SC.Rules.RuleList<SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext> ruleList =
            SC.Rules.RuleFactory.GetRules<SC.Sharedsource.Rules.Indexing.Committing.IndexCommittingRuleContext>(
              this.RuleRoot.Axes.GetDescendants(), 
              "Rule");
          Assert.IsNotNull(ruleList, "ruleList");
          Assert.IsTrue(ruleList.Count > 0, "ruleList.Count < 1 in database " + this.RuleRoot.Database.Name);
          ruleList.Run(ruleContext);
          this.ShouldOptimize = ruleContext.ShouldOptimize;
          return ruleContext.ShouldCommit;
        }
        catch (Exception ex)
        {
          SC.Diagnostics.Log.Error(this.ToString() + " : " + ex + " : " + ex.Message, ex, this);
        }

        return true;
      }

      private set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets or sets a value that controls whether to invoke the rules engine (whether
    /// the potential number of items updated exceeds this threshold).
    /// </summary>
    public int UpdatesThreshold
    {
      get
      {
        return this._updatesThreshold;
      }

      set
      {
        this._updatesThreshold = value;
      }
    }

    /// <summary>
    /// Gets or sets a value that controls whether to invoke the rules engine (whether
    /// the number of milliseconds has elapsed since the last commit).
    /// </summary>
    public int MillisecondsThreshold
    {
      get
      {
        return this._millisecondsThreshold;
      }

      set
      {
        this._millisecondsThreshold = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the commit policy executor
    /// should optimize the index.
    /// </summary>
    public bool ShouldOptimize { get; set; }

    /// <summary>
    /// Gets or sets the name of the database that contains the rules to invoke.
    /// </summary>
    public SC.Data.Items.Item RuleRoot
    {
      get
      {
        if (this._ruleRoot == null)
        {
          string path = "/sitecore/system/Settings/Rules/Indexing/Committing/Rules";

          if (!string.IsNullOrEmpty(this.RuleDatabaseName))
          {
            SC.Data.Database db = SC.Configuration.Factory.GetDatabase(this.RuleDatabaseName);
            Assert.IsNotNull(db, "db: " + this.RuleDatabaseName);
            this._ruleRoot = db.GetItem(path);
          }
          else
          {
            foreach (SC.Data.Database db in SC.Configuration.Factory.GetDatabases())
            {
              this._ruleRoot = db.GetItem(path);

              if (this._ruleRoot != null)
              {
                return this._ruleRoot;
              }
            }
          }

          Assert.IsNotNull(this._ruleRoot, "_ruleRoot");
        }

        return this._ruleRoot;
      }
    }

    public string RuleDatabaseName { get; set; }

    /// <summary>
    /// Disposes of resources used by this implementation.
    /// </summary>
    public void Dispose()
    {
      SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately = true;
    }

    /// <summary>
    /// Increment the potential number of items updated since the last commit.
    /// </summary>
    /// <param name="operation">Indexing operation.</param>
    public void IndexModified(CS.IndexOperation operation)
    {
      ++this._count;
    }

    /// <summary>
    /// Reset the count of items updated since the last commit to zero
    /// and record the date and time of that commit operation.
    /// </summary>
    public void Committed()
    {
      this._lastCommit = DateTime.Now;
      this._count = 0;
    }

    /// <summary>
    /// Empty implementation.
    /// </summary>
    /// <param name="searchIndex">Search index.</param>
    public void Initialize(CS.ISearchIndex searchIndex)
    {
      Assert.ArgumentNotNull(searchIndex, "searchIndex");
      Assert.ArgumentNotNullOrEmpty(searchIndex.Name, "searchIndex.Name");

      if (!string.IsNullOrEmpty(this.RuleDatabaseName))
      {
        return;
      }

      string[] parts = searchIndex.Name.Split('_');

      if (parts.Length < 2)
      {
        return;
      }

      if (SC.StringUtil.Contains(
        parts[1],
        SC.Configuration.Factory.GetDatabaseNames()))
      {
        this.RuleDatabaseName = parts[1];
      }
    }
  }
}