namespace Sitecore.Sharedsource.Tasks
{
  using System.IO;
  using System.Text;
  using System.Xml.Serialization;

  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharedsource.Pipelines.OutputCaches;
  using Sitecore.Sharedsource.Publishing;
  using Sitecore.Tasks;

  public class ProcessOutputCaches
  {
    public void Process(
      Item[] items,
      CommandItem commandItem,
      ScheduleItem scheduleItem)
    {
      Assert.ArgumentNotNull(scheduleItem, "scheduleItem");

      if (scheduleItem["Instance"] != Configuration.Settings.InstanceName)
      {
        return;
      }

      Log.Info(this + " : OutputCacheClearingOptions : " + scheduleItem["OutputCacheClearingOptions"], this);
      XmlSerializer serializer = new XmlSerializer(typeof(OutputCacheClearingOptions));
      OutputCacheClearingOptions outputCacheClearingOptions =
        (OutputCacheClearingOptions) serializer.Deserialize(new StringReader(
          scheduleItem["OutputCacheClearingOptions"]));
      new OutputCacheClearer().ClearOutputCaches(outputCacheClearingOptions);

      if (scheduleItem["Auto Remove"] == "1")
      {
        scheduleItem.InnerItem.Delete();
      }
    }
  }
}