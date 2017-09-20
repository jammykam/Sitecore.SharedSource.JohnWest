namespace Sitecore.Sharedsource.Pipelines.GetPresentationDataSources
{
  using SC = Sitecore;

  public class Tester : GetDataSourcesPipelineProcessor
  {
    override public void Process(GetPresentationDataSourcesArgs args)
    {
      SC.Diagnostics.Log.Info(this + " : Process()", this);
    }

    public override void DoProcess(GetPresentationDataSourcesArgs args)
    {
      throw new System.NotImplementedException();
    }
  }
}