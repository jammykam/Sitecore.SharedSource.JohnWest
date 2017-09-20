namespace Sitecore.Sharedsource.Pipelines.Loader
{
  using System;
  using System.IO;

  public class RollLogs
  {
    public void Process(Sitecore.Pipelines.PipelineArgs args)
    {
      Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
      string logDirectory = Sitecore.IO.FileUtil.MapPath(Sitecore.Configuration.Settings.LogFolder);
      string bakDirectory = logDirectory + "\\" + Sitecore.DateUtil.IsoNow;

      if (!Directory.Exists(bakDirectory))
      {
        Directory.CreateDirectory(bakDirectory);
      }

      foreach(string file in Directory.GetFiles(logDirectory))
      {
        try
        {
          File.Move(file, bakDirectory + "\\" + Path.GetFileName(file));
        }
        catch (Exception ex)
        {
          // disregard locked files
          Sitecore.Diagnostics.Log.Info(this + " : unable to move " + file + " " + ex.GetType() + " : " + ex.Message, this);
        }
      }
    }
  }
}