namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor.Dialogs.TreeListExEditor
{
  using System;

  using SC = Sitecore;

  public class TreeListExEditorForm : Sitecore.Shell.Applications.ContentEditor.Dialogs.TreeListExEditor.TreeListExEditorForm
  {
    protected override void OnLoad(EventArgs e)
    {
      if (!Context.ClientPage.IsEvent)
      {
        SC.Web.UrlHandle handle = SC.Web.UrlHandle.Get();
        SC.Diagnostics.Assert.IsNotNull(handle, "handle");
        Sitecore.Sharedsource.Shell.Applications.ContentEditor.TreeList ssTreeList =
          this.TreeList as Sitecore.Sharedsource.Shell.Applications.ContentEditor.TreeList;
        SC.Diagnostics.Assert.IsNotNull(ssTreeList, "ssTreeList");
        ssTreeList.ItemID = handle["itemid"];
        ssTreeList.ItemLanguage = handle["itemlanguage"];
        ssTreeList.ItemVersion = handle["itemversion"];
      }

      base.OnLoad(e);
    }
  }
}