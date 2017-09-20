namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;
  using System.Collections.Specialized;

  public class FieldDropLink : Sitecore.Shell.Applications.ContentEditor.LookupEx
  {
    protected override string GetItemHeader(Sitecore.Data.Items.Item item)
    {
      if (String.IsNullOrEmpty(this.FieldName)
        || item[this.FieldName].StartsWith("@")) // don't impact default usage
      {
        return base.GetItemHeader(item);
      }

      if (String.IsNullOrEmpty(item[this.FieldName]))
      {
        return item.DisplayName;
      }

      return item[this.FieldName];
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      if (String.IsNullOrEmpty(this.Source)
        || !this.Source.Contains("="))
      {
        return;
      }

      NameValueCollection args = Sitecore.Web.WebUtil.ParseUrlParameters(
        this.Source);

      if (!String.IsNullOrEmpty(args["field"]))
      {
        this.FieldName = args["field"];
      }

      this.Source = args["source"] == null ? String.Empty : args["source"];
    }
  }
}