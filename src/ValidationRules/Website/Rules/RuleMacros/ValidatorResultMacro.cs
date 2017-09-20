namespace Sitecore.Sharedsource.Rules.RuleMacros
{
  using System;
  using System.Xml.Linq;

  using Sitecore.Text;

  using SC = Sitecore;

  public class ValidatorResultMacro : SC.Rules.RuleMacros.IRuleMacro
  {
    public void Execute(
      XElement element, 
      string name, 
      UrlString parameters, string value)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(element, "element");
      SC.Diagnostics.Assert.ArgumentNotNull(name, "name");
      SC.Diagnostics.Assert.ArgumentNotNull(parameters, "parameters");
      SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
      SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions options =
        new SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions();

      if (!String.IsNullOrEmpty(value))
      {
        SC.Data.Items.Item item = SC.Client.ContentDatabase.GetItem(
          value);

        if (item != null)
        {
          options.SelectedItem = item;
        }
      }

      options.Root = SC.Client.ContentDatabase.GetItem(
        "/sitecore/system/Settings/Rules/Validation Rules/ValidatorResults");
      options.Title = "Select Validator Result";
      options.Text = "Select the validation result to use in this rule.";
      options.Icon = "Applications/16x16/preferences.png";
      options.ShowRoot = false;
      SC.Web.UI.Sheer.SheerResponse.ShowModalDialog(
        options.ToUrlString().ToString(), 
        true);
    }
  }
}