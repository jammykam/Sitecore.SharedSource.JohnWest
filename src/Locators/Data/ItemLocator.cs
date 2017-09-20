namespace Sitecore.Sharedsource.Mvc.Data
{
  using System;

  using SC = Sitecore;

  public class ItemLocator : SC.Mvc.Data.ItemLocator
  {
    public ItemLocator(SC.Mvc.Data.ItemLocator innerLocator)
    {
      this.InnerLocator = innerLocator;
    }

    public SC.Mvc.Data.ItemLocator InnerLocator { get; set; }

    public override SC.Data.Items.Item GetItem(SC.Data.ID itemId)
    {
      SC.Data.Items.Item item = this.InnerLocator.GetItem(itemId);
      string message = String.Format(
        "{0} : GetItem(itemId[{1}]) : {2}", 
        this, 
        itemId,
        item != null ? item.Paths.FullPath : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return item;
    }

    public override SC.Data.Items.Item GetItem(string pathOrId)
    {
      SC.Data.Items.Item item = this.InnerLocator.GetItem(pathOrId);
      string message = String.Format(
        "{0} : GetItem(pathOrId[{1}]) : {2}",
        this,
        pathOrId,
        item != null ? item.Paths.FullPath : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return item;
    }

    public override SC.Data.Items.Item GetItem(
      string pathOrId, 
      SC.Data.Items.Item contextItem)
    {
      SC.Data.Items.Item item = this.InnerLocator.GetItem(pathOrId, contextItem);
      string message = String.Format(
        "{0} : GetItem(pathOrId[{1}], contextItem[{2}]) : {3}",
        this,
        pathOrId,
        contextItem != null ? contextItem.Paths.FullPath : "null",
        item != null ? item.Paths.FullPath : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return item;
    }

    public override SC.Data.Items.Item GetItem(
      string pathOrId, 
      string basePath)
    {
      SC.Data.Items.Item item = this.InnerLocator.GetItem(pathOrId, basePath);
      string message = String.Format(
        "{0} : GetItem(pathOrId[{1}], basePath[{2}]) : {3}",
        this,
        pathOrId,
        basePath,
        item != null ? item.Paths.FullPath : "null");
      SC.Diagnostics.Log.Info(message, this);
      SC.Diagnostics.Log.Info(Environment.StackTrace, this);
      return item;
    }
  }
}