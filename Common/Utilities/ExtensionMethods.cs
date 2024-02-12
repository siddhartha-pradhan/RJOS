using System.Net;
using System.Net.NetworkInformation;

namespace Common.Utilities;

public static class ExtensionMethods
{
    public static string SetUniqueFileName(this string fileExtension)
    {
        var renamedFileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Millisecond.ToString();
        
        return renamedFileName + fileExtension;
    }
    
    public static string GetIpAddress()
    {
        var hostName = Dns.GetHostName();
        
        var ipEntry = Dns.GetHostEntry(hostName);
        
        var addr = ipEntry.AddressList;
        
        return Convert.ToString(addr[^1]) ?? "";
    }
    public static string GetMacAddress()
    {
        var macAddress = string.Empty;
        
        var networks = NetworkInterface.GetAllNetworkInterfaces();
        
        foreach (var adapter in networks)
        {
            if (macAddress != string.Empty) continue;

            macAddress = Convert.ToString(adapter.GetPhysicalAddress());
        }
        
        return macAddress ?? "";
    }
}