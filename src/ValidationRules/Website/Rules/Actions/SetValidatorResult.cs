namespace Sitecore.Sharedsource.Rules.Actions
{
  using System;
  using System.Reflection;

  using SC = Sitecore;

  public class SetValidatorResult<T> : Sitecore.Rules.Actions.RuleAction<T> 
    where T: Sitecore.Rules.Validators.ValidatorsRuleContext
  {
    // the ID of the item named after the ValidatorResult
    public string ValidatorResult { get; set; }

    // optional validation message
    public string Text { get; set; }

    public override void Apply(T ruleContext)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.IsNotNullOrEmpty(
        this.ValidatorResult,
        "ValidatorResult");

      // get the item from the ID
      // then get the value in the enumeration 
      // that matches the name of that item
      // http://www.sitecore.net/Community/Technical-Blogs/John-West-Sitecore-Blog/Posts/2012/03/Set-the-Priority-for-a-Scheduled-Agent-in-the-Sitecore-ASPNET-CMS.aspx
      SC.Data.Items.Item resultItem = SC.Client.ContentDatabase.GetItem(
        this.ValidatorResult);
      SC.Diagnostics.Assert.IsNotNull(resultItem, "result");
      Type enumType = typeof(SC.Data.Validators.ValidatorResult);
      object resultValue = Enum.Parse(
        enumType,
        resultItem.Name,
        true /*ignore the character case of the string to parse*/);
      Type ruleContextType =
        typeof (SC.Rules.Validators.ValidatorsRuleContext);
      PropertyInfo resultProperty = ruleContextType.GetProperty("Result");
      resultProperty.SetValue(
        ruleContext,
        resultValue,
        null /*index required only for indexed properties*/);

      if (!String.IsNullOrEmpty(ruleContext.Text))
      {
        return;
      }

      if (ruleContext.Parameters["Validation Message"] != null)
      {
        ruleContext.Text = ruleContext.Parameters["Validation Message"].ToString();
      }
      else if (!String.IsNullOrEmpty(this.Text))
      {
        ruleContext.Text = this.Text;
      }
      else
      {
        ruleContext.Text = "Failed rules-based validation";
      }
    }
  }
} 