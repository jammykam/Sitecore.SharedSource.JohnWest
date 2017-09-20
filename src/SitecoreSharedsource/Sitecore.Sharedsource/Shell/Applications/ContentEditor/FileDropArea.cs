////TODO: clean up orphaned media
////TODO: allow multiple and access the selected item int he webdav field
/// TODO: if someone changes the value in the HTML (or title?) field, clear the FDA field value
/// TODO: on any exception, abort pipeline

namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;
  using Sitecore.Collections;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.Web.UI.Sheer;

  using Assert = Sitecore.Diagnostics.Assert;

  public class FileDropArea : Sitecore.Shell.Applications.ContentEditor.FileDropArea
  {
    /// <summary>
    /// The item the user is editing, which contains the FDA field.
    /// </summary>
    private Item _item;

    private FileDropAreaField _field;

    /// <summary>
    /// The item the user is editing, which contains the FDA field.
    /// </summary>
    private Item Item
    {
      get
      {
        if (this._item == null)
        {
          this._item = Sitecore.Context.ContentDatabase.GetItem(
            this.ItemID,
            Language.Parse(this.ItemLanguage),
            Sitecore.Data.Version.Parse(this.ItemVersion));
          Assert.IsNotNull(this._item, "_item");
        }

        return this._item;
      }
    }

    private FileDropAreaField Field
    {
      get
      {
        if (this._field == null)
        {
          Field field = this.Item.Fields[this.FieldID];
          Assert.IsNotNull(field, "field");
          FileDropAreaField temp = new FileDropAreaField(field, this.Value);
          Assert.IsNotNull(temp, "temp");
          this._field = temp;
        }

        return this._field;
      }
    }

    public override void HandleMessage(Message message)
    {
      base.HandleMessage(message);

      if (message == null || message["id"] != this.ID || message.Name != "contentfiledroparea:convert")
      {
        return;
      }

//      S.Context.ClientPage.SendMessage(this, "item:load(id=" + this.ItemID + ",language=" + this.ItemLanguage + ")");
      Sitecore.Context.ClientPage.Start(this, "Start");
    }

    public void Start(ClientPipelineArgs args)
    {
      long start = DateTime.Now.Ticks;
      this.StartProgressBox(args);

      // if the progress box likely appeared, 
      // then don't show the alert
      if (DateTime.Now.Ticks - start < 1000000
        && !args.Aborted)
      {
        SheerResponse.Alert(
          Translate.Text("Word file(s) converted."));
      }
    }

    /// <summary>
    /// Shows the progress box that invokes Apply().
    /// </summary>
    private void StartProgressBox(ClientPipelineArgs args)
    {
      // create a progress box with a job to call the Apply() method of this class.
      // Afterwards, refresh the item.
      Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute(
        string.Format("{0}", Translate.Text("Applying updates from file(s)")),
        string.Format("{0}",Translate.Text("Applying updates from file(s)")),
        "People/16x16/hammer.png",
        this.Apply,
        "item:load(id=" + this.Item.ID + ")",
        new object[] { args });
    }

    /// <summary>
    /// Nested logic for argument processing and possible UI alerts.
    /// </summary>
    private void Apply(object[] parameters)
    {
      Assert.ArgumentCondition(
        parameters != null && parameters.Length == 1 && parameters[0] is ClientPipelineArgs,
        "parameters",
        "//TODO");
      ClientPipelineArgs args = parameters[0] as ClientPipelineArgs;

      // the FDA field of the item edited by the user
      Field field = this.Item.Fields[this.FieldID];
      Assert.IsNotNull(field, "field");

      FileDropAreaField fda = new FileDropAreaField(
        field, 
        base.GetViewStateProperty("XmlValue", string.Empty) as string);
      ItemList mediaItems = fda.GetMediaItems();

      if (mediaItems.Count < 1 )
      {
        SheerResponse.Alert(
          Translate.Text("Add a media item."));
        args.AbortPipeline();
        return;
      }

      Item item = null;

      if (mediaItems.Count == 1)
      {
        item = mediaItems[0];
      }
      else if (mediaItems.Count > 1)
      { 
        ItemList selectedItems = null;

        try
        {
          // this may throw an exception; see 406891
          selectedItems = this.GetSelectedItems();
        }
        catch (Exception ex)
        {
          SheerResponse.Alert(
            Translate.Text("Select an item from the list."));
          args.AbortPipeline();
          return;
        }

        if (selectedItems.Count != 1)
        {
          SheerResponse.Alert(
            Translate.Text("Select a single item from the list."));
          args.AbortPipeline();
          return;
        }

        item = selectedItems[0];
      }

      MediaItem mediaItem = new MediaItem(item);

      if (mediaItem.Extension != "docx")
      {
        SheerResponse.Alert(Translate.Text(
          "Seelect a Word document from the list."));
        args.AbortPipeline();
        return;
      }

      WordUtil.HandleWordDocument(this.Item, mediaItem);
    }
  }
}
