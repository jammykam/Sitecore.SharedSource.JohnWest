namespace Sitecore.Sharedsource.Pipelines.HttpRequest
{
  using System;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Pipelines.HttpRequest;
  using Sitecore.Rules;
  using Sitecore.Sharedsource.Rules;

  public class RulesEngineDeviceResolver : HttpRequestProcessor
  {
    public override void Process(HttpRequestArgs args)
    {
      if (Sitecore.Context.Database == null
        || Sitecore.Context.Item == null
        || String.Compare(Sitecore.Context.Database.Name, "core", true) == 0)
      {
        return;
      }

      // If a global rule applied, device rules do not apply.
      if (this.InvokeGlobalRules())
      {
        return;
      }

      this.ApplyDeviceRules();
    }

    /// <summary>
    /// Invoke global device resolution rules.
    /// </summary>
    /// <returns>True if a global device resolution rule applied.</returns>
    protected bool InvokeGlobalRules()
    {
      Sitecore.Data.Items.Item ruleRoot = Sitecore.Context.Database.GetItem(
        "/sitecore/system/Settings/Rules/Determine Context Device/Rules");

      if (ruleRoot == null)
      {
        return false;
      }

      // TODO: use descendants instead of children
      // for each possible global rule definition item
      foreach (Sitecore.Data.Items.Item child in ruleRoot.Children)
      {
        string ruleXml = child["Rule"];

        if (String.IsNullOrEmpty(ruleXml) || child["Disabled"] == "1")
        {
          continue;
        }

        // parse the rule XML
        RuleList<RuleContext> rules = new RuleList<RuleContext>();
        rules.Name = child.Paths.Path;
        RuleList<RuleContext> parsed = RuleFactory.ParseRules<RuleContext>(
          Sitecore.Context.Database, 
          ruleXml);
        rules.AddRange(parsed.Rules);

        if (rules == null || rules.Count < 1)
        {
          continue;
        }

        // invoke the rule
        RuleContext ruleContext = new RuleContext();
        ruleContext.Item = Sitecore.Context.Item;
        rules.Run(ruleContext);

        // rule applied
        if (ruleContext.IsAborted)
        {
          return true;
        }
      }

      return false;
    }

    protected bool ApplyDeviceRules()
    {
      // for each device
      foreach (DeviceItem device in
        Sitecore.Context.Database.Resources.Devices.GetAll())
      {
        // for each field of the device
        foreach (Field field in device.InnerItem.Fields)
        {
          // ignore empty fields and fields of type other than rules.
          if (field.TypeKey != "rules" || String.IsNullOrEmpty(field.Value))
          {
            continue;
          }

          // parse the rule definition XML in the field.
          RuleList<SetContextDeviceRuleContext> rules = 
            new RuleList<SetContextDeviceRuleContext>();
          rules.Name = field.Item.Paths.Path;
          RuleList<SetContextDeviceRuleContext> parsed =
            RuleFactory.ParseRules<SetContextDeviceRuleContext>(field.Database, field.Value);
          rules.AddRange(parsed.Rules);

          if (rules == null || rules.Count < 1)
          {
            continue;
          }

          // invoke the rule.
          SetContextDeviceRuleContext ruleContext = new SetContextDeviceRuleContext(
            device);
          rules.Run(ruleContext);

          // rule applied.
          if (ruleContext.IsAborted)
          {
            return true;
          }
        }
      }

      return false;
    }
  }
}