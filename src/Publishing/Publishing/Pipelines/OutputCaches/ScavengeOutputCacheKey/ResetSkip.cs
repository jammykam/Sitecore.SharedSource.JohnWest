namespace Publishing.Pipelines.OutputCaches.ScavengeOutputCacheKey
{
  public class ResetSkip : ScavengeOutputCacheKeyProcessor
  {
    protected new void Process(
      ScavengeOutputCacheKeyArgs args)
    {
      args.Skip = false;
    }

    protected override void DoProcess(ScavengeOutputCacheKeyArgs args)
    {
    }
  }
}
