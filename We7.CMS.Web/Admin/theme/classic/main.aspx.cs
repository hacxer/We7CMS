using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Thinkment.Data;
using Newtonsoft.Json;

namespace We7.CMS.Web.Admin
{
    public partial class main : BasePage
    {
        /// <summary>
        /// 是否判断用户权限
        /// </summary>
        protected override bool NeedAnPermission
        {
            get
            {
                if (AccountHelper.GetAccount(AccountID, new string[] { "UserType" }).UserType == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
