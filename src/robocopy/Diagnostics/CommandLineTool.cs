namespace Sitecore.Sharedsource.Diagnostics
{
  using System;
  using System.Diagnostics;

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

    public bool Execute()
    {
      return this.Execute(false);
    }

    public bool Execute(bool log)
    {
      this.InvokeProcess();

      if (this.ExitCode == 0 && String.IsNullOrEmpty(this.StandardError))
      {
        return true;
      }

      if (log)
      {
        Sitecore.Diagnostics.Log.Error(this + " : Exit code " + this.ExitCode + " returned by " + this.CommandLine, this);

        if (!String.IsNullOrEmpty(this.StandardOutput))
        {
          Sitecore.Diagnostics.Log.Error(
            this + " : STDOUT : " + System.Environment.NewLine + this.StandardOutput, this);
        }

        if (!String.IsNullOrEmpty(this.StandardError))
        {
          Sitecore.Diagnostics.Log.Error(
            this + " : STDERR : " + System.Environment.NewLine + this.StandardError, this);
        }

        Sitecore.Diagnostics.Log.Error(this + " : Stack Trace :", this);
        Sitecore.Diagnostics.Log.Error(System.Environment.StackTrace.ToString(), this);
      }

      return false;
    }

    protected void InvokeProcess()
    {
      Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(this.CommandLine, "CommandLine");
      ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + this.CommandLine);
      procStartInfo.WorkingDirectory = Sitecore.Configuration.Settings.LogFolder;
      procStartInfo.RedirectStandardOutput = true;
      procStartInfo.RedirectStandardError = true;
      procStartInfo.UseShellExecute = false;
      procStartInfo.CreateNoWindow = true;
      Process process = new Process {StartInfo = procStartInfo};
      process.Start();
      process.WaitForExit();
      this.ExitCode = process.ExitCode;
      this.StandardError = process.StandardError.ReadToEnd();
      this.StandardOutput = process.StandardOutput.ReadToEnd();
      process.Close();
    }
  }
}