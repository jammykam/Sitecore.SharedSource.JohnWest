namespace Sitecore.Sharedsource.Rules.Actions
{
  using System;

  using Sitecore.Sharedsource.Data.Items;

  using SC = Sitecore;

  // rules engine action to apply a workflow command 
  // to update the workflow state,
  // which invokes actions associated with the command
  // and the subsequent state
  public class ApplyWorkflowCommand<T> :
    SC.Rules.Actions.RuleAction<T>
    where T : SC.Rules.RuleContext
  {
    public string WorkflowCommandID { get; set; }

    public string Comment { get; set; }

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
        this.WorkflowCommandID,
        "WorkflowCommandID");
      SC.Diagnostics.Assert.ArgumentNotNullOrEmpty(
        this.Comment,
        "Comment");
      SC.Data.Items.Item workflowCommand =
        ruleContext.Item.Database.GetItem(this.WorkflowCommandID);
      SC.Diagnostics.Assert.IsNotNull(workflowCommand, "workflowCommand");
      SC.Data.Templates.Template checkTemplate =
        SC.Data.Managers.TemplateManager.GetTemplate(workflowCommand);
      SC.Diagnostics.Assert.IsTrue(
        checkTemplate.InheritsFrom(SC.TemplateIDs.WorkflowCommand),
        "workflow command template");
      SC.Data.Items.Item commandWorkflow = workflowCommand.GetAncestorWithTemplateThatIsOrInheritsFrom(
        SC.TemplateIDs.Workflow);

      // before invoking a workflow command,
      // put the item in the appropriate workflow and workflow state
      if (commandWorkflow == null)
      {
        throw new Exception("Unable to determine workflow from workflow command");
      }

      SC.Data.Items.Item commandState = workflowCommand.GetAncestorWithTemplateThatIsOrInheritsFrom(
        SC.TemplateIDs.WorkflowState);

      if (commandState == null)
      {
        throw new Exception("Unable to determine workflow state from workflow command");
      }

      if (ruleContext.Item[SC.FieldIDs.Workflow] != commandWorkflow.ID.ToString())
      {
        using (new SC.Data.Items.EditContext(ruleContext.Item))
        {
          ruleContext.Item[SC.FieldIDs.Workflow] = commandWorkflow.ID.ToString();
        }
      }
      
      if (ruleContext.Item[SC.FieldIDs.WorkflowState] != commandState.ID.ToString())
      {
        using (new SC.Data.Items.EditContext(ruleContext.Item))
        {
          ruleContext.Item[SC.FieldIDs.WorkflowState] = commandState.ID.ToString();
        }
      }

      SC.Workflows.IWorkflow workflow = ruleContext.Item.State.GetWorkflow();
      SC.Diagnostics.Assert.IsNotNull(workflow, "workflow");
      workflow.Execute(
        this.WorkflowCommandID,
        ruleContext.Item,
        this.Comment,
        false, 
        new object[] {});
    }
  }
}