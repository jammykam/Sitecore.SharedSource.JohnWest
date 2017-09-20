namespace Sitecore.Sharedsource.Rules.Conditions
{
  using System;
  using System.Collections;

  using SC = Sitecore;

  public class CountPlaceholderRenderings<T> : 
    SC.Rules.Conditions.WhenCondition<T> where T : SC.Rules.RuleContext
  {
    public string PlaceholderKey { get; set; }
    public int MaxRenderings { get; set; }

    protected override bool Execute(T ruleContext)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.ArgumentNotNull(
        ruleContext.Item,
        "ruleContext.Item");
      SC.Diagnostics.Assert.ArgumentNotNullOrEmpty(
        this.PlaceholderKey,
        "PlaceholderKey");
      SC.Diagnostics.Assert.IsTrue(
        this.MaxRenderings > 0,
        "MaxRenderings");
      // note: retrieving the field value without using LayoutField
      // would not support layout deltas.
      SC.Data.Fields.LayoutField field =
        ruleContext.Item.Fields[SC.FieldIDs.LayoutField];
      string xml = field.Value;

      if (String.IsNullOrEmpty(xml))
      {
        // number 5 is alive
        return false;
      }

      ArrayList devices = SC.Layouts.LayoutDefinition.Parse(xml).Devices;

      if (devices == null)
      {
        return false;
      }

      return this.CheckRenderingCounts(devices, ruleContext);
    }

    protected bool CheckRenderingCounts(
      ArrayList devices, 
      SC.Rules.RuleContext ruleContext)
    {
      // note: this applies for all devices - you couldn't allow 3 renderings 
      // in a placeholder for one device but only 1 for another device.
      // you could add another property to the condition to specify a single device
      foreach (SC.Layouts.DeviceDefinition device in devices)
      {
        if (String.IsNullOrEmpty(device.Layout)
          || device.Renderings == null)
        {
          continue;
        }

        int count = 0;

        // I find this more readable without LINQ
        foreach (SC.Layouts.RenderingDefinition rendering in device.Renderings)
        {
          if (String.IsNullOrEmpty(rendering.Placeholder))
          {
            continue;
          }

          if (rendering.Placeholder.EndsWith(
            this.PlaceholderKey, 
            StringComparison.OrdinalIgnoreCase))
          {
            count++;
          }
        }

        if (count <= this.MaxRenderings)
        {
          continue;
        }

 //       ruleContext.Parameters["Validation Message"] = count 
        ruleContext.Parameters["Validation Message"] = count 
          + " renderings bound to placeholder " 
          + this.PlaceholderKey 
          + " (limit " 
          + this.MaxRenderings +
          ")";
        return true;
      }

      return false;
    }
  }
}