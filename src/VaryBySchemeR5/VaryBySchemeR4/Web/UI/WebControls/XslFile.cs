namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using Sitecore.Data.Items;

  public class XslFile : Sitecore.Web.UI.WebControls.XslFile
  {
    private Tristate _varyByScheme = new Tristate();

    public bool VaryByScheme
    {
      get
      {
        if (this._varyByScheme == Tristate.Undefined)
        {
          this._varyByScheme = Tristate.True;

          if (!String.IsNullOrEmpty(this.Path))
          {
            string query = String.Format(
              "/sitecore/Layout/Renderings//*[@@templateid = '{0}' and CompareCaseInsensitive(@Path, '{1}')]",
              Sitecore.TemplateIDs.XSLRendering.ToString(),
              this.Path);
            Item[] renderings = this.GetItem().Database.SelectItems(query);

            if (renderings.Length == 1
              && renderings[0].Fields["VaryByScheme"] != null
              && renderings[0]["VaryByScheme"] != "1")
            {
              this._varyByScheme = Tristate.False;
            }
          }
        }

        return this._varyByScheme == Tristate.True;
      }

      set
      {
        if (value)
        {
          this._varyByScheme = Tristate.True;
        }
      }
    }

    public override string GetCacheKey()
    {
      string key = base.GetCacheKey();

      if (String.IsNullOrEmpty(key) || !this.VaryByScheme)
      {
        return key;
      }

      return key + "_#scheme:" + Sitecore.Context.Page.Page.Request.Url.Scheme;
    }
  }
}