namespace Sitecore.Sharedsource.Pipelines.GetContentEditorWarnings
{
  using System;

  public class PublishingStatus
  {
    public string SourceDatabase
    {
      get;
      set;
    }

    public void Process(
      Sitecore.Pipelines.GetContentEditorWarnings.GetContentEditorWarningsArgs args)
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(this.SourceDatabase, "SourceDatabase");
      Sitecore.Diagnostics.Assert.IsNotNull(args, "args");
      Sitecore.Data.Items.Item sourceItem = args.Item;
      Sitecore.Diagnostics.Assert.IsNotNull(sourceItem, "args.Item");

      if (String.Compare(sourceItem.Database.Name, this.SourceDatabase, false) != 0)
      {
        return;
      }

      if (!this.IsLatest(sourceItem))
      {
        return;
      }

      string message = this.GetMessage(sourceItem);

      if (!String.IsNullOrEmpty(message))
      {
        this.AddWarning(message, sourceItem, args);
      }
    }

    protected bool IsLatest(Sitecore.Data.Items.Item item)
    {
      Sitecore.Data.Items.Item latest = item.Database.GetItem(
        item.ID,
        item.Language,
        Sitecore.Data.Version.Latest);

      if (latest.Version.Number != item.Version.Number)
      {
        return false;
      }

      return true;
    }

    protected string GetMessage(Sitecore.Data.Items.Item item)
    {
      Sitecore.Data.Items.Item targetsRoot =
        item.Database.GetItem("/sitecore/system/publishing targets");
      Sitecore.Diagnostics.Assert.IsNotNull(targetsRoot, "/sitecore/system/publishing targets");
      string message = String.Empty;

      foreach (Sitecore.Data.Items.Item target in targetsRoot.Children)
      {
        if (!this.PublishingTargetApplies(item, target.ID))
        {
          continue;
        }

        string dbName = target[Sitecore.FieldIDs.PublishingTargetDatabase];
        Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(dbName, target.Paths.FullPath);
        Sitecore.Data.Database db = Sitecore.Configuration.Factory.GetDatabase(dbName);
        Sitecore.Diagnostics.Assert.IsNotNull(db, dbName);
        Sitecore.Data.Items.Item targetItem = db.GetItem(item.ID, item.Language);

        if (targetItem == null)
        {
          message += "You have not published this item to the " + dbName + " database.<br />";
        }
        else if (item.Version.Number != targetItem.Version.Number)
        {
          message += "You have not published this version to the " + dbName + " database.<br />";
        }
        else if (item[Sitecore.FieldIDs.Revision] != targetItem[Sitecore.FieldIDs.Revision])
        {
          message += "You have not published this revision to the "
            + dbName
            + " database.<br />";
        }
      }

      return message;
    }

    protected bool PublishingTargetApplies(
      Sitecore.Data.Items.Item item,
      Sitecore.Data.ID publishingTarget)
    {
      while (item != null)
      {
        string restricted = item[Sitecore.FieldIDs.PublishingTargets];

        if (!(String.IsNullOrEmpty(restricted) || restricted.Contains(item.ID.ToString())))
        {
          return false;
        }

        item = item.Parent;
      }

      return true;
    }

    protected void AddWarning(
      string message,
      Sitecore.Data.Items.Item item,
      Sitecore.Pipelines.GetContentEditorWarnings.GetContentEditorWarningsArgs args)
    {
      Sitecore.Pipelines.GetContentEditorWarnings.GetContentEditorWarningsArgs.ContentEditorWarning warning
        = args.Add();
      warning.Title = "Publishing Status";
      warning.Text = message;
      string command = String.Format(
        "item:load(id={0},language={1},version={2})",
        item.ID,
        item.Language,
        item.Version.Number);
      warning.AddOption("Refresh", command);
    }
  }
}