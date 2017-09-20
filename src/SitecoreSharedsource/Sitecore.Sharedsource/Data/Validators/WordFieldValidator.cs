namespace Sitecore.Sharedsource.Data.Validators.FieldValidators
{
  using System;
  using System.Runtime.Serialization;

  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Data.Validators;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;

  [Serializable]
  public class WordFieldValidator : StandardValidator
  {
    public WordFieldValidator(
      SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public WordFieldValidator()
    {
    }

    public override string Name
    {
      get
      {
        return Translate.Text("Word Field Validator");
      }
    }

    protected override ValidatorResult Evaluate()
    {
      string value = this.ControlValidationValue;

      if (string.IsNullOrEmpty(value))
      {
        return ValidatorResult.Valid;
      }

      Item item = Database.GetItem(this.ItemUri);
      Assert.IsNotNull(item, "item");
      FileDropAreaField fda = new FileDropAreaField(item.Fields[this.FieldID], value);
      ItemList mediaItems = fda.GetMediaItems();

      if (mediaItems == null || mediaItems.Count < 1 || mediaItems[0] == null)
      {
        return ValidatorResult.Valid;
      }

      if (mediaItems.Count > 1 || mediaItems[0]["extension"] != "docx")
      {
        string field = string.IsNullOrEmpty(
          fda.InnerField.Title) ? fda.InnerField.Name : fda.InnerField.Title;
        this.Text = Translate.Text(
          "{0} may contain only one Word document (.docx file).", 
          field);
        return this.GetMaxValidatorResult();
      }

      return ValidatorResult.Valid;
    }

    protected override ValidatorResult GetMaxValidatorResult()
    {
      return ValidatorResult.CriticalError;
    }
  }
}