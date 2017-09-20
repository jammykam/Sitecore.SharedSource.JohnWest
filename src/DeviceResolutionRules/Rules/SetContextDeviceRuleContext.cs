using Sitecore.Globalization;

namespace Sitecore.Sharedsource.Rules
{
  using Sitecore.Data;
  using Sitecore.Data.Items;

  public class SetContextDeviceRuleContext : Sitecore.Rules.RuleContext
  {
    private DeviceItem _evaluateDevice;

    public SetContextDeviceRuleContext(DeviceItem deviceItem)
    {
      this.EvaluateDevice = deviceItem;
      this.Item = this.EvaluateDevice.InnerItem;
    }

    public Item ContextItem
    {
      get { return Sitecore.Context.Item; }
    }

    public DeviceItem ContextDevice
    {
      get { return Sitecore.Context.Device; }
      set
      {
        Sitecore.Context.Device = value;
        this.Abort();
      }
    }

    public Database ContextDatabase
    {
      get { return Sitecore.Context.Database; }
    }

    public DeviceItem EvaluateDevice
    {
      get { return _evaluateDevice; }
      set { _evaluateDevice = value; }
    }

    public Language ContextLanguage
    {
      get { return Sitecore.Context.Language; }
    }
  }
}