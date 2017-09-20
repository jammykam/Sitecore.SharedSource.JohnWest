namespace Sitecore.Sharedsource.Pipelines.RenderWord.ProcessWordNodeTree
{
  using System.Collections.Generic;

  using HtmlAgilityPack;

  using Sitecore.Diagnostics;

  public class ExtractTokenTable : ProcessWordNodeTreeProcessor
  {
    public override void Process(ProcessWordNodeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.CurrentElement, "args.CurrentElement");
      Assert.ArgumentNotNull(args, "listField");

      if (args.CurrentElement.Name != "table")
      {
        return;
      }

      HtmlNodeCollection match = args.CurrentElement.SelectNodes(".//tr[1]//th[1][starts-with(text(),'Token')]");

      if (match == null || match.Count < 1)
      {
        return;
      }

      match = args.CurrentElement.SelectNodes(".//tr[position() != 1]");
      Assert.IsTrue(match.Count > 0, "invalid token table");
      Assert.IsNull(args.TokenTable, "multiple token tables");
      args.TokenTable = new Dictionary<string, string>();

      // the table should be key | version | value. 
      // This merges the version with the key to create key=value pairs like keyversion=value.
      for (int row = 0; row < match.Count; row++)
      {
        HtmlNodeCollection columns = match[row].SelectNodes(".//td");
        Assert.IsTrue(columns != null && columns.Count == 3, "invalid number of columns in token table");
////TODO:        string token = TextBlockUtil.TrimToken(columns[0].InnerText.Trim());
        string token = columns[0].InnerText.Trim();
        string version = columns[1].InnerText.Trim();

        if (!string.IsNullOrEmpty(version))
        {
          token += version;
        }

        args.TokenTable[token] = columns[2].InnerText.Trim();
      }

      args.CurrentElement.Remove();
      args.AbortPipeline();
    }
  }
}