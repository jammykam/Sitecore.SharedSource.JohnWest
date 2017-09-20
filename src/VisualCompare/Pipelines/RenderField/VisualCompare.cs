namespace Sitecore.Sharedsource.Pipelines.RenderField
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Pipelines.RenderField;
  using Sitecore.Sharedsource.Data.Fields;

  public class VisualCompare
  {
    private List<string> _excludeFieldTypes;

    public List<string> ExcludeFieldTypes
    { 
      get
      {
        if (this._excludeFieldTypes == null)
        {
          this._excludeFieldTypes = new List<string>();
        }

        return this._excludeFieldTypes;
      }
    }
 
    public string SourceDB
    {
      get;
      set;
    }

    public string TargetDB
    {
      get; 
      set;
    }

    public bool RenderTextualDifferences
    {
      get; 
      set; 
    }

    public bool RenderBeforeComparison
    {
      get; 
      set;
    }

    public bool IndicateUnchangedFields
    {
      get;
      set;
    }

    protected string RenderWithoutDifferences(Item item, RenderFieldArgs args)
    {
      CustomRenderFieldArgs diffArgs = new CustomRenderFieldArgs()
      {
        Item = item,
        FieldName = args.FieldName,
        Parameters = args.Parameters,
        RawParameters = args.RawParameters,
        RenderParameters = args.RenderParameters,
        DisableWebEdit = args.DisableWebEdit,
      };

      CorePipeline.Run("renderField", diffArgs);
      return diffArgs.Result.ToString();
    }

    protected Item GetItem(RenderFieldArgs args)
    {
      if (args is CustomRenderFieldArgs
        || args.Item.Database.Name != this.SourceDB
        || this.ExcludeFieldTypes.Contains(args.FieldTypeKey)
        || (!Sitecore.Context.PageMode.IsPreview)
        || (!MainUtil.GetBool(Sitecore.Web.WebUtil.GetQueryString("sc_compare"), false)))
      {
        return null;
      }

      Database target = Configuration.Factory.GetDatabase(this.TargetDB);
      Assert.IsNotNull(target, "target");
      Item item = target.GetItem(args.Item.ID, args.Item.Language);
      return item == null || item.Statistics.Revision == args.Item.Statistics.Revision ? null : item;
    }

    protected string GetColor(bool fieldChanged, bool isContextItem)
    {
      if (isContextItem)
      {
        if (fieldChanged)
        {
          return "red";
        }
        else
        {
          return "green";
        }
      }
      else
      {
        if (fieldChanged)
        {
          return "blue";
        }
      }

      return "black";
    }

    public void Process(RenderFieldArgs args)
    {
      Item item = this.GetItem(args);

      if (item == null)
      {
        return;
      }

      string oldText = item[args.FieldName];
      string updatedText = args.Item[args.FieldName];
      bool fieldChanged = true;

      if (String.Compare(updatedText, oldText, false) == 0)
      {
        fieldChanged = false;

        if (!this.IndicateUnchangedFields)
        {
          return;
        }
      }

      if (this.RenderBeforeComparison)
      {
        oldText = this.RenderWithoutDifferences(item, args);
        updatedText = this.RenderWithoutDifferences(args.Item, args);
      }

      if (this.RenderTextualDifferences && args.FieldTypeKey != "image")
      {
        FieldDifferenceEvaluator evaluator = new FieldDifferenceEvaluator();
        args.Result.FirstPart = evaluator.GetDifferences(
          updatedText,
          oldText);
      }

      args.Result.FirstPart = @"<div style=""border: 1px " 
        + this.GetColor(fieldChanged, Sitecore.Context.Item.ID == args.Item.ID)
        + @" solid; margin: -1px;"">" 
        + args.Result.FirstPart;
      args.Result.LastPart += "</div>";
    }
  }
}