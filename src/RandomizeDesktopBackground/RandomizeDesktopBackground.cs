namespace Sitecore.Sharedsource.Pipelines.LoggingIn
{
  using System;
  using System.IO;

  public class RandomizeDesktopBackground
  {
    public void Process(Sitecore.Pipelines.LoggingIn.LoggingInArgs args)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");

      if (args.StartUrl != "/sitecore/shell/default.aspx"
        || !args.Success)
      {
        return;
      }

      string path = Sitecore.IO.FileUtil.MapPath(Sitecore.Configuration.Settings.WallpapersPath)
                    + "\\"
                    + Sitecore.Security.Domains.Domain.ExtractShortName(args.Username);

      if (!Directory.Exists(path))
      {
        return;
      }

      string[] files = Directory.GetFiles(path);

      if (files.Length < 1)
      {
        return;
      }

      Sitecore.Security.Accounts.User user = Sitecore.Security.Accounts.User.FromName(args.Username, true);
      int which = new Random().Next(files.Length - 1);
      user.Profile["Wallpaper"] = Sitecore.IO.FileUtil.UnmapPath(files[which]);
      user.Profile.Save();
    }
  }
}