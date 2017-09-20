namespace Sitecore.Sharedsource.Syndication
{
  using System;
  using Sitecore.Data.Items;
  using Sitecore.Web.UI.HtmlControls.Data;

  public class TagFeed : Sitecore.Syndication.PublicFeed
  {
    private string[] _tagIDs = null;
    private Item[] _potentialItems = null;

    public string[] TagIDs
    {
      get
      {
        if (this._tagIDs == null)
        {
          this._tagIDs = this.GetTagIDs();
        }

        return this._tagIDs;
      }
    }

    public Item[] PotentialItems
    {
      get
      {
        if (this._potentialItems == null)
        {
          this._potentialItems = this.GetPotentialItems();
        }

        return this._potentialItems;
      }
    }

    public override System.Collections.Generic.IEnumerable<Sitecore.Data.Items.Item> GetSourceItems()
    {
      if (this.TagIDs != null && this.TagIDs.Length > 0 
        && this.PotentialItems != null && this.PotentialItems.Length > 0)
      {
        foreach (Item entry in this.PotentialItems)
        {
          foreach (string id in this.TagIDs)
          {
            if (entry["Tags"].Contains(id))
            {
              yield return entry;
              continue;
            }
          }
        }
      }
    }

    protected string[] GetTagIDs()
    {
      Sitecore.Data.Fields.MultilistField tags = this.FeedItem.Fields["Tags"];

      if (tags == null || tags.Count < 1)
      {
        return null;
      }

      string[] tagIds = new string[tags.Count];

      for (int t = 0; t < tags.Count; t++)
      {
        tagIds[t] = tags.GetItems()[t].ID.ToString();
      }

      return tagIds;
    }

    protected Item[] GetPotentialItems()
    {
      string path = this.FeedItem["source"];
      Item[] items = null;

      if (!String.IsNullOrEmpty(path))
      {
        if (path.StartsWith("query:") || path.StartsWith("fast:"))
        {
          items = LookupSources.GetItems(this.FeedItem, path);
        }
        else
        {
          Item item = this.FeedItem.Database.GetItem(path);

          if (item != null)
          {
            items = item.Axes.GetDescendants();
          }
        }
      }
      else
      {
        items = this.FeedItem.Axes.GetDescendants();
      }

      return items;
    }
  }
}