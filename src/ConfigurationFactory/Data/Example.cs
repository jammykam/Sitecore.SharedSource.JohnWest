namespace Sitecore.Sharedsource.Data
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Xml;

  public class Example : IExample
  {
    private string _stringProperty = null;
    private int _intProperty = 0;
    private ArrayList _publicList = new ArrayList();
    private List<string> _protectedList = new List<string>();

    protected void Log(string value)
    {
      Sitecore.Diagnostics.Log.Info(this + " : " + value, this);
    }

    public Example(string first, string second)
    {
      this.Log("First constructor argument: " + first);
      this.Log("Second constructor argument: " + second);
    }

    public string StringProperty
    {
      get
      { 
        return this._stringProperty;
      }

      set
      {
        this._stringProperty = value;
        this.Log("StringProperty set to " + value);
      }
    }

    public int IntProperty
    {
      get
      { 
        return this._intProperty;
      }

      set
      {
        this._intProperty = value;
        this.Log("IntProperty set to " + value);
      }
    }

    public ArrayList PublicList
    {
      get
      {
        this.Log("PublicList accessed.");
        return this._publicList;
      }
    }

    public void AddToProtectedList(string value)
    {
      this._protectedList.Add(value);
      this.Log(value + " added to protected list.");
    }

    public void ArbitraryMethod(XmlNode config)
    {
      this.Log("ArbitraryMethod invoked with " + config.OuterXml);
    }
  }
}