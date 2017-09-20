namespace Sitecore.Sharedsource.Data.Fields
{
  using System;
  using System.Collections;
  using System.Text.RegularExpressions;

  public class LayoutField : Sitecore.Data.Fields.LayoutField
  {
    public LayoutField(Sitecore.Data.Fields.Field innerField) 
      : base(innerField)
    {
    }

    public LayoutField(Sitecore.Data.Items.Item item) 
      : base(item)
    {
    }

    public LayoutField(Sitecore.Data.Fields.Field innerField, string runtimeValue) 
      : base(innerField, runtimeValue)
    {
    }

    public void Relink(Sitecore.Data.Items.Item old, Sitecore.Data.Items.Item updated)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(old, "old");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(updated, "updated");
      this.Replace(old.Paths.FullPath, old.ID.ToString(), updated.Paths.FullPath, updated.ID.ToString());
    }

    protected void Replace(string oldPath, string oldId, string updatedPath, string updatedId)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(oldPath, "oldPath");
      Sitecore.Diagnostics.Assert.ArgumentNotNullOrEmpty(oldId, "oldId");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(updatedPath, "updatedPath");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(updatedId, "updatedId");

      if (this.Value.IndexOf(oldPath, StringComparison.OrdinalIgnoreCase) >= 0)
      {
        this.Value = Regex.Replace(
          this.Value,
          oldPath,
          updatedPath,
          RegexOptions.IgnoreCase);
      }

      if (this.Value.IndexOf(oldId, StringComparison.OrdinalIgnoreCase) >= 0)
      {
        this.Value = Regex.Replace(
          this.Value,
          oldId,
          updatedId,
          RegexOptions.IgnoreCase);
      }
    }

    public override void RemoveLink(Links.ItemLink itemLink)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(itemLink, "itemLink");
      this.Replace(
        itemLink.TargetPath, 
        itemLink.TargetItemID.ToString(), 
        String.Empty, 
        String.Empty);
    }

    public override void Relink(Sitecore.Links.ItemLink link, Sitecore.Data.Items.Item updated)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(link, "link");
      Sitecore.Diagnostics.Assert.ArgumentNotNull(updated, "updated");
      this.Replace(
        link.TargetPath, 
        link.TargetItemID.ToString(), 
        updated.Paths.FullPath, 
        updated.ID.ToString());
    }

    public override void ValidateLinks(Links.LinksValidationResult result)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(result, "result");
      base.ValidateLinks(result);

      if (string.IsNullOrEmpty(this.Value))
      {
        return;
      }

      ArrayList devices = Sitecore.Layouts.LayoutDefinition.Parse(this.Value).Devices;

      if (devices == null)
      {
        return;
      }

      foreach (Sitecore.Layouts.DeviceDefinition device in devices)
      {
        if (String.IsNullOrEmpty(device.Layout))
        {
          continue;
        }

        if (device.Renderings != null)
        {
          foreach (Sitecore.Layouts.RenderingDefinition rendering in device.Renderings)
          {
            if (String.IsNullOrEmpty(rendering.Datasource)
                || !(rendering.Datasource.StartsWith("/") || rendering.Datasource.StartsWith("{")))
            {
              continue;
            }

            Sitecore.Data.Items.Item dataSource = this.InnerField.Database.GetItem(
              rendering.Datasource);

            if (dataSource != null)
            {
              result.AddValidLink(dataSource, rendering.Datasource);
            }
            else if (dataSource.ID != this.InnerField.Item.ID)
            {
              result.AddBrokenLink(rendering.Datasource);
            }
          }
        }
      }
    }
  }
}