namespace Sitecore.Sharedsource.Data.Validators.ItemValidators
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class SubsequentDateTimeValidator : Sitecore.Data.Validators.StandardValidator
  {
    public SubsequentDateTimeValidator()
      : base()
    {
    }

    public SubsequentDateTimeValidator(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string Name
    {
      get
      {
        return Sitecore.StringUtil.GetString(this.Parameters["ValidatorName"], this.GetType().ToString());
      }
    }

    protected override Sitecore.Data.Validators.ValidatorResult GetMaxValidatorResult()
    {
      return GetFailedResult(Sitecore.Data.Validators.ValidatorResult.Error);
    }

    protected override Sitecore.Data.Validators.ValidatorResult Evaluate()
    {
      Sitecore.Data.Items.Item item = this.GetItem();
      string firstFieldName = Sitecore.StringUtil.GetString(this.Parameters["FirstFieldName"], "StartDate");
      string lastFieldName = Sitecore.StringUtil.GetString(this.Parameters["LastFieldName"], "EndDate");
      Sitecore.Data.Fields.DateField first = item.Fields[firstFieldName];
      Sitecore.Data.Fields.DateField last = item.Fields[lastFieldName];

      if (first == null)
      {
        this.Text = firstFieldName + " does not exist.";
        return Sitecore.Data.Validators.ValidatorResult.Warning;
      }

      if (last == null)
      {
        this.Text = lastFieldName + " does not exist.";
        return Sitecore.Data.Validators.ValidatorResult.Warning;
      }

      if (item.Template.StandardValues != null && item.Template.StandardValues.ID == item.ID)
      {
        return Sitecore.Data.Validators.ValidatorResult.Valid;
      }

      if (DateTime.Compare(first.DateTime, last.DateTime) >= 0)
      {
        string firstTitle = Sitecore.StringUtil.GetString(first.InnerField.Title, first.InnerField.Name);
        string lastTitle = Sitecore.StringUtil.GetString(last.InnerField.Title, last.InnerField.Name);
        this.Text = String.Format("{0} must be greater than {1}.", firstTitle, lastTitle);
        return Sitecore.Data.Validators.ValidatorResult.Error;
      }

      return Sitecore.Data.Validators.ValidatorResult.Valid;
    }
  }
}