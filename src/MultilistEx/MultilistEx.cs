namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
  using System.Collections;
  using System.Web.UI;

  using Sitecore.Collections;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Resources;

  using SC = Sitecore;

  public class MultilistEx : SC.Shell.Applications.ContentEditor.MultilistEx
  {
    private string _format;

    private void RewriteSource()
    {
      if (string.IsNullOrEmpty(this.Source))
      {
        return;
      }
        
      SafeDictionary<string> query = SC.Web.WebUtil.ParseQueryString(this.Source);

      if (query.Count < 0)
      {
        return;
      }

      this._format = query["format"];
      this.Source = query["datasource"];
    }

    private string GetItemHeader(SC.Data.Items.Item item)
    {
      if (string.IsNullOrEmpty(this._format))
      {
        return item.DisplayName;
      }

      return this._format.Replace("@name", item.Name)
        .Replace("@parentname", item.Parent.Name)
        .Replace("@grampsname", item.Parent.Parent.Name)
        .Replace("@path", item.Paths.FullPath);
    }

    protected override void DoRender(HtmlTextWriter output)
    {
      this.RewriteSource();
      IDictionary dictionary;
      ArrayList list;
      Assert.ArgumentNotNull(output, "output");
      Item current = SC.Context.ContentDatabase.GetItem(this.ItemID);
      Item[] items = this.GetItems(current);
      this.GetSelectedItems(items, out list, out dictionary);
      base.ServerProperties["ID"] = this.ID;
      string str = string.Empty;
      if (this.ReadOnly)
      {
        str = " disabled=\"disabled\"";
      }
      output.Write("<input id=\"" + this.ID + "_Value\" type=\"hidden\" value=\"" + StringUtil.EscapeQuote(this.Value) + "\" />");
      output.Write("<table" + this.GetControlAttributes() + ">");
      output.Write("<tr>");
      output.Write("<td class=\"scContentControlMultilistCaption\" width=\"50%\">" + Translate.Text("All") + "</td>");
      output.Write("<td width=\"20\">" + Sitecore.Resources.Images.GetSpacer(20, 1) + "</td>");
      output.Write("<td class=\"scContentControlMultilistCaption\" width=\"50%\">" + Translate.Text("Selected") + "</td>");
      output.Write("<td width=\"20\">" + Sitecore.Resources.Images.GetSpacer(20, 1) + "</td>");
      output.Write("</tr>");
      output.Write("<tr>");
      output.Write("<td valign=\"top\" height=\"100%\">");
      output.Write("<select id=\"" + this.ID + "_unselected\" class=\"scContentControlMultilistBox\" multiple=\"multiple\" size=\"10\"" + str + " ondblclick=\"javascript:scContent.multilistMoveRight('" + this.ID + "')\" onchange=\"javascript:document.getElementById('" + this.ID + "_all_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''\" >");
      foreach (DictionaryEntry entry in dictionary)
      {
        Item item = entry.Value as Item;
        if (item != null)
        {
          output.Write("<option value=\"" + this.GetItemValue(item) + "\">" + this.GetItemHeader(item) + "</option>");
        }
      }
      output.Write("</select>");
      output.Write("</td>");
      output.Write("<td valign=\"top\">");
      this.RenderButton(output, "Core/16x16/arrow_blue_right.png", "javascript:scContent.multilistMoveRight('" + this.ID + "')");
      output.Write("<br />");
      this.RenderButton(output, "Core/16x16/arrow_blue_left.png", "javascript:scContent.multilistMoveLeft('" + this.ID + "')");
      output.Write("</td>");
      output.Write("<td valign=\"top\" height=\"100%\">");
      output.Write("<select id=\"" + this.ID + "_selected\" class=\"scContentControlMultilistBox\" multiple=\"multiple\" size=\"10\"" + str + " ondblclick=\"javascript:scContent.multilistMoveLeft('" + this.ID + "')\" onchange=\"javascript:document.getElementById('" + this.ID + "_selected_help').innerHTML=this.selectedIndex>=0?this.options[this.selectedIndex].innerHTML:''\">");
      for (int i = 0; i < list.Count; i++)
      {
        Item item3 = list[i] as Item;
        if (item3 != null)
        {
          output.Write("<option value=\"" + this.GetItemValue(item3) + "\">" + this.GetItemHeader(item3) + "</option>");
        }
        else
        {
          string path = list[i] as string;
          if (path != null)
          {
            string str3;
            Item item4 = Sitecore.Context.ContentDatabase.GetItem(path);
            if (item4 != null)
            {
              str3 = this.GetItemHeader(item4) + ' ' + Translate.Text("[Not in the selection List]");
            }
            else
            {
              str3 = path + ' ' + Translate.Text("[Item not found]");
            }
            output.Write("<option value=\"" + path + "\">" + str3 + "</option>");
          }
        }
      }
      output.Write("</select>");
      output.Write("</td>");
      output.Write("<td valign=\"top\">");
      this.RenderButton(output, "Core/16x16/arrow_blue_up.png", "javascript:scContent.multilistMoveUp('" + this.ID + "')");
      output.Write("<br />");
      this.RenderButton(output, "Core/16x16/arrow_blue_down.png", "javascript:scContent.multilistMoveDown('" + this.ID + "')");
      output.Write("</td>");
      output.Write("</tr>");
      output.Write("<tr>");
      output.Write("<td valign=\"top\">");
      output.Write("<div style=\"border:1px solid #999999;font:8pt tahoma;padding:2px;margin:4px 0px 4px 0px;height:14px\" id=\"" + this.ID + "_all_help\"></div>");
      output.Write("</td>");
      output.Write("<td></td>");
      output.Write("<td valign=\"top\">");
      output.Write("<div style=\"border:1px solid #999999;font:8pt tahoma;padding:2px;margin:4px 0px 4px 0px;height:14px\" id=\"" + this.ID + "_selected_help\"></div>");
      output.Write("</td>");
      output.Write("<td></td>");
      output.Write("</tr>");
      output.Write("</table>");
    }

    private void RenderButton(HtmlTextWriter output, string icon, string click)
    {
      Assert.ArgumentNotNull(output, "output");
      Assert.ArgumentNotNull(icon, "icon");
      Assert.ArgumentNotNull(click, "click");
      ImageBuilder builder = new ImageBuilder
      {
        Src = icon,
        Width = 0x10,
        Height = 0x10,
        Margin = "2px"
      };
      if (!this.ReadOnly)
      {
        builder.OnClick = click;
      }
      output.Write(builder.ToString());
    }
  }
}