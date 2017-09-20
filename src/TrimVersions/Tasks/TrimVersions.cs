namespace Sitecore.Sharedsource.Tasks
{
  using System;

  public class TrimVersions
  {
    public int MaxVersions
    {
      get;
      set;
    }

    public string Database 
    { 
      get; 
      set;
    }

    public string RootItem
    {
      get; 
      set; 
    }

    protected void Run()
    {
      if (this.MaxVersions < 1)
      {
        this.MaxVersions = 10;
      }

      if (String.IsNullOrEmpty(this.Database))
      {
        this.Database = "master";
      }

      Sitecore.Data.Database db =
        Sitecore.Configuration.Factory.GetDatabase(this.Database);
      Sitecore.Diagnostics.Assert.IsNotNull(db, this.Database);

      if (String.IsNullOrEmpty(this.RootItem))
      {
        this.RootItem = db.GetRootItem().Paths.FullPath;
      }

      foreach (Sitecore.Globalization.Language language in db.Languages)
      {
        this.Iterate(db.GetItem(this.RootItem, language));
      }
    }

    protected void Iterate(Sitecore.Data.Items.Item item)
    {
      while (item.Versions.GetVersionNumbers().Length > this.MaxVersions)
      {
        string msg = String.Format(
          "{0} : delete version {1} of {2} in {3} for {4}",
          this,
          item.Versions.GetVersionNumbers()[0],
          item.Versions.GetVersionNumbers().Length,
          item.Language.Name,
          item.Paths.FullPath);
        Sitecore.Diagnostics.Log.Audit(msg, this);
        item.Versions[item.Versions.GetVersionNumbers()[0]].Versions.RemoveVersion();
      }

      foreach (Sitecore.Data.Items.Item child in item.Children)
      {
        this.Iterate(child);
      }
    }
  }
}