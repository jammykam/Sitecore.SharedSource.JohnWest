namespace Sitecore.Sharedsource.Rules.RuleMacros
{
  using System;
  using System.Xml.Linq;

  using Sitecore.Sharedsource.Data.Items;

  using SC = Sitecore;

  // rules engine macro to select a workflow state
  public class WorkflowStateMacro : SC.Rules.RuleMacros.IRuleMacro
  {
    public void Execute(
      XElement element, 
      string name, 
      Sitecore.Text.UrlString parameters, 
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
      SC.Data.Items.TemplateItem workflowStateTemplate = SC.Context.ContentDatabase.GetTemplate(
        SC.TemplateIDs.WorkflowState);
      SC.Diagnostics.Assert.IsNotNull(workflowStateTemplate, "workflowStateTemplate");
      options.IncludeTemplatesForSelection = SC.Shell.Applications.Dialogs.ItemLister.SelectItemOptions.GetTemplateList(
        workflowStateTemplate.GetSelfAndDerivedTemplates());
      options.Title = "Select Workflow State"; 
      options.Text = "Select the workflow state to use in this rule.";
      options.Icon = "Software/16x16/jar.png";
      SC.Web.UI.Sheer.SheerResponse.ShowModalDialog(
        options.ToUrlString().ToString(), 
        true);
    }
  }
}