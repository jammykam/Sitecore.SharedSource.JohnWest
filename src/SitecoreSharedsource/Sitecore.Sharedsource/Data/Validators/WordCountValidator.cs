namespace Sitecore.Sharedsource.Data.Validators.FieldValidators
{
  using System;
  using System.Runtime.Serialization;
  using System.Xml;

  using DocumentFormat.OpenXml.Packaging;

  using Sitecore;
  using Sitecore.Data.Items;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;

  [Serializable]
  public class WordCountValidator : StandardValidator
  {
    public WordCountValidator(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public WordCountValidator()
    {
    }

    public override string Name
    {
      get
      {
        return Translate.Text("Word Word Count Field Validator");
      }
    }

    protected override ValidatorResult Evaluate()
    {
      int? maxWords = this.GetIntParam("MaxWords");
      int? minWords = this.GetIntParam("MinWords");
      int? minWarn = this.GetIntParam("MinWarn");
      int? maxWarn = this.GetIntParam("MaxWarn");

      if (minWarn == null && maxWarn == null && maxWords == null && minWords == null)
      {
        this.Text = Translate.Text("No thresholds defined for " + this.Name);
        return ValidatorResult.Warning;
      }

      if (string.IsNullOrEmpty(this.ControlValidationValue))
      {
        return ValidatorResult.Valid;
      }

      XmlDocument doc = new XmlDocument();
      doc.LoadXml(this.ControlValidationValue);
      Item folder = Sitecore.Context.ContentDatabase.GetItem(
        doc.DocumentElement.Attributes["mediaid"].Value);

      if (folder == null || !folder.HasChildren)
      {
        return ValidatorResult.Valid;
      }

      foreach (Item child in folder.Children)
      {
        MediaItem mediaItem = new MediaItem(child);

        using (WordprocessingDocument document = WordprocessingDocument.Open(
          mediaItem.GetMediaStream(),
          false /*isEditable*/))
        {
          int words = int.Parse(
            document.ExtendedFilePropertiesPart.Properties.Words.Text);

          if (minWords != null && words < minWords)
          {
            this.Text = Translate.Text(
              ".docx should contain at least {0} words (currently {1}).", 
              MainUtil.FormatLong(minWords.Value), 
              MainUtil.FormatLong(words));
            return this.GetMaxValidatorResult();
          }

          if (maxWords != null && words > maxWords)
          {
            this.Text = Translate.Text(
              ".docx should contain less than {0} words (currently {1}).", 
              MainUtil.FormatLong(maxWords.Value),
              MainUtil.FormatLong(words));
            return this.GetMaxValidatorResult();
          }

          if (minWarn != null && words > minWarn)
          {
            this.Text = Translate.Text(
              ".docx contains more than {0} words (currently {1}).",
              MainUtil.FormatLong(minWarn.Value),
              MainUtil.FormatLong(words));
            return ValidatorResult.Warning;
          }

          if (maxWarn == null || words < maxWarn)
          {
            continue;
          }

          this.Text = Translate.Text(
            ".docx file contains {0} words; approaching excessive length.", 
            MainUtil.FormatLong(words));

          if (maxWords != null)
          {
            this.Text += " " + Translate.Text(
              ".docx cannot contain more than {0} words.", 
              MainUtil.FormatLong(maxWords.Value));
          }

          return ValidatorResult.Warning;
        }
      }

      return ValidatorResult.Valid;
    }

    protected override ValidatorResult GetMaxValidatorResult()
    {
      return ValidatorResult.CriticalError;
    }

    private int? GetIntParam(string name)
    {
      Assert.ArgumentNotNullOrEmpty(name, "name");
      string text = this.Parameters[name];

      if (string.IsNullOrEmpty(text))
      {
        return null;
      }

      return int.Parse(text);
    }
  }
}