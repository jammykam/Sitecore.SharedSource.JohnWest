namespace Sitecore.Sharedsource.ContentSearch
{
  using CS = Sitecore.ContentSearch;
  using SC = Sitecore;

  public class TimeIntervalCommitPolicyWrapper : CS.TimeIntervalCommitPolicy
  {
    public new bool ShouldCommit
    {
      get
      {
        return SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately
          || base.ShouldCommit;
      }
    }
  }
}