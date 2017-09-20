namespace Sitecore.Sharedsource.Tasks
{
  using System;

  public class NewsMover
  {
    private Sitecore.Data.Database _database = null;
    private Sitecore.Data.Items.TemplateItem _articleTemplate = null;
    private Sitecore.Data.Items.TemplateItem _yearTemplate = null;
    private Sitecore.Data.Items.TemplateItem _monthTemplate = null;
    private Sitecore.Data.Items.TemplateItem _dayTemplate = null;

    public string Database
    {
      get;
      set;
    }

    public string YearTemplate
    {
      get;
      set;
    }

    public string MonthTemplate
    {
      get;
      set;
    }

    public string DayTemplate
    {
      get;
      set;
    }

    public string DateField
    {
      get;
      set;
    }

    public string ArticleTemplate
    {
      get;
      set;
    }

    public void OnItemSaved(object sender, EventArgs args)
    {
      this.ValidateAndSetParameters(args);
      Sitecore.Data.Items.Item article = this.GetArticle(args);

      if (String.Compare(article.Database.Name, this.Database) != 0
        || article.TemplateID != this._articleTemplate.ID
        || (this._articleTemplate.StandardValues != null
        && article.ID == this._articleTemplate.StandardValues.ID))
      {
        return;
      }

      DateTime articleDate = this.HandleDateField(article);
      this.HandleArticleDate(article, articleDate);
    }

    protected void ValidateAndSetParameters(EventArgs args)
    {
      this.ValidateAndSetDatabase();
      this.ValidateDateField();
      this.ValidateAndSetArticleTemplate();
      this.ValidateAndSetYearTemplate();
      this.ValidateAndSetMonthTemplate();
      this.ValidateAndSetDayTemplate();
    }

    protected void ValidateAndSetDatabase()
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(this.Database, "Database");
      this._database = Sitecore.Configuration.Factory.GetDatabase(
        this.Database);
      Sitecore.Diagnostics.Assert.IsNotNull(this._database, this.Database);
    }

    protected void ValidateDateField()
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(this.DateField, "DateField");
    }

    protected void ValidateAndSetArticleTemplate()
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(
        this.ArticleTemplate,
        "ArticleTemplate");
      this._articleTemplate =
        this._database.Templates[this.ArticleTemplate];
      Sitecore.Diagnostics.Assert.IsNotNull(
        this._articleTemplate,
        this.ArticleTemplate);
    }

    protected void ValidateAndSetYearTemplate()
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(
        this.YearTemplate,
        "YearTemplate");
      this._yearTemplate =
        this._database.Templates[this.YearTemplate];
      Sitecore.Diagnostics.Assert.IsNotNull(this._yearTemplate, this.YearTemplate);
    }

    protected Sitecore.Data.Items.Item GetArticle(EventArgs args)
    {
      Sitecore.Data.Items.Item article =
        Sitecore.Events.Event.ExtractParameter(args, 0) as Sitecore.Data.Items.Item;
      Sitecore.Diagnostics.Assert.ArgumentNotNull(article, "article");
      return article;
    }

    protected void ValidateAndSetMonthTemplate()
    {
      if (!String.IsNullOrEmpty(this.MonthTemplate))
      {
        this._monthTemplate = this._database.Templates[this.MonthTemplate];
        Sitecore.Diagnostics.Assert.IsNotNull(
          this._monthTemplate,
          this.MonthTemplate);
      }
    }

    protected void ValidateAndSetDayTemplate()
    {
      if (!String.IsNullOrEmpty(this.DayTemplate))
      {
        this._dayTemplate = this._database.Templates[this.DayTemplate];
        Sitecore.Diagnostics.Assert.IsNotNull(this._dayTemplate, this.DayTemplate);
      }

      Sitecore.Diagnostics.Assert.IsFalse(
        this._monthTemplate == null && this._dayTemplate != null,
        "dayTemplate without monthTemplate");
    }

    protected DateTime HandleDateField(Sitecore.Data.Items.Item article)
    {
      Sitecore.Data.Fields.DateField dateField = article.Fields[this.DateField];
      Sitecore.Diagnostics.Assert.IsNotNull(dateField, this.DateField);
      DateTime result = dateField.DateTime;

      if (String.IsNullOrEmpty(dateField.InnerField.Value))
      {
        using (new Sitecore.Data.Items.EditContext(article, true, false))
        {
          dateField.Value = Sitecore.DateUtil.IsoNow;
          result = Sitecore.DateUtil.IsoDateToDateTime(dateField.InnerField.Value);
        }
      }

      string sort = "-" + dateField.Value.Substring(4).Replace("T", String.Empty);

      if (article[Sitecore.FieldIDs.Sortorder] != sort)
      {
        using (new Sitecore.Data.Items.EditContext(article, true, true))
        {
          article[Sitecore.FieldIDs.Sortorder] = sort;
        }
      }

      return result;
    }

    protected void HandleArticleDate(
      Sitecore.Data.Items.Item article,
      DateTime articleDate)
    {
      Sitecore.Data.Items.Item root = this.GetNewsRoot(article);
      root = this.GetOrCreateChild(
        root,
        String.Format("{0:yyyy}", articleDate),
        this._yearTemplate,
        -articleDate.Year);

      if (this._monthTemplate != null)
      {
        root = this.GetOrCreateChild(
          root,
          String.Format("{0:MMMM}", articleDate),
          this._monthTemplate,
          -articleDate.Month);

        if (this._dayTemplate != null)
        {
          root = this.GetOrCreateChild(
            root,
            String.Format("{0:dd}", articleDate),
            this._dayTemplate,
            -articleDate.Day);
        }
      }

      if (String.Compare(article.Parent.Paths.FullPath, root.Paths.FullPath, true) == 0)
      {
        return;
      }

      Sitecore.Data.Items.Item orphaned = article.Parent;
      article.MoveTo(root);

      while ((!orphaned.HasChildren)
        && (orphaned.TemplateID == this._yearTemplate.ID
        || (this._monthTemplate != null && orphaned.TemplateID == this._monthTemplate.ID)
        || (this._dayTemplate != null && orphaned.Template.ID == this._dayTemplate.ID)))
      {
        Sitecore.Data.Items.Item parent = orphaned.Parent;
        orphaned.Delete();
        orphaned = parent;
      }

      if ((!Sitecore.Context.IsBackgroundThread) && Sitecore.Context.ClientPage.IsEvent)
      {
        string message = String.Format("item:refreshchildren(id={0})", article.Database.GetRootItem().ID);
        Sitecore.Context.ClientPage.SendMessage(this, message);
      }
    }

    protected Sitecore.Data.Items.Item GetNewsRoot(Sitecore.Data.Items.Item article)
    {
      Sitecore.Data.Items.Item root = article.Parent;

      while (root.TemplateID == this._yearTemplate.ID
        || root.TemplateID == this._articleTemplate.ID
        || (this._monthTemplate != null && root.TemplateID == this._monthTemplate.ID)
        || (this._dayTemplate != null && root.TemplateID == this._dayTemplate.ID))
      {
        root = root.Parent;
      }

      return root;
    }

    protected Sitecore.Data.Items.Item GetOrCreateChild(
      Sitecore.Data.Items.Item parent,
      string childName,
      Sitecore.Data.Items.TemplateItem template,
      int sortOrder)
    {
      Sitecore.Data.Items.Item child = parent.Children[childName];

      if (child == null)
      {
        child = parent.Add(childName, template);

        using (new Sitecore.Data.Items.EditContext(child))
        {
          child[Sitecore.FieldIDs.Sortorder] = sortOrder.ToString();
        }
      }

      return child;
    }
  }
}
