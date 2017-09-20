namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System;
  using Sitecore.Data.Items;

  public class Sublayout : Sitecore.Web.UI.WebControls.Sublayout
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
              "/sitecore/Layout/Sublayouts//*[@@templateid = '{0}' and CompareCaseInsensitive(@Path, '{1}')]",
              Sitecore.TemplateIDs.Sublayout.ToString(),
              this.Path);
            Item[] sublayouts = this.GetItem().Database.SelectItems(query);

            if (sublayouts.Length == 1 
              && sublayouts[0].Fields["VaryByScheme"] != null 
              && sublayouts[0]["VaryByScheme"] != "1")
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
