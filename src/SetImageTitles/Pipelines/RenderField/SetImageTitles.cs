namespace Sitecore.Sharedsource.Pipelines.RenderField
{
  using System;
  using System.IO;
  using System.Text;
  using System.Text.RegularExpressions;

  public class SetImageTitles
  {
    private Regex regex = new Regex("/([A-F0-9]{32,32})\\.ashx");
    
    public void Process(Sitecore.Pipelines.RenderField.RenderFieldArgs args)
    {
      if (args.FieldTypeKey != "rich text"
        || (String.IsNullOrEmpty(args.Result.FirstPart) && String.IsNullOrEmpty(args.Result.LastPart)))
      {
        return;
      }

      args.Result.FirstPart = this.SetTitles(args.Result.FirstPart);
      args.Result.LastPart = this.SetTitles(args.Result.LastPart);
    }

    private string SetTitles(string value)
    {
      HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(value);
      HtmlAgilityPack.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//img[@src]");

      if (nodes == null || nodes.Count == 0)
      {
        return value;
      }

      string id = null; // non-null indicates need to update field value after looping

      foreach (HtmlAgilityPack.HtmlNode node in nodes)
      {
        string src = node.GetAttributeValue("src", String.Empty);

        if (src == null)
        {
          continue;
        }

        Match match = this.regex.Match(src);

        if (match.Success)
        {
          id = match.Groups[1].Value;
          Sitecore.Data.ID guid = Sitecore.Data.ID.Parse(id);
          Sitecore.Data.Items.Item item = Sitecore.Context.Database.GetItem(guid);

          if (item == null)
          {
            continue;
          }

          string title = String.Format(
            "{0} [{1}, {2}]",
            item.Name,
            item["extension"].ToUpper(),
            this.FormatBytes(Int32.Parse(item["size"])));
          node.SetAttributeValue("title", title);
        }
      }

      if (id == null)
      {
        return value;
      }

      StringBuilder sb = new StringBuilder();
      StringWriter sw = new StringWriter(sb);
      doc.Save(sw);
      sw.Flush();
      return sb.ToString();
    }

    // http://sharpertutorials.com/pretty-format-bytes-kb-mb-gb/
    private string FormatBytes(long bytes)
    {
      const int SCALE = 1024;
      string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
      long max = (long)Math.Pow(SCALE, orders.Length - 1);

      foreach (string order in orders)
      {
        if (bytes > max)
        {
          return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
        }

        max /= SCALE;
      }

      return "0 Bytes";
    }
  }
}