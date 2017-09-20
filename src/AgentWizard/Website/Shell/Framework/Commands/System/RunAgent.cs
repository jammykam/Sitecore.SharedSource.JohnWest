using System;

namespace Sitecore.Sharedsource.Shell.Framework.Commands.System
{
  [Serializable]
  public class RunAgent : Sitecore.Shell.Framework.Commands.Command
  {
    public override void Execute(
      Sitecore.Shell.Framework.Commands.CommandContext context)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(
        context,
        "context");

      // THe UI framework invokes the Process() method defined in this class
      Sitecore.Context.ClientPage.Start(this, "Process");
    }

    protected void Process(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (!args.IsPostBack)
      {
        string url = new Sitecore.Text.UrlString(
          Sitecore.UIUtil.GetUri("control:RunAgent")).ToString();
        Sitecore.Web.UI.Sheer.SheerResponse.ShowModalDialog(url, true);
        args.WaitForPostBack();
      }
    }
  }
}

