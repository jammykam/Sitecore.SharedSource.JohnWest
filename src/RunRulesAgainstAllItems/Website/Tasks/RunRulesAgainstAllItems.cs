namespace Sitecore.Sharedsource.Tasks
{
  using System;
  using System.Collections.Generic;

  using SC = Sitecore;

  public class RunRulesAgainstAllItems
  {
    // supports the Rules property
    private List<string> _rules = new List<string>();
 
    // database containing items to process (defaults to Master)
    public string Database { get; set; }

    // query to retrieve items (defaults to all items)
    public string Query { get; set; }

    // process default version of items in all languages
    public bool AllLanguages { get; set; }

    // process all versions of items in relevant languages
    public bool AllVersions { get; set; }

    // rules to invoke (defaults to all under the 
    // /sitecore/system/Settings/Rules/Global Item Rules/Rules item)
    public List<string> Rules
    {
      get
      {
        return this._rules;
      }
    }

    // agent body
    public void Run()
    {
      // database containing items to process
      SC.Data.Database db = this.GetDatabase();
      SC.Diagnostics.Assert.IsNotNull(db, "db");

      // ids of items to process
      SC.Data.ID[] itemIDs = this.GetItemIDs(db);
      SC.Diagnostics.Assert.IsNotNull(itemIDs, "itemIDs");

      if (SC.Context.Job != null)
      {
        SC.Context.Job.Status.Total = itemIDs.Length;
      }

      // rules to invoke
      SC.Rules.RuleList<SC.Rules.RuleContext> rules = this.GetRules(db);
      SC.Diagnostics.Assert.IsNotNull(rules, "rules");

      // execute rules against each item
      foreach (SC.Data.ID id in itemIDs)
      {
        SC.Data.Items.Item[] versions = this.GetVersionsToProcess(
          db, 
          id);

        if (SC.Context.Job != null && versions.Length > 0)
        {
          SC.Context.Job.Status.Messages.Add(
            "Processing " + versions[0].Paths.FullPath);
        }
        
        foreach (SC.Data.Items.Item version in versions)
        {
          SC.Rules.RuleContext ruleContext = new SC.Rules.RuleContext();
          ruleContext.Item = version;
          rules.Run(ruleContext);
        }

        if (SC.Context.Job != null)
        {
          SC.Context.Job.Status.Processed++;
        }
      }
    }

    // retrieve database containing items to process
    protected SC.Data.Database GetDatabase()
    {
      string dbName = "master";

      if (!String.IsNullOrEmpty(this.Database))
      {
        dbName = this.Database;
      }

      return SC.Configuration.Factory.GetDatabase(dbName);
    }

    // retrieve IDs of items to process
    protected SC.Data.ID[] GetItemIDs(SC.Data.Database db)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(db, "db");
      SC.Data.Query.Query query = new SC.Data.Query.Query(
        this.GetQuery())
      {
        // override the Query.MaxItems setting in the Web.config file
        Max = int.MaxValue
      };

      SC.Data.Items.Item root = db.GetRootItem();
      SC.Diagnostics.Assert.IsNotNull(root, "root");

      // result of invoking the query
      object queryResult = query.Execute(root);
      SC.Diagnostics.Assert.IsNotNull(queryResult, "queryResult");

      // list for method to return
      List<SC.Data.ID> methodResults = new List<SC.Data.ID>();
      SC.Data.Query.QueryContext queryContext =
        queryResult as SC.Data.Query.QueryContext;

      // if the query returned only one item
      if (queryContext != null)
      {
        methodResults.Add(queryContext.ID);
      }
      else
      {
        // the query potentially returned multiple results
        SC.Data.Query.QueryContext[] queryContexts =
          queryResult as SC.Data.Query.QueryContext[];
        SC.Diagnostics.Assert.IsNotNull(queryContexts, "queryContexts");

        for (int i = 0; i < queryContexts.Length; i++)
        {
          methodResults.Add(queryContexts[i].ID);
        }
      }

      return methodResults.ToArray();
    }

    // retrieve the query that identifies the items
    protected string GetQuery()
    {
      string query = "//*";

      if (!String.IsNullOrEmpty(this.Query))
      {
        query = this.Query;
      }

      return query;
    }

    // retrieve the rules to invoke
    protected SC.Rules.RuleList<SC.Rules.RuleContext> GetRules(
      SC.Data.Database db)
    {
      // items that define the rules to invoke
      List<SC.Data.Items.Item> ruleItems =
        new List<SC.Data.Items.Item>();

      // no rules specified to configuration factory; apply all global rules
      if (this.Rules.Count < 1)
      {
        SC.Data.Items.Item ruleRoot =
          db.GetItem("/sitecore/system/Settings/Rules/Global Item Rules/Rules");
        SC.Diagnostics.Assert.IsNotNull(ruleRoot, "ruleRoot");
        ruleItems.AddRange(ruleRoot.Children);
      }
      else
      {
        foreach (string id in this.Rules)
        {
          SC.Data.Items.Item ruleItem = db.GetItem(id);
          SC.Diagnostics.Assert.IsNotNull(ruleItem, id);
          ruleItems.Add(ruleItem);
        }
      }

      SC.Rules.RuleList<SC.Rules.RuleContext> rules =
        new SC.Rules.RuleList<SC.Rules.RuleContext>();
      rules.Name = "All Global Item Rules";

      foreach (SC.Data.Items.Item ruleItem in ruleItems)
      {
        string ruleXml = ruleItem["Rule"];

        if (String.IsNullOrEmpty(ruleXml) || ruleItem["Disabled"] == "1")
        {
          continue;
        }

        SC.Rules.RuleList<SC.Rules.RuleContext> parsed = SC.Rules.RuleFactory.ParseRules<SC.Rules.RuleContext>(
          db,
          ruleXml);
        rules.AddRange(parsed.Rules);
      }

      return rules;
    }

    // retrieve the versions of the item to process in the relevant language(s)
    protected SC.Data.Items.Item[] GetVersionsToProcess(
      SC.Data.Database db,
      SC.Data.ID id)
    {
      // default version of the item in default language
      SC.Data.Items.Item item = db.GetItem(id);
      SC.Diagnostics.Assert.IsNotNull(item, "item");

      // all versions of the item to process in all languages to process
      List<SC.Data.Items.Item> versions =
        new List<SC.Data.Items.Item>();

      // languages to process
      List<SC.Globalization.Language> languages =
        new List<SC.Globalization.Language>();

      if (this.AllLanguages)
      {
        languages.AddRange(item.Languages);
      }
      else
      {
        languages.Add(item.Language);
      }

      foreach (SC.Globalization.Language language in languages)
      {
        // default version of item in individual language
        SC.Data.Items.Item languageItem = db.GetItem(id, language);
        SC.Diagnostics.Assert.IsNotNull(languageItem, "languageItem");

        if (this.AllVersions)
        {
          versions.AddRange(languageItem.Versions.GetVersions());
        }
        else
        {
          versions.Add(languageItem);
        }
      }

      return versions.ToArray();
    }
  }
}