using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using We7.CMS.Common;
using We7.Framework.Config;

namespace Fck.CustomButton.ImportWord
{
    /// <summary>
    /// Import Word 
    /// </summary>
    public partial class ImportWord : System.Web.UI.Page
    {
        /// <summary>
        /// the url of We7 's shop
        /// </summary>
        public static string shopUrl;
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            shopUrl = GeneralConfigs.GetConfig().ShopService ?? "http://m.we7.cn/";
            string url = "/Plugins/ImportWord/UI/index.aspx";
            if (System.IO.File.Exists(Server.MapPath(url)))
            {
                Response.Redirect(url);
            }
        }
    }
}