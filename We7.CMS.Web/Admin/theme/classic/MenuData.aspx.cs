using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using We7.CMS.Controls;
using We7.CMS.Config;
using We7.CMS.Install;

namespace We7.CMS.Web.Admin.theme.classic
{
    public partial class MenuData : System.Web.UI.Page
    {
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private static object syncRoot = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            We7MenuControl menu = new We7MenuControl();            
            //检查
            if (menu.AllShowMemuItem.Count == 0)
            {
                lock (syncRoot)
                {
                    if (menu.AllShowMemuItem.Count == 0)
                    {
                        // 无Menu数据则进行SQL执行
                        BaseConfigInfo bci = BaseConfigs.GetBaseConfig();
                        if (bci.DBType != "" && bci.DBConnectionString != "")
                        {
                            Installer.ExcuteSQLGroup(bci);
                            ApplicationHelper.ResetApplication();
                        }
                    }
                }
            }
            Response.Write(menu.AllMenuHtml());
            Response.End();
        }
    }
}
