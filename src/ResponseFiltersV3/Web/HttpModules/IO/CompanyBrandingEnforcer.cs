namespace Sitecore.Sharedsource.Web.HttpModules.IO
{
  using System.Web;
  using System.IO;
  using System.Text;
  using System.Text.RegularExpressions;

  public class CompanyBrandingEnforcer : MemoryStream
  {
    // The stream passed to the constructor (probably the HTTP response output stream or another filter)
    readonly Stream _output;

    // Buffer for the chunks
    private StringBuilder _response;

    // Indicates not to run replacements for this HTTP response
    private Tristate _ignore = Tristate.Undefined;

    public CompanyBrandingEnforcer(Stream output)
    {
      Sitecore.Diagnostics.Assert.IsNotNull(output, "output");
      this._output = output;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      // The current chunk, converted from UTF8
      string text = null;

      // If this is the first chunk of this HTTP response
      if (this._ignore == Tristate.Undefined)
      {
        if (Sitecore.Context.Site == null
          || (HttpContext.Current.Response.ContentType != "text/html" && HttpContext.Current.Response.ContentType != "text/xml"))
        {
          this._ignore = Tristate.True;
        }
      }

      // If we don't know to ignore this chunk
      if (this._ignore != Tristate.True)
      {
        // Convert the UTF8 to text
        text = Encoding.UTF8.GetString(buffer);

        // If this is the first chunk for this response
        if (this._response == null)
        {
          if (Regex.IsMatch(text, "<html", RegexOptions.IgnoreCase))
          {
            this._response = new StringBuilder();
            this._ignore = Tristate.False;
          }
          else
          {
            this._ignore = Tristate.True;
          }
        }
      }

      // If we're ignoring this chunk, just write it to the stream
      if (this._ignore != Tristate.False)
      {
        // It should not be possible for this assert to throw
        Sitecore.Diagnostics.Assert.IsTrue(this._ignore != Tristate.Undefined, "this._ignore != Tristate.Undefined");
        this._output.Write(buffer, offset, count);
      }
      else
      {
        Sitecore.Diagnostics.Assert.IsNotNull(text, "text");
        // Add this chunk to the buffer););
        this._response.Append(text);

        // If it's the last chunk
        if (Regex.IsMatch(text, "</html>", RegexOptions.IgnoreCase))
        {
          text = this._response.ToString();
          text = Regex.Replace(text, "my\\s*company", "myCompany", RegexOptions.IgnoreCase);
          this._output.Write(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetByteCount(text));
        }
      }
    }
  }
}