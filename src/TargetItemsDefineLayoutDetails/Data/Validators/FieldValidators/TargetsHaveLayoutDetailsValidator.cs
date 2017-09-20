namespace Sitecore.Sharedsource.Data.Validators.FieldValidators
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public class TargetsHaveLayoutDetailsValidator : Sitecore.Data.Validators.StandardValidator
  {
    public TargetsHaveLayoutDetailsValidator()
    {
    }

    public TargetsHaveLayoutDetailsValidator(SerializationInfo info, StreamingContext context)
    {
    }

    protected override Sitecore.Data.Validators.ValidatorResult Evaluate()
    {
      HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(base.ControlValidationValue);
      HtmlAgilityPack.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");

      if (nodes == null)
      {
        return Sitecore.Data.Validators.ValidatorResult.Valid;
      }

      foreach (HtmlAgilityPack.HtmlNode node in (nodes))
      {
        Sitecore.Data.Items.Item target = this.GetTargetItem(node.GetAttributeValue("href", String.Empty));

        if (target == null)
        {
          this.Text = "Target item for " + node.GetAttributeValue("href", String.Empty) + " does not exist.";
          return this.GetMaxValidatorResult();
        }

        if (String.IsNullOrEmpty(target[Sitecore.FieldIDs.LayoutField]))
        {
          this.Text = target.Paths.FullPath + " has no layout details.";
          return this.GetMaxValidatorResult();
        }
      }

      return Sitecore.Data.Validators.ValidatorResult.Valid;
    }

    protected Sitecore.Data.Items.Item GetTargetItem(string href)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(href, "href");
      Sitecore.Links.DynamicLink link;

      try
      {
        link = Sitecore.Links.DynamicLink.Parse(href);
      }
      catch (Sitecore.Web.InvalidLinkFormatException)
      {
        return null;
      }

      return Sitecore.Configuration.Factory.GetDatabase(
        base.ItemUri.DatabaseName).GetItem(link.ItemId);
    }

    protected override Sitecore.Data.Validators.ValidatorResult GetMaxValidatorResult()
    {
      return base.GetFailedResult(Sitecore.Data.Validators.ValidatorResult.Error);
    }

    public override string Name
    {
      get { return "TargetsHaveLayoutDetailsValidator"; }
    }
  }
}
