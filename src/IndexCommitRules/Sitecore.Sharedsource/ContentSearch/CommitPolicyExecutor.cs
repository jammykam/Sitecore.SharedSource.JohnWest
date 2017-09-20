namespace Sitecore.Sharedsource.ContentSearch
{
  using System;

  using CS = Sitecore.ContentSearch;
  using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;
  using SC = Sitecore;

  public class CommitPolicyExecutor : CS.ICommitPolicyExecutor, IDisposable
  {
    /// <summary>
    /// Used by the Dispose() method to commit before destruction.
    /// </summary>
    private CS.IProviderUpdateContext _lastContext;

    /// <summary>
    /// Inform the commit policy after commits complete.
    /// </summary>
    /// <param name="context">Index processing context.</param>
    public void Committed(CS.IProviderUpdateContext context)
    {
      context.CommitPolicy.Committed();
    }

    /// <summary>
    /// Inform the commit policy of index updates. After the specified number
    /// of updates, invoke the rules engine to determine whether to 
    /// commit changes to the index to disk and optimize the index.
    /// </summary>
    /// <param name="context">Index processing context.</param>
    /// <param name="operation">Index operation.</param>
    public void IndexModified(
      CS.IProviderUpdateContext context, 
      CS.IndexOperation operation)
    {
      this._lastContext = context;
      context.CommitPolicy.IndexModified(operation);

      lock (this)
      {
        // for the RulesBasedCommitPolicy providder, 
        // it is important to call ShouldCommit before ShouldOptimize

        if (SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately
          || context.CommitPolicy.ShouldCommit)
        {
          context.Commit();
        }

        SC.Sharedsource.ContentSearch.RulesBasedCommitPolicy policy =
          context.CommitPolicy as SC.Sharedsource.ContentSearch.RulesBasedCommitPolicy;

        if (policy == null)
        {
          return;
        }

        if (policy.ShouldOptimize)
        {
          context.Optimize();
          policy.ShouldOptimize = false;
        }
      }
    }

    public void Dispose()
    {
      SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately = true;

      if (this._lastContext != null)
      {
        this._lastContext.Commit();
      }
    }
  }
}
