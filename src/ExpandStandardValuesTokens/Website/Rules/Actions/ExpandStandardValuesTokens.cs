namespace Sitecore.Sharedsource.Rules.Actions
{
  using System;

  using SC = Sitecore;

  // rules engine action to expand standard values in the item
  public class ExpandStandardValuesTokens<T> :
    SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    public override void Apply(T ruleContext)
    {
      SC.Diagnostics.Assert.IsNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.IsNotNull(ruleContext.Item, "ruleContext.Item");
      string query = String.Format(
        "ancestor-or-self::*[@@templateid='{0}']",
        SC.TemplateIDs.BranchTemplate.ToString());

      // don't expand tokens in branch templates or standard values items
      if (ruleContext.Item.Axes.SelectSingleItem(query) != null
        || (ruleContext.Item.Template.StandardValues != null
        && ruleContext.Item.Template.StandardValues.ID == ruleContext.Item.ID))
      {
        return;
      }

      ruleContext.Item.Fields.ReadAll();

      foreach (SC.Data.Fields.Field field in
        ruleContext.Item.Fields)
      {
        // if any field appears to contain a standard values token
        // replace standard values tokens in all fields
        if (field.ContainsStandardValue && field.Value.Contains("$"))
        {
          using(new SC.Data.Items.EditContext(ruleContext.Item))
          {
            SC.Data.MasterVariablesReplacer replacer =
              SC.Configuration.Factory.GetMasterVariablesReplacer();
            SC.Diagnostics.Assert.IsNotNull(replacer, "replacer");
            replacer.ReplaceItem(ruleContext.Item);
            break;
          }
        }
      }
    }
  }
}
