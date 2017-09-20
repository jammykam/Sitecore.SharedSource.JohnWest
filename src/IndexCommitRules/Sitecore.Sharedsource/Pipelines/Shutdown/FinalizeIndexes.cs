namespace Sitecore.Sharedsource.Pipelines.Shutdown
{
  using SC = Sitecore;

  public class FinalizeIndexes
  {
    // TODO: set could be private, unless else could set this property?
    public static bool FinalizeImmediately { get; set; }

    public void Process(SC.Pipelines.PipelineArgs args)
    {
      SC.Sharedsource.Pipelines.Shutdown.FinalizeIndexes.FinalizeImmediately = true;
    }
  }
}