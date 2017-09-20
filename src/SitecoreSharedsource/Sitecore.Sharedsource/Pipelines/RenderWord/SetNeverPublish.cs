namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  public class SetNeverPublish : RenderWordProcessor
  {
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.WordMediaItem, "args.WordMediaItem");

      if (args.WordMediaItem.Publishing.NeverPublish)
      {
        return;
      }

      using (new EditContext(args.WordMediaItem))
      {
        args.WordMediaItem.Publishing.NeverPublish = true;
      }
    }
  }
}