namespace Sitecore.Sharedsource.Mvc.Presentation
{
  using System.IO;

  using SC = Sitecore;

  public class XsltRenderer : SC.Mvc.Presentation.XsltRenderer
  {
    protected override void AddParameters(System.Xml.Xsl.XsltArgumentList arguments)
    {
      base.AddParameters(arguments);
      arguments.AddParam("sc_datasource", string.Empty, this.Rendering.DataSource);
    }

    public override void Render(TextWriter writer)
    {
      // without this, Sitecore won't run the rendering
      if (this.Item == null)
      {
        this.Item = SC.Context.Item;
      }

      base.Render(writer);
    }
  }
}