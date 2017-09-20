namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using System.Collections.Generic;

  using DocumentFormat.OpenXml.Packaging;

  using HtmlAgilityPack;

  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Data.Items;

  public class RenderWordArgs : PipelineArgs
  {
    /// <summary>
    /// Gets the Microsoft Word document
    /// </summary>
    public WordprocessingDocument WordDoc { get; private set; }

    /// <summary>
    /// Gets the database for related item operations, including media.
    /// </summary>
    public Database Database { get; private set; }

    /// <summary>
    /// Gets or sets simple HTML-like representation of the Word document.
    /// </summary>
    public HtmlDocument HtmlDoc { get; set; }

    public string Path { get; private set; }
    public string Name { get; private set; }

    /// <summary>
    /// Gets or sets the title retrieved from the Word document.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets the media item representing the Word document.
    /// </summary>
    public Item WordMediaItem { get; private set; }

    /// <summary>
    /// Gets the field for storing the title.
    /// </summary>
    public Field TitleField { get; private set; }

    /// <summary>
    /// Gets the field for storing the token table.
    /// </summary>
    public Field TokenTableField { get; private set; }

    /// <summary>
    /// Gets the field for storing the body.
    /// </summary>
    /// 
    public Field BodyField { get; private set; }

    /// <summary>
    /// Gets or sets the token table.
    /// </summary>
    public Dictionary<string, string> TokenTable { get; set; }

    public RenderWordArgs(
      Item wordMediaItem,
      WordprocessingDocument doc, 
      Database db, 
      string name, 
      Field titleField,
      Field tokenTableField,
      Field bodyField,
      string path = "")
    {
      Assert.ArgumentNotNull(wordMediaItem, "wordMediaItem");
      Assert.ArgumentNotNull(doc, "doc");
      Assert.ArgumentNotNull(db, "db");
      Assert.ArgumentNotNullOrEmpty(name, "name");
      Assert.ArgumentNotNull(titleField, "titleField");
      Assert.ArgumentNotNull(tokenTableField, "tokenTableField");
      Assert.ArgumentNotNull(bodyField, "bodyField");
      this.WordMediaItem = wordMediaItem;
      this.Name = name;      
      this.WordDoc = doc;
      this.Database = db;
      this.TitleField = titleField;
      this.TokenTableField = tokenTableField;
      this.BodyField = bodyField;
      this.Path = path ?? string.Empty;
    }
  }
}