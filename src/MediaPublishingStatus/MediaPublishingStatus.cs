namespace Sitecore.Sharedsource.Data.Validators.ItemValidators
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.Serialization;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.Links;

  [Serializable]
  public class MediaPublishingStatus : StandardValidator
  {
    public MediaPublishingStatus() : base()
    {
    }

    public MediaPublishingStatus(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string Name
    {
      get
      {
        return this.GetType().ToString();
      }
    }

    public static bool IsPublished(Item item, Database targetDatbase)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNull(targetDatbase, "targetDatbase");
      Item targetItem = targetDatbase.GetItem(item.ID, item.Language, item.Version);
      return targetItem != null
             && item.Version.Number == targetItem.Version.Number
             && item.Statistics.Revision == targetItem.Statistics.Revision;
    }

    public static Database[] GetPublishingTargets(Item item)
    {
      Assert.ArgumentNotNull(item, "item");
      Item pubTargets = item.Database.GetItem("/sitecore/system/publishing targets");
      Assert.IsNotNull(pubTargets, "publishing targets");
      List<Database> databases = new List<Database>();

      foreach (Item target in pubTargets.Children)
      {
        if (PublishingTargetApplies(item, target.ID))
        {
          string name = target[FieldIDs.PublishingTargetDatabase];
          Assert.IsNotNull(name, target.Paths.FullPath);
          Database db = Factory.GetDatabase(name);
          Assert.IsNotNull(db, name);
          databases.Add(db);
        }
      }

      return databases.ToArray();
    }

    public static bool PublishingTargetApplies(Item item, ID publishingTarget)
    {
      while (item != null)
      {
        string restricted = item[FieldIDs.PublishingTargets];

        if (!(String.IsNullOrEmpty(restricted) || restricted.Contains(item.ID.ToString())))
        {
          return false;
        }

        item = item.Parent;
      }

      return true;
    }

    protected override ValidatorResult Evaluate()
    {
      Item item = this.GetItem();
      Assert.IsNotNull(item, "GetItem");
      Database[] pubTargets = GetPublishingTargets(item);

      foreach (ItemLink link in item.Links.GetAllLinks())
      {
        if (link.SourceFieldID == Sitecore.Data.ID.Null)
        {
          continue;
        }

        Sitecore.Data.Fields.Field field = item.Fields[link.SourceFieldID];
        Assert.IsNotNull(field, link.SourceFieldID.ToString());
        Item media = link.GetTargetItem();

        if (String.IsNullOrEmpty(field.Value) || media == null || !media.Paths.IsMediaItem)
        {
          continue;
        }

        if (field.Value.Contains(media.ID.ToString()) || field.Value.Contains(ShortID.Encode(media.ID)))
        {
          foreach (Database db in pubTargets)
          {
            if (!IsPublished(media, db))
            {
              this.Text = String.Format(
                "The field {0} references the media item {1}, which is not current in the {2} database.",
                field.Name,
                media.Paths.FullPath,
                db.Name);
              return GetFailedResult(ValidatorResult.Error);
            }
          }
        }
      }

      return ValidatorResult.Valid;
    }

    protected override ValidatorResult GetMaxValidatorResult()
    {
      return ValidatorResult.Error;
    }
  }
}