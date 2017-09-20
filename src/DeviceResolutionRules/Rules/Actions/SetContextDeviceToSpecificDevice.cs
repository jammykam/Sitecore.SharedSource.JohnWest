namespace Sitecore.Sharedsource.Rules.Actions
{
  using Sitecore.Data.Items;

  public class SetContextDeviceToSpecificDevice<T> : 
    Sitecore.Rules.Actions.RuleAction<T>
    where T : Sitecore.Rules.RuleContext
  {
    public string DeviceID
    {
      get;
      set;
    }

    public override void Apply(T ruleContext)
    {
      Sitecore.Context.Device = new DeviceItem(
        ruleContext.Item.Database.GetItem(this.DeviceID));
      ruleContext.Abort();
    }
  }
}