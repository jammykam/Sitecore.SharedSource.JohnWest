namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;

  public class Link : Sitecore.Shell.Applications.ContentEditor.Link
  {
    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      // if source does not consist of key=value pairs, do nothing.

      if (String.IsNullOrEmpty(this.Source) || this.Source.IndexOf("=") < 0)
      {
        return;
      }

      Sitecore.Collections.SafeDictionary<string> parsed = 
        Sitecore.Web.WebUtil.ParseQueryString(this.Source);
      this.Source = String.Empty;

      foreach(string key in parsed.Keys)
      {
        switch(key.ToLower())
        {
          case "disabletextinput":
            if (parsed[key].ToLower() == "true")
            {
              this.Disabled = true;
            }

            break;
          case "datasource":
            this.Source = parsed[key];
            break;
          default:
//            throw new Exception("Unrecognized source parameter: " + key);
            break;
        }
      }
    }
  }
}
