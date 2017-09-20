namespace Sitecore.Sharedsource.Data.Items
{
  using System;
  using System.Collections.Generic;

  using SC = Sitecore;

  public static class Item
  {
    // return true if the item is the standard values definition item 
    // for its data template or part of a branch template definition
    public static bool IsStandardValuesOrBranchTemplate(
      this SC.Data.Items.Item item)
    {
      if (item.Template.StandardValues != null
        && item.ID == item.Template.StandardValues.ID)
      {
        return true;
      }

      string query = String.Format(
        "ancestor-or-self::*[@@templateid='{0}']",
        SC.TemplateIDs.BranchTemplate.ToString());
      return item.Axes.SelectSingleItem(query) != null;
    }

    // return the item or its nearest ancestor that uses a data template
    // or a template that inherits from that template
    public static SC.Data.Items.Item GetAncestorWithTemplateThatIsOrInheritsFrom(
      this SC.Data.Items.Item item,
      SC.Data.Items.TemplateItem template)
    {
      return (GetAncestorWithTemplateThatIsOrInheritsFrom(item, template.ID));
    }

    public static SC.Data.Items.Item GetAncestorWithTemplateThatIsOrInheritsFrom(
      this SC.Data.Items.Item item,
      SC.Data.ID templateID)
    {
      string id = templateID.ToString();

      while (item != null)
      {
        string[] templates = item.Template.GetSelfAndDerivedTemplates();

        if (new List<string>(templates).Contains(id))
        {
          return item;
        }

        item = item.Parent;
      }

      return null;
    }
  }
}