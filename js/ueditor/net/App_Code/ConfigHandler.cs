using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
/// <summary>
/// Config 的摘要说明
/// </summary>
public class ConfigHandler : Handler
{
    public ConfigHandler(HttpContext context) : base(context) { }

    public override void Process()
    {
        string UrlPrefix = ConfigurationManager.AppSettings["UrlPrefix"];
        var obj = Config.Items;
        if (UrlPrefix != null)
        {
            obj.Remove("imageUrlPrefix");
            obj.Add("imageUrlPrefix", UrlPrefix);
            obj.Remove("fileUrlPrefix");
            obj.Add("fileUrlPrefix",UrlPrefix);
            obj.Remove("catcherUrlPrefix");
            obj.Add("catcherUrlPrefix", UrlPrefix);

        }
        WriteJson(obj);
    }
}