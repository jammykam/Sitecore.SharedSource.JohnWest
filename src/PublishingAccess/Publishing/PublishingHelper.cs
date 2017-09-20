namespace Sitecore.Sharedsource.Publishing
{
  using System;
  using Sitecore.SecurityModel;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Security.AccessControl;
  using Sitecore.Security.Accounts;

  public class PublishingHelper
  {
    public static bool CanPublishItem(Item item, User user)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNull(user, "user");

      return PublishingHelper.CanPublishDatabase(item.Database)
        && PublishingHelper.HasDefaultPublishAccess(item, user)
        && PublishingHelper.HasCustomPublishAccess(item, user);
    }

    private static bool HasCustomPublishAccess(Item item, User user)
    {
      AccessRight itemPublish = AccessRight.FromName("item:publish");

      if (itemPublish != null)
      {
        using (new SecurityEnabler())
        {
          return AuthorizationManager.IsAllowed(item, itemPublish, user);
        }
      }

      return true;
    }

    private static bool HasDefaultPublishAccess(Item item, User user)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNull(user, "user");

      if (!Settings.Publishing.CheckSecurity)
      {
        return true;
      }

      using (new SecurityEnabler())
      {
        return AuthorizationManager.IsAllowed(item, AccessRight.ItemRead, user)
          && AuthorizationManager.IsAllowed(item, AccessRight.ItemWrite, user);
      }
    }

    public static bool CanPublishDatabase(Sitecore.Data.Database database)
    {
      Assert.ArgumentNotNull(database, "database");
      return String.Compare(database.Name, "master", true) == 0;
    }
  }
}