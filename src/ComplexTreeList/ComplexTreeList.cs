namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Web.UI;

  using Assert = Sitecore.Diagnostics.Assert;
  using SC = Sitecore;

  public class TreeList : SC.Shell.Applications.ContentEditor.TreeList
  {
    // have we already parsed the Source property?
    private bool parsedSource;

    // the item to select by default in the selection tree
    private string selectFirstItem;

    // the database containing the data source for the treelist
    private SC.Data.Database database;

    // token indicates how to sort the selected items
    protected string SortBy
    {
      get
      {
        return this.GetViewStateString("SortBy");
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.SetViewStateString("SortBy", value);
      }
    }

    // token indicates value to show for items in list of selected items
    protected string Display
    {
      get
      {
        return this.GetViewStateString("Display");
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.SetViewStateString("Display", value);
      }
    }

    // custom logic starts here
    protected override void OnLoad(EventArgs args)
    {
      // if we have not already parsed the Source property
      // and the Source property has a value
      // rewrite it as appropriate before invoking default OnLoad()
      if (!(this.parsedSource
        || string.IsNullOrEmpty(this.Source)))
      {
        this.ProcessSource(this.Source.Trim());
      }

      base.OnLoad(args);

      if (!(this.parsedSource
        || string.IsNullOrEmpty(this.Source)))
      {
        // check if the sort changed anything
        string value = this.Value;
        this.SortSelected();

        if (this.GetValue() != value)
        {
          this.SetValue(this.GetValue());
          SC.Shell.Applications.ContentEditor.TreeList.SetModified();
        }

        if (!string.IsNullOrEmpty(this.selectFirstItem))
        {
          this.SelectFirstItem(this.selectFirstItem);
        }
      }

      this.parsedSource = true;
    }

    // rewrite the Source property
    // set selectFirstItem now rather than parsing again later
    protected void ProcessSource(string originalSource)
    {
      if ((!string.IsNullOrEmpty(originalSource)) 
        && this.Source.Contains("{itemfield:"))
      {
        MatchCollection matches = Regex.Matches(
          originalSource, 
          "(?<match>\\{itemfield\\:(?<field>[^\\}]+)})");
        Assert.IsTrue(matches.Count > 0, "matches > 0");

        foreach(Match match in matches)
        {
          foreach (Group group in match.Groups)
          {
            originalSource = originalSource.Replace(
              match.Groups["match"].Value,
              this.CurrentItem[match.Groups["field"].Value]);
          }
        }
      }

      // If the Source contains a single value, convert it to a 
      // key=value pair. 
      if (originalSource.StartsWith("query:")
        || originalSource.StartsWith("~") 
        || originalSource.StartsWith("/")
        || originalSource.StartsWith("."))
      {
        originalSource = "DataSource=" + originalSource;
      }

      SC.Collections.SafeDictionary<string> parameters = 
        SC.Web.WebUtil.ParseQueryString(originalSource);
      string databaseName = parameters["DatabaseName"];

      if (String.IsNullOrEmpty(databaseName))
      {
        this.database = this.CurrentItem.Database;
      }
      else
      {
        this.database = SC.Configuration.Factory.GetDatabase(databaseName);
        SC.Diagnostics.Assert.IsNotNull(this.database, "database");
      }

      string dataSource = parameters["DataSource"];

      if (!(string.IsNullOrEmpty(dataSource) || dataSource.StartsWith("~")))
      {
        parameters["DataSource"] = this.ExpandDataSource(dataSource);
      }

      string sortBy = parameters["SortBy"];

      if (!string.IsNullOrEmpty(sortBy))
      {
        this.SortBy = sortBy;
        parameters.Remove("SortBy");
      }

      string display = parameters["Display"];
      
      if (!string.IsNullOrEmpty(display))
      {
        this.Display = display;
        parameters.Remove("Display");
      }

      string displayInTree = parameters["DisplayInTree"];

      if (displayInTree != null)
      {
        this.DisplayFieldName = displayInTree;
        parameters.Remove("DisplayInTree");
      }

      string includeItemsForDisplay = parameters["IncludeItemsForDisplay"];

      if ((!string.IsNullOrEmpty(includeItemsForDisplay) 
        && includeItemsForDisplay.StartsWith("query:")))
      {
        parameters["IncludeItemsForDisplay"] =
          this.CalculateIncludeItemsForDisplay(includeItemsForDisplay);
      }

      this.selectFirstItem = parameters["SelectFirstItem"];
      this.Source = SC.Web.WebUtil.BuildQueryString(parameters, false);
    }

    // find the data context to allow specification 
    // of the initial selection in the tree
    protected SC.Web.UI.HtmlControls.DataContext FindDataContext(
      Control control)
    {
      SC.Web.UI.HtmlControls.DataContext dataContext = control as SC.Web.UI.HtmlControls.DataContext;

      if (dataContext != null)
      {
        return dataContext;
      }

      foreach (Control childControl in control.Controls)
      {
        dataContext = this.FindDataContext(childControl);

        if (dataContext != null)
        {
          return dataContext;
        }
      }

      return null;
    }

    // update the data context to specify the initial selection in the tree
    protected void SelectFirstItem(string idPathOrQuery)
    {
      SC.Data.Items.Item[] queried = this.DetermineItems(idPathOrQuery);

      if (queried != null && queried.Length > 0 && queried[0] != null)
      {
        SC.Web.UI.WebControls.TreeviewEx tree = 
          this.FindControl(this.ID + "_all") as SC.Web.UI.WebControls.TreeviewEx;
        SC.Diagnostics.Assert.IsNotNull(tree, "tree");
        SC.Web.UI.HtmlControls.DataContext dataContext = 
          this.FindDataContext(this);
        SC.Diagnostics.Assert.IsNotNull(dataContext, "dataContext");
        dataContext.Folder = queried[0].Paths.FullPath;
      }
    }

    // convert IDs, paths, relative paths, queries, fast queries, 
    // and relative fast queries in data sources to explicit paths
    protected string ExpandDataSource(string dataSource)
    {
      if (dataSource.StartsWith("~"))
      {
        return dataSource;
      }

      SC.Data.Items.Item[] queried = this.DetermineItems(dataSource);

      // TOOD: may want to handle these cases differently
      SC.Diagnostics.Assert.IsNotNull(queried, dataSource);
//      SC.Diagnostics.Assert.IsTrue(queried.Length == 1, "queried count");
//      SC.Diagnostics.Assert.IsNotNull(queried[0], "queried[0]");
      return queried != null && queried.Length > 0 ? queried[0].Paths.FullPath : null;
    }

    // convert IDs, paths, relative paths, queries, fast queries,
    // and relative fast queries to lists of items 
    protected SC.Data.Items.Item[] DetermineItems(string dataSoruce)
    {
      SC.Data.Items.Item item = this.CurrentItem;
      SC.Data.Items.Item[] queried;

      if (dataSoruce.StartsWith("query:fast:"))
      {
        dataSoruce = dataSoruce.Substring("query:fast:".Length);
        SC.Diagnostics.Assert.IsFalse(
          dataSoruce.StartsWith(".") && this.database.Name != item.Database.Name,
          "relative fast queries cannot specify database");

        while (dataSoruce.StartsWith("../"))
        {
          item = item.Parent;
          dataSoruce = dataSoruce.Substring("../".Length);
        }

        if (dataSoruce.StartsWith("./"))
        {
          dataSoruce = dataSoruce.Substring("./".Length);
        }

        dataSoruce = "fast:" + item.Paths.FullPath + '/' + dataSoruce;
        queried = this.database.SelectItems(dataSoruce);
      }
      else
      {
        if (dataSoruce.StartsWith("query:"))
        {
          dataSoruce = dataSoruce.Substring("query:".Length);
        }

        SC.Diagnostics.Assert.IsFalse(
          dataSoruce.StartsWith(".") && this.database.Name != item.Database.Name,
          "relative queries cannot specify database");
        queried = this.database.SelectItems(dataSoruce);
      }

      return queried;
    }

    // sort the list of selected items
    protected void SortSelected()
    {
      // we need to know whether and by what to sort
      if (string.IsNullOrEmpty(this.SortBy))
      {
        return;
      }

      // get the list of selected items
      string viewStateString = this.GetViewStateString("ID");
      SC.Web.UI.HtmlControls.Listbox listbox = this.FindControl(
        viewStateString + "_selected") as SC.Web.UI.HtmlControls.Listbox;
      SC.Diagnostics.Assert.IsNotNull(
        listbox,
        typeof(SC.Web.UI.HtmlControls.Listbox));

      // temporary list of items to sort
      List<SC.Web.UI.HtmlControls.ListItem> list =
        new List<SC.Web.UI.HtmlControls.ListItem>();

      // populate temporary list from UI list
      for (int i = 0; i < listbox.Controls.Count; i++)
      {
        SC.Web.UI.HtmlControls.ListItem listItem =
          listbox.Controls[i] as SC.Web.UI.HtmlControls.ListItem;
        SC.Diagnostics.Assert.IsNotNull(
          listItem,
          typeof(SC.Web.UI.HtmlControls.ListItem));
        list.Add(listItem);
      }

      // clear the UI list
      while (listbox.Controls.Count > 0)
      {
        listbox.Controls.RemoveAt(0);
      }

      // sort the temporary list
      list.Sort(this.Compare);

      // add the sorted list to the UI
      foreach (SC.Web.UI.HtmlControls.ListItem listItem in list)
      {
        listbox.Controls.Add(listItem);
      }

      // I couldn't find a case that required this, 
      // but I included it in case you find such a case.
      SC.Web.UI.Sheer.SheerResponse.Refresh(listbox);
    }

    // comparison method for sorting selected items
    protected int Compare(
      SC.Web.UI.HtmlControls.ListItem listItemA,
      SC.Web.UI.HtmlControls.ListItem listItemB)
    {
      SC.Diagnostics.Assert.IsNotNull(listItemA, "listItemA");
      SC.Diagnostics.Assert.IsNotNull(listItemB, "listItemB");

      // not too keen on this format
      string[] split = listItemA.Value.Split(new char[] { '|' });

      if (split.Length != 2)
      {
        return 1;
      }

      SC.Data.Items.Item itemA = this.CurrentItem.Database.GetItem(
        split[1], 
        this.CurrentItem.Language);

      if (itemA == null)
      {
        return 1;
      }

      split = listItemB.Value.Split(new char[] { '|' });

      if (split.Length != 2)
      {
        return -1;
      }

      SC.Data.Items.Item itemB = this.CurrentItem.Database.GetItem(
        split[1],
        this.CurrentItem.Language);

      if (itemB == null)
      {
        return -1;
      }
      else if (this.SortBy == "@displayname")
      {
        return string.Compare(
          itemA.DisplayName, 
          itemB.DisplayName, 
          StringComparison.InvariantCultureIgnoreCase);
      }
      else if (this.SortBy == "@path")
      {
        return string.Compare(
          itemA.Paths.FullPath, 
          itemB.Paths.FullPath, 
          StringComparison.InvariantCultureIgnoreCase);
      }
      else if (this.SortBy == "@name")
      {
        return string.Compare(
          itemA.Name, 
          itemB.Name, 
          StringComparison.InvariantCultureIgnoreCase);
      }

      return string.Compare(
        itemA[this.SortBy], 
        itemB[this.SortBy], 
        StringComparison.InvariantCultureIgnoreCase);
    }

    // the user clicks the down arrow
    protected new void Down()
    {
      if (string.IsNullOrEmpty(this.SortBy))
      {
        base.Down();
      }
      else
      {
        SC.Web.UI.Sheer.SheerResponse.Alert(
          SC.Globalization.Translate.Text("These items sort automatically."), new string[0]);
      }
    }

    // the user clicks the up arrow
    protected new void Up()
    {
      if (string.IsNullOrEmpty(this.SortBy))
      {
        base.Up();
      }
      else
      {
        SC.Web.UI.Sheer.SheerResponse.Alert(
          SC.Globalization.Translate.Text("These items sort automatically."), new string[0]);
      }
    }

    // the user adds an item to the selection
    protected new void Add()
    {
      base.Add();
      this.SortSelected();
    }

    // determine the value to show for an item in the list of selected items
    protected override string GetHeaderValue(
      SC.Data.Items.Item item)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(item, "item");
      string display = item.DisplayName;

      if (!string.IsNullOrEmpty(this.Display))
      {
        if (this.Display == "@name")
        {
          display = item.Name;
        }
        else if (this.Display == "@path")
        {
          display = item.Paths.FullPath;
        }
        else if (!string.IsNullOrEmpty(item[this.Display]))
        {
          display = item[this.Display];
        }
      }

      return display;
    }

    // expand query in IncludeItemsForDisplay to list of IDs
    protected string CalculateIncludeItemsForDisplay(string value)
    {
      if (!value.StartsWith("query:"))
      {
        return value;
      }

      SC.Data.Items.Item[] queried = this.DetermineItems(value);

      if (queried == null)
      {
        queried = new SC.Data.Items.Item[] { this.CurrentItem.Database.GetRootItem() };
      }

      string result = string.Empty;

      // in order to show an item, we may have to show its ancestors
      foreach (SC.Data.Items.Item looper in queried)
      {
        result += looper.ID.ToString() + ',';

        foreach (SC.Data.Items.Item ancestor
          in looper.Axes.GetAncestors())
        {
          if (!result.Contains(ancestor.ID.ToString()))
          {
            result += ancestor.ID + ",";
          }
        }
      }

      return result.TrimEnd(',');
    }

    private SC.Data.Items.Item _currentItem;

    public new string ItemID
    {
      get
      {
        return this.GetViewStateString("ItemID");
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.SetViewStateString("ItemID", value);
        base.ItemID = value;
      }
    }

    public string ItemVersion
    {
      get
      {
        return this.GetViewStateString("ItemVersion");
      }

      set
      {
        SC.Diagnostics.Assert.ArgumentNotNull(value, "value");
        this.SetViewStateString("ItemVersion", value);
      }
    }

    [NotNull]
    protected SC.Data.Items.Item CurrentItem
    {
      get
      {
        if (this._currentItem == null)
        {
          this._currentItem = SC.Context.ContentDatabase.GetItem(
            this.ItemID,
            SC.Globalization.Language.Parse(this.ItemLanguage),
            SC.Data.Version.Parse(this.ItemVersion));
          SC.Diagnostics.Assert.IsNotNull(this._currentItem, "item");
        }

        return this._currentItem;
      }
    }
  }
}