namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordElement
{
  using HtmlAgilityPack;

  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  public class ProcessWordElementArgs : PipelineArgs
  {
    public MediaItem MediaItem { get; set; }
    public Database Database { get; private set; }
    public HtmlNode Element { get; private set; }
    public string Path { get; private set; }
    public string Name { get; private set; }

    public ProcessWordElementArgs(Database db, HtmlNode element, string name, string path)
    {
      Assert.ArgumentNotNull(db, "db");
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNullOrEmpty(name, "name");
      this.Database = db;
      this.Element = element;
      this.Name = name;
      this.Path = path;

      if (string.IsNullOrEmpty(this.Path))
      {
        this.Path = string.Empty;
      }
    }
  }
}