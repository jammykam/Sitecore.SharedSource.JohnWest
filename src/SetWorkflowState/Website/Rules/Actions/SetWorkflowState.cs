namespace Sitecore.Sharedsource.Rules.Actions
{
  using System;

  using Sitecore.Sharedsource.Data.Items;

  using SC = Sitecore;

  // rules engine action to set workflow state 
  // without invoking workflow actions associated with that state
  public class SetWorkflowState<T> :
    SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    public string WorkflowStateID { get; set; }

    public override void Apply(T ruleContext)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
      SC.Diagnostics.Assert.ArgumentNotNull(
        ruleContext.Item,
        "ruleContext.Item");

      if (ruleContext.Item.IsStandardValuesOrBranchTemplate())
      {
        return;
      }

      SC.Diagnostics.Assert.ArgumentNotNullOrEmpty(
        this.WorkflowStateID, 
        "WorkflowStateID");
      SC.Data.Items.Item workflowState = 
        ruleContext.Item.Database.GetItem(this.WorkflowStateID);
      SC.Diagnostics.Assert.IsNotNull(workflowState, "workflowState");
      SC.Data.Templates.Template checkTemplate =
        SC.Data.Managers.TemplateManager.GetTemplate(workflowState);
      SC.Diagnostics.Assert.IsTrue(
        checkTemplate.InheritsFrom(SC.TemplateIDs.WorkflowState),
        "workflow state template");

      // when placing an item in a workflow state,
      // ensure the item is also in that workflow
      SC.Data.Items.Item workflow = workflowState.GetAncestorWithTemplateThatIsOrInheritsFrom(
        SC.TemplateIDs.Workflow);

      if (workflow == null)
      {
        throw new Exception("Unable to determine workflow from workflow command");
      }

      if (ruleContext.Item[SC.FieldIDs.WorkflowState] ==  this.WorkflowStateID
        && ruleContext.Item[SC.FieldIDs.Workflow] == workflow.ID.ToString())
      {
        return;
      }

      using (new SC.Data.Items.EditContext(ruleContext.Item))
      {
        ruleContext.Item[SC.FieldIDs.WorkflowState] = this.WorkflowStateID;
        ruleContext.Item[SC.FieldIDs.Workflow] = workflow.ID.ToString();
      }
    }
  }
}