namespace Sitecore.Sharedsource.Sites.SiteContext
{
  public static class GetHomeItemExtension
  {
    public static Sitecore.Data.Items.Item GetHomeItem(
      this Sitecore.Sites.SiteContext me, 
      Sitecore.Data.Database database)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(database, "database");
      return database.GetItem(me.StartPath);
    }

    public static Sitecore.Data.Items.Item GetHomeItem(
      this Sitecore.Sites.SiteContext me)
    {
      return GetHomeItemExtension.GetHomeItem(me, Sitecore.Context.Database);
    }
  }
}