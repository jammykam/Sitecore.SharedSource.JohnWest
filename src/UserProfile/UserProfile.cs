namespace Sitecore.Sharedsource.Web.UI.WebControls
{
  using System.Web.UI;

  public class UserProfile : Sitecore.Web.UI.WebControl
  {
    protected override void DoRender(HtmlTextWriter output)
    {
      Sitecore.Security.Accounts.User user = this.GetUser();

      if (user == null)
      {
        output.WriteLine("No such user.<br /><br />");
      }
      else
      {
        output.WriteLine("Username: " + user.Name + "<br />");
        output.WriteLine("Email: " + user.Profile.Email + "<br /><br />" + Sitecore.Context.Domain.Name + "<br />");
      }

      foreach(Sitecore.Security.Accounts.User member in Sitecore.Context.Domain.GetUsers())
      {
        output.WriteLine(@"<a href=""/users/" + member.LocalName + @".aspx"">" + member.LocalName + "</a><br />");
      }

      output.WriteLine("<br />");
    }

    private Sitecore.Security.Accounts.User GetUser()
    {
      if (Sitecore.Context.Items["profile_user"] == null)
      {
        Sitecore.Diagnostics.Assert.IsNotNull(Sitecore.Context.RawUrl, "RawUrl");
        string user = Sitecore.Context.RawUrl;
        int pos = user.IndexOf("?");

        if (pos > 0)
        {
          user = user.Substring(0, pos);
        }

        pos = user.LastIndexOf("/");

        if (pos > 0)
        {
          user = user.Substring(pos + 1);
        }

        pos = user.IndexOf(".aspx");

        if (pos > -1)
        {
          user = user.Substring(0, pos);
        }

        user = Sitecore.Context.Domain + "\\" + user;

        if (!Sitecore.Security.Accounts.User.Exists(user))
        {
          return null;
        }

        Sitecore.Security.Accounts.User obj = Sitecore.Security.Accounts.Account.FromName(
          user, Sitecore.Security.Accounts.AccountType.User)
                                              as Sitecore.Security.Accounts.User;
        Sitecore.Context.Items["profile_user"] = obj;
      }

      if (Sitecore.Context.Items["profile_user"] == null)
      {
        return null;
      }

      return Sitecore.Context.Items["profile_user"] as Sitecore.Security.Accounts.User;
    }
  }
}