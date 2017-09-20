namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor.Editors.Disassembler
{
  using System;
  using System.IO;
  using System.Reflection;
  using Assert = Sitecore.Diagnostics.Assert;
  using S = Sitecore;

  public class DisassemblerPage : S.Web.UI.XamlSharp.Xaml.XamlMainControl
  {
    public S.Web.UI.HtmlControls.Memo Code { get; set;  }

    protected override void OnLoad(EventArgs args)
    {
      string itemId = S.Web.WebUtil.GetQueryString("id");
      Assert.IsNotNullOrEmpty(itemId, "itemId");
      string dbName = S.Web.WebUtil.GetQueryString("database");
      Assert.IsNotNullOrEmpty(dbName, "dbName");
        S.Data.Database db = S.Configuration.Factory.GetDatabase(dbName);
      Assert.IsNotNull(db, "db: " + dbName);
      S.Data.Items.Item item = db.GetItem(itemId);
      Assert.IsNotNull(item, "item: " + itemId + " in " + dbName);
      string value = item["Type"];
      Assert.IsNotNullOrEmpty(value, "value : Type field of " + item.Paths.FullPath);
      string name = string.Empty;
      string dll = string.Empty;
      int comma = value.IndexOf(',');

      if (comma >= 0)
      {
        name = value.Substring(0, comma).Trim();
        dll = value.Substring(comma + 1).Trim();
      }

      Assembly assembly = S.Reflection.ReflectionUtil.LoadAssembly(dll);
      Assert.IsNotNull(assembly, "assembly: " + dll);
      Type type = assembly.GetType(name, false, true);

      if (type == null)
      {
        type = assembly.GetType(type + "`1");
      }

      Assert.IsNotNull(type, "type : " + type + " in " + dll);
      dll = S.IO.FileUtil.MapPath("/bin/" + dll + ".dll");
      Assert.IsTrue(File.Exists(dll), dll + " does not exist");
      string tempFile = string.Format(
        "{0}.{1}.il",
        type,
        DateTime.Now.ToString("yyyyMMddTHHmmssffff"));
      S.IO.TempFolder.EnsureFolder();
      tempFile = S.IO.FileUtil.MapPath(
        S.IO.FileUtil.MakePath(S.IO.TempFolder.Folder, tempFile, '/'));
      S.Sharedsource.Diagnostics.CommandLineTool clt =
        new S.Sharedsource.Diagnostics.CommandLineTool(
          "c:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v8.0A\\bin\\NETFX 4.0 Tools\\x64\\ildasm.exe");
      clt.Execute(
        true /*log*/, 
        "/out=" + tempFile + " /source /linenum /item=" + type + " " + dll); // /pubonly /html
      Assert.IsTrue(File.Exists(tempFile), tempFile + " does not exist");
      this.Code.Value = S.IO.FileUtil.ReadFromFile(tempFile);
      File.Delete(tempFile);
      base.OnLoad(args);
    }
  }
}