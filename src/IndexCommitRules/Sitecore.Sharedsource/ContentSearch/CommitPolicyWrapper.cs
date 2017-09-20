namespace Sitecore.Sharedsource.ContentSearch
{
  using System;

  using Assert = Sitecore.Diagnostics.Assert;
  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class CommitPolicyWrapper : SC.ContentSearch.ICommitPolicy,
                                     IDisposable,
                                     SC.ContentSearch.ISearchIndexInitializable
  {
    private SC.ContentSearch.ICommitPolicy _wrappedPolicy;

    public CommitPolicyWrapper(SC.ContentSearch.ICommitPolicy policyToWrap)
    {
      Assert.ArgumentNotNull(policyToWrap, "policyToWrap");
      this._wrappedPolicy = policyToWrap;
    }

    public void Dispose()
    {
    }

    public void IndexModified(CS.IndexOperation operation)
    {
      this._wrappedPolicy.IndexModified(operation);
    }

    public void Committed()
    {
      this._wrappedPolicy.Committed();
    }

    public bool ShouldCommit
    {
      get
      {
        return SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately
          || this._wrappedPolicy.ShouldCommit;
      }
    }

    public void Initialize(CS.ISearchIndex searchIndex)
    {
    }
  }
}