namespace Sitecore.Sharedsource.Publishing.Pipelines.Publish
{
  using System;
  using Sitecore.Publishing.Pipelines.Publish;
  using Sitecore.Sharedsource.Diagnostics;

  //TODO: may not have rights to invoke command
  //TODO: synchronous or asynchronous?
  //TODO: is it appropriate to do this on publish? 
  //TODO: Is there a better way to indicate when to do this?

  public class InvokePublishingTargetCLT : Sitecore.Publishing.Pipelines.Publish.PublishProcessor
  {
    public override void Process(PublishContext context)
    {
      foreach(string name in context.PublishOptions.PublishingTargets)
      {
        string path = "/sitecore/system/publishing targets/" + name;
        Sitecore.Data.Items.Item target = context.PublishOptions.SourceDatabase.GetItem(path);
        Sitecore.Diagnostics.Assert.IsNotNull(target, path);
        string command = target["CommandLine"];

        if (String.IsNullOrEmpty(command))
        {
          return;
        }

        CommandLineTool clt = new CommandLineTool(command);
        clt.Execute(true);
      }
    }
  }
}