using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using We7.CMS.Common.Enum;
using System.IO;
using We7.CMS.Common;
using We7.Framework.Config;
namespace We7.CMS.Web.Admin
{
    public partial class Folder : BasePage
    {
        
        /// <summary>
        /// the url of We7 's shop
        /// </summary>
        public static string shopUrl;
        /// <summary>
        /// page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            shopUrl = GeneralConfigs.GetConfig().ShopService ?? "http://m.we7.cn/";
            string url = "/Plugins/FileManagement/UI/Folder.aspx";
            if (System.IO.File.Exists(Server.MapPath(url)))
            {
                Response.Redirect(string.Format("/Plugins/FileManagement/UI/Folder.aspx?{0}", Request.QueryString.ToString()));
            }

            //PluginInfo pinfo = new PluginInfo(Server.MapPath(string.Format("/Plugins/FileManagement/Plugin.xml")));
            //if (pinfo.IsInstalled)
            //{
            //    Response.Redirect(string.Format("/Plugins/FileManagement/UI/Folder.aspx?{0}", Request.QueryString.ToString()));
            //}
        }
        protected override MasterPageMode MasterPageIs
        {
            get
            {
                return MasterPageMode.None;
            }
        }
    }
}