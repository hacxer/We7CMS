using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using We7.CMS.Common.Enum;

namespace We7.CMS.Web.Admin.ContentModel
{
    public partial class ValidatorManage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }
        protected override MasterPageMode MasterPageIs
        {
            get
            {
                return MasterPageMode.NoMenu;
            }
        }

    }
}