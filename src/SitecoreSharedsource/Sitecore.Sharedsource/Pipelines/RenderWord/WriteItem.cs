namespace Sitecore.Sharedsource.Pipelines.RenderWord
{
  using System.Text.RegularExpressions;

  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;

  public class WriteItem : RenderWordProcessor
  {
    public override void Process(RenderWordArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args.HtmlDoc, "args.HtmlDoc");
      Assert.ArgumentNotNull(args.HtmlDoc.DocumentNode, "args.HtmlDoc.DocumentNode");
      Assert.ArgumentNotNull(args.TitleField, "args.TitleField");
      Assert.ArgumentNotNull(args.BodyField, "args.BodyField");
      Assert.ArgumentNotNull(args.TokenTableField, "args.TokenTableField");

      using (new EditContext(args.BodyField.Item))
      {
        args.BodyField.Value = Regex.Replace(
          args.HtmlDoc.DocumentNode.WriteTo(),
          "</?(body|sitecorebulletedlistchar)>",
          string.Empty).Replace("<code>\r\n", "<code>");
      }

      if ((!string.IsNullOrEmpty(args.Title)) && args.Title != args.TitleField.Value)
      {
        using (new EditContext(args.TitleField.Item))
        {
          args.TitleField.Value = args.Title;
        }
      }

      string value = string.Empty;

      //TODO: if token table is empty, should clear token table?
      if (args.TokenTable != null)
      {
        foreach (string key in args.TokenTable.Keys)
        {
          value += key + "=" + args.TokenTable[key].Trim() + "&";
        }

        using (new EditContext(args.TokenTableField.Item))
        {
          args.TokenTableField.Value = value.TrimEnd('&');
        }
      }
    }
  }
}