namespace Sitecore.Sharedsource.Shell.Applications.WebEdit.Commands
{
  using System.Collections.Specialized;

  public class ResetField : Sitecore.Shell.Applications.WebEdit.Commands.WebEditCommand
  {
    public override void Execute(
      Sitecore.Shell.Framework.Commands.CommandContext context)
    {
      if (context.Items.Length != 1)
      {
        return;
      }

      Sitecore.Data.Items.Item item = context.Items[0];
      NameValueCollection parameters = new NameValueCollection();
      parameters["id"] = item.ID.ToString();
      parameters["database"] = item.Database.ToString();
      parameters["language"] = item.Language.ToString();
      parameters["version"] = item.Version.ToString();
      parameters["field"] = context.Parameters["field"];
      Context.ClientPage.Start(this, "Run", parameters);
    }

    protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
    {
      if (!Sitecore.Web.UI.Sheer.SheerResponse.CheckModified())
      {
        return;
      }

      if (args.IsPostBack)
      {
        Sitecore.Web.UI.Sheer.SheerResponse.Confirm(Sitecore.Globalization.Translate.Text(
          "Reset {0} to its standard value?",
          new object[] {args.Parameters["field"]}));
        args.WaitForPostBack();
      }

      if (!args.HasResult)
      {
        return;
      }

      if (args.Result != "yes")
      {
        return;
      }

      Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase(args.Parameters["database"]);
      Sitecore.Globalization.Language language = Sitecore.Globalization.Language.Parse(args.Parameters["language"]);
      Sitecore.Data.Version version = Sitecore.Data.Version.Parse(args.Parameters["version"]);
      Sitecore.Data.Items.Item item = database.GetItem(args.Parameters["id"], language, version);

      using (new Sitecore.Data.Items.EditContext(item))
      {
        item.Fields[args.Parameters["field"]].Reset();
      }
    }
  }
}