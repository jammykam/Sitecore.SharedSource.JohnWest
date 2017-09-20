namespace Sitecore.Sharedsource.Text
{
  using System;
  using System.Collections;
  using System.Xml;

  using SC = Sitecore;

  public class Replacer : SC.Text.Replacer
  {
    private ArrayList _replacements = new ArrayList();

    public Replacer(string name) : base(name)
    {
    }

    public override void AddReplacement(XmlNode definition)
    {
      SC.Diagnostics.Assert.IsNotNull(definition, "definition");

      switch (definition.LocalName)
      {
        case "random":
          this.AddRandom(definition);
          return;
      }

      base.AddReplacement(definition);
    }

    protected void AddRandom(XmlNode definition)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(definition, "definition");
      RandomReplacement replacement = new RandomReplacement(definition);

      if (!replacement.IsEmpty)
      {
        lock (this._replacements.SyncRoot)
        {
          this._replacements.Add(replacement);
        }
      }
    }

    public override bool IsEmpty
    {
      get
      {
        return base.IsEmpty && this._replacements.Count < 1;
      }
    }

    public override string Replace(string text)
    {
      SC.Diagnostics.Assert.ArgumentNotNull(text, "text");

      foreach (Replacement replacement in this._replacements)
      {
        if (!replacement.IsEmpty)
        {
          text = replacement.Replace(text);
        }
      }

      return base.Replace(text);
    }

    public class RandomReplacement : SC.Text.Replacer.Replacement
    {
      private string _find;
      private int _max = Int32.MaxValue;
      private bool _ignoreCase;
      private bool _forPublish; //TODO: do whatever with this
      private static Random _random = new Random(DateTime.Now.Millisecond);

      public RandomReplacement(XmlNode definition)
      {
        this._find = SC.Xml.XmlUtil.GetAttribute("find", definition);
        string max = SC.Xml.XmlUtil.GetAttribute("max", definition);

        if (!String.IsNullOrEmpty(max))
        {
          if (!Int32.TryParse(max, out this._max))
          {
            throw new ArgumentException("Invalid max configuration: " + max);
          }
        }

        this._ignoreCase = SC.Xml.XmlUtil.GetAttribute(
          "ignoreCase", 
          definition) == "true";
        this._forPublish = SC.Xml.XmlUtil.GetAttribute(
          "forPublish", 
          definition) == "true";
      }

      public override string Replace(string text)
      {
        if (this._forPublish)
        {
          SC.Diagnostics.PerformanceCounters.PublishingCounters.PublishReplacements.Increment();
        }

        int startIndex;

        do
        {
          if (this._ignoreCase)
          {
            startIndex = text.IndexOf(
              this._find,
              StringComparison.OrdinalIgnoreCase);
          }
          else
          {
            startIndex = text.IndexOf(
              this._find,
              StringComparison.Ordinal);
          }

          if (startIndex < 0)
          {
            break;
          }

          string replace = _random.Next(this._max).ToString();
          text = text.Substring(0, startIndex) 
            + replace 
            + text.Substring(startIndex + this._find.Length);
        } while (startIndex > -1);

        return text;
      }

      public override bool IsEmpty
      {
        get { return String.IsNullOrEmpty(this._find); }
      }
    }
  }
}