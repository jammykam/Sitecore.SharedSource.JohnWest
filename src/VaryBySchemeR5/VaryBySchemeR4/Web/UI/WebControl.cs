namespace Sitecore.Sharedsource.Web.UI
{
  using System;
  using Sitecore.Data.Items;

  public abstract class WebControl : Sitecore.Web.UI.WebControl
  {
    private Tristate _varyByScheme = new Tristate();

    public bool VaryByScheme
    {
      get
      {
        if (this._varyByScheme == Tristate.Undefined)
        {
          this._varyByScheme = Tristate.True;
          string query = String.Format(
            "/sitecore/Layout/Renderings//*[@@templateid = '{0}' and CompareCaseInsensitive(@Namespace,'{1}') and CompareCaseInsensitive(@Tag,'{2}')]",
            "{1DDE3F02-0BD7-4779-867A-DC578ADF91EA}",
            this.GetType().Namespace,
            this.GetType().Name);
          Item[] renderings = this.GetItem().Database.SelectItems(query);

          if (renderings.Length == 1
            && renderings[0].Fields["VaryByScheme"] != null
            && renderings[0]["VaryByScheme"] != "1")
          {
            this._varyByScheme = Tristate.False;
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

    protected override string GetCachingID()
    {
      string id = base.GetCachingID();

      if (String.IsNullOrEmpty(id))
      {
        id = this.GetType().ToString();
      }

      return id;
    }

    public override string GetCacheKey()
    {
      string key = base.GetCacheKey();

      if (String.IsNullOrEmpty(key))
      {
        return key;
      }

      if (this.VaryByScheme)
      {
        key += "_#scheme:" + Sitecore.Context.Page.Page.Request.Url.Scheme;
      }

      return key;
    }
  }
}