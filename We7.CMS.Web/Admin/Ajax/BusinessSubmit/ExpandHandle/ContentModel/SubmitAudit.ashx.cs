using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
using We7.CMS.Common;
using We7.Framework;
using System.Reflection;
using Thinkment.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    /// <summary>
    /// SubmitAudit 的摘要说明
    /// </summary>
    public class SubmitAudit : BasePage,IHttpHandler//, IRequiresSessionState
    {
        protected override bool NeedAnAccount
        {
            get
            {
                return true;
            }
        }
        protected override bool NeedAnPermission
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// SQL构造接口
        /// </summary>
        private IQueryCondition condiction;
        public SubmitAudit()
        {
            if (!string.IsNullOrEmpty(We7.CMS.Accounts.Security.CurrentAccountID))  //判断是否登录
            {
                condiction = new QueryCondition(HttpContext.Current.Request.Form); //初始化查询对象
                condiction.OnToJsonEvent += ExpandClass.SubmitAuditCommand;
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            if (condiction!=null)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write(condiction.ToJson(condiction));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}