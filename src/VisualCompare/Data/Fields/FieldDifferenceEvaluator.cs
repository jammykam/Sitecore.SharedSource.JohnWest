namespace Sitecore.Sharedsource.Data.Fields
{
  using System;
  using System.Text;
  using Sitecore.Text.Diff;
  
  public class FieldDifferenceEvaluator
  {
    protected void Append(StringBuilder builder, string value, int index, int length)
    {
      this.Append(builder, value, index, length, null);
    }

    protected void Append(StringBuilder builder, string value, int index, int length, string color)
    {
      if (!String.IsNullOrEmpty(color))
      {
        builder.Append("<span style=\"color:" + color + "\">");
      }

      if (length > 0 && index >= 0)
      {
        builder.Append(StringUtil.Mid(value, index, length));
      }

      if (!String.IsNullOrEmpty(color))
      {
        builder.Append("</span>");
      }
    }

    public string GetDifferences(string first, string second)
    {
      DiffEngine engine = new DiffEngine();
      DiffListHtml source = new DiffListHtml(StringUtil.RemoveTags(first));
      DiffListHtml destination = new DiffListHtml(StringUtil.RemoveTags(second));
      engine.ProcessDiff(source, destination, DiffEngineLevel.SlowPerfect);
      StringBuilder builder = new StringBuilder();

      foreach (DiffResultSpan span in engine.DiffReport())
      {
        if (span != null)
        {
          switch (span.Status)
          {
            case DiffResultSpanStatus.NoChange:
              this.Append(builder, first, span.SourceIndex, span.Length);
              break;
            case DiffResultSpanStatus.Replace:
              this.Append(
                builder,
                first,
                span.SourceIndex,
                span.Length,
                "green");
              this.Append(
                builder,
                second,
                span.DestIndex,
                span.Length,
                "red;text-decoration:line-through;font-weight:bold");
              break;
            case DiffResultSpanStatus.DeleteSource:
              this.Append(
                builder,
                first,
                span.SourceIndex,
                span.Length,
                "green;font-weight:bold");
              break;
            case DiffResultSpanStatus.AddDestination:
              this.Append(
                builder,
                second,
                span.DestIndex,
                span.Length,
                "red;text-decoration:line-through;font-weight:bold");
              break;
          }
        }
      }

      return builder.ToString();
    }
  }
}