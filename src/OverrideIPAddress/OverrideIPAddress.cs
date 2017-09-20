namespace Sitecore.Sharedsource.Analytics.Pipelines.StartTracking
{
  using System.Net;

  using Sitecore.Analytics;
  using Sitecore.Analytics.Pipelines.StartTracking;

  public class OverrideIPAddress
  {
    public void Process(StartTrackingArgs args)
    {
      if (Tracker.CurrentVisit == null
        || Tracker.CurrentVisit.GeoIp == null
        || Tracker.CurrentVisit.Ip == null)
      {
        return;
      }

      string ip = new IPAddress(
        Tracker.CurrentVisit.GeoIp.Ip).ToString();

      if (ip != "0.0.0.0" && ip != "127.0.0.1")
      {
        return;
      }

      string html = Sitecore.Web.WebUtil.ExecuteWebPage(
        "http://www.whatismyip.com/automation/n09230945.asp");
      IPAddress address = IPAddress.Parse(html);
      Tracker.CurrentVisit.GeoIp = 
        Tracker.Visitor.DataContext.GetGeoIp(address.GetAddressBytes());
    }
  }
}