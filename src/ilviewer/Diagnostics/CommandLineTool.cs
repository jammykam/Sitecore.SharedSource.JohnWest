namespace Sitecore.Sharedsource.Diagnostics
{
  using System;
  using System.Diagnostics;
  using System.Security;

  using Assert = Sitecore.Diagnostics.Assert;
  using Log = Sitecore.Diagnostics.Log;
  using S = Sitecore;

  public class CommandLineTool
  {
    public string CommandLine
    {
      get;
      set;
    }

    public int ExitCode
    {
      get;
      set;
    }

    public CommandLineTool(string commandLine)
    {
      this.CommandLine = commandLine;
    }

    public string StandardOutput
    {
      get;
      set;
    }

    public string StandardError
    {
      get;
      set;
    }

    public bool Execute(bool log = false, string arguments = null)
    {
      this.InvokeProcess(arguments);

      if (this.ExitCode == 0 && String.IsNullOrEmpty(this.StandardError))
      {
        return true;
      }

      if (log)
      {
        Log.Error(
          this + " : Exit code " + this.ExitCode + " returned by " + this.CommandLine, 
          this);

        if (!string.IsNullOrEmpty(this.StandardOutput))
        {
          Log.Error(
            this + " : STDOUT : " + System.Environment.NewLine + this.StandardOutput, 
            this);
        }

        if (!string.IsNullOrEmpty(this.StandardError))
        {
          Log.Error(
            this + " : STDERR : " + System.Environment.NewLine + this.StandardError, 
            this);
        }

        Log.Error(this + " : Stack Trace :", this);
        Log.Error(Environment.StackTrace, this);
      }

      return false;
    }

    protected void InvokeProcess(string arguments = null)
    {
      Assert.IsNotNullOrEmpty(this.CommandLine, "CommandLine");

      ProcessStartInfo procStartInfo = new ProcessStartInfo(this.CommandLine)
//      ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd")
      {
        WorkingDirectory = S.Configuration.Settings.LogFolder,
        RedirectStandardOutput = true,
        RedirectStandardError = true
      };

      procStartInfo.UseShellExecute = false;

      //      procStartInfo.WindowStyle = ProcessWindowStyle.Normal;
      //      procStartInfo.LoadUserProfile = true; 

//      procStartInfo.Arguments = "/c \"" + this.CommandLine + '"';

      if (!string.IsNullOrEmpty(arguments))
      {
        procStartInfo.Arguments = arguments;
//        procStartInfo.Arguments += " " + arguments;        procStartInfo.Arguments += " " + arguments;
      }

//      procStartInfo.CreateNoWindow = false;

      Process process = new Process { StartInfo = procStartInfo };
      Log.Info(this + " : invoke " + this.CommandLine + " " + procStartInfo.Arguments, this);
      process.Start();
      process.WaitForExit();
      this.ExitCode = process.ExitCode;
      this.StandardError = process.StandardError.ReadToEnd();
      this.StandardOutput = process.StandardOutput.ReadToEnd();
      process.Close();
    }
  }
}