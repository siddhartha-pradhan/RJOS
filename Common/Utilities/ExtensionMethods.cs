using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Common.Utilities;

public static class ExtensionMethods
{
    private static readonly string MaliciousInputPattern = @"(<\s*script\s*[^>]*>(.*?)<\s*/\s*script\s*>)|(<\s*iframe\s*[^>]*>(.*?)<\s*/\s*iframe\s*>)|(<\s*a\s*[^>]*>(.*?)<\s*/\s*a\s*>)|(<\s*p\s*[^>]*>(.*?)<\s*/\s*p\s*>)|(<\s*strong\s*[^>]*>(.*?)<\s*/\s*strong\s*>)|(\b(?:on\w+))|(\bon\w+\s*=)|(javascript\s*:\s*)|(document\s*\.\s*)|(\b(?:eval|expression|prompt|alert|confirm|javascript|setTimeout|setInterval))|(--\s*;)|(/\*\s*(.*?)\s*\*/)|('(?:[^']|'')*')|(\]|\)|(#|\bselect\b|\bupdate\b|\bdelete\b|\binsert\b|\bdrop\b|\bcreate\b|\btruncate\b|\balter\b|\bexec\b|\bexecute\b|\bscript\b))";

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

    public static bool IsMaliciousInput(string input)
    {
        return false;
    }
}