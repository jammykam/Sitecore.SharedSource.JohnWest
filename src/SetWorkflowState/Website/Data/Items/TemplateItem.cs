namespace Sitecore.Sharedsource.Data.Items
{
  using System;
  using System.Collections.Generic;

  using SC = Sitecore;

  public static class TemplateItem
  {
    // return the ID of the template
    // and any templates that inherit from that template
    public static string[] GetSelfAndDerivedTemplates(
      this SC.Data.Items.TemplateItem template)
    {
      SC.Diagnostics.Assert.IsNotNull(template, "template");
      List<string> results = new List<string>();
      results.Add(template.ID.ToString());
      SC.Links.ItemLink[] links = SC.Globals.LinkDatabase.GetReferrers(
        template);

      if (links != null)
      {
        foreach (SC.Links.ItemLink link in links)
        {
          if (link.SourceFieldID != SC.FieldIDs.BaseTemplate)
          {
            continue;
          }

          if (String.Compare(
            link.SourceDatabaseName,
            template.Database.Name,
            StringComparison.OrdinalIgnoreCase) != 0)
          {
            continue;
          }

          SC.Data.Items.Item referrer = template.Database.GetItem(link.SourceItemID);

          if (referrer != null
            && referrer[SC.FieldIDs.BaseTemplate].Contains(template.ID.ToString()))
          {
            results.Add(link.SourceItemID.ToString());
          }
        }

        // or replace that foreach with something like this if you prefer it...
        /*
        results.AddRange(from link in links
                         where link.SourceFieldID == SC.FieldIDs.BaseTemplate
                         where String.Compare(link.SourceDatabaseName, template.Database.Name, StringComparison.OrdinalIgnoreCase) == 0
                         let referrer = template.Database.GetItem(link.SourceItemID)
                         where referrer != null && referrer[SC.FieldIDs.BaseTemplate].Contains(template.ID.ToString())
                         select link.SourceItemID.ToString());
        */
      }

      return results.ToArray();
    }
  }
}