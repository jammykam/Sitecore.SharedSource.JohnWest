namespace Sitecore.Sharedsource.Rules.RuleMacros
{
  using System;
  using System.Xml.Linq;

  using Sitecore.Sharedsource.Data.Items;

  using SC = Sitecore;

  // rules engine macro to select a workflow command
  public class WorkflowCommandMacro : SC.Rules.RuleMacros.IRuleMacro
  {
    public void Execute(
      XElement element, 
      string name, 
      SC.Text.UrlString parameters, 
      string value)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(element, "element");
      SC.Diagnostics.Assert.ArgumentNotNull(name, "name");
      SC.Diagnostics.Assert.ArgumentNotNull(parameters, "parameters");
      SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
      SC.Diagnostics.Assert.IsNotNull(
        SC.Context.ContentDatabase, 
        "Sitecore.Context.ContentDatabase");
      SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions options = 
        new SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions();

      if (!String.IsNullOrEmpty(value))
      {
        SC.Data.Items.Item item = Client.ContentDatabase.GetItem(value);
        
        if (item != null)
        {
          options.SelectedItem = item;
        }
      }

      options.Root = Client.ContentDatabase.GetItem(SC.ItemIDs.WorkflowRoot);
      SC.Data.Items.TemplateItem workflowCommandTemplate = SC.Context.ContentDatabase.GetTemplate(
        SC.TemplateIDs.WorkflowCommand);
      SC.Diagnostics.Assert.IsNotNull(workflowCommandTemplate, "workflowCommandTemplate");
      options.IncludeTemplatesForSelection = SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions.GetTemplateList(
        workflowCommandTemplate.GetSelfAndDerivedTemplates());
      options.Title = "Select Workflow Command"; 
      options.Text = "Select the workflow command to use in this rule.";
      options.Icon = "Applications/16x16/nav_right_green.png";
      SC.Web.UI.Sheer.SheerResponse.ShowModalDialog(
        options.ToUrlString().ToString(), 
        true);
    }
  }
}