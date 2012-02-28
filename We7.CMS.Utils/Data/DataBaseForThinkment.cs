using System;
using System.Collections.Generic;
using System.Web;
using Thinkment.Data;
using System.Data;
using We7.Framework;
using We7.CMS.Accounts;
using We7.Framework.Config;

namespace We7.CMS.Data
{
    /// <summary>
    /// 数据库处理类
    /// 功能:增、删、改
    ///      基于DataTable的查询
    ///      分页查询
    /// author:丁乐
    /// </summary>
    public class DataBaseForThinkment:IParametExtension<DataBaseForThinkment>
    {
        protected virtual string tablename
        {
            get;
            set;
        }

        public DataBaseForThinkment(string tablename)
        {
            this.tablename = tablename;
        }

        public DataBaseForThinkment()
        { }

        private  bool isSiteGroup=false;
        /// <summary>
        /// 是否启用站群
        /// </summary>
        public bool IsSiteGroup
        {
            get
            {
                if (SiteConfigs.GetConfig().SiteGroupEnabled)
                {
                    List<string> siteTable = new List<string>();
                    siteTable.AddRange(new string[] { "Department", "Permission", "Account", "Role", "AccountRole", "MenuItem" }); //站群表。 暂时先写死。以后改成其他形式
                    isSiteGroup = siteTable.Exists(delegate(string value)
                    {
                        return string.Compare(tablename, value, true) == 0 ? true : false;
                    });
                }
               
                return isSiteGroup;
            }
        }

        private IDataBaseAssiant iDatabase;
        private IDataBaseAssiant iDatabase_Remote;
        /// <summary>
        /// 数据业务接口
        /// 备注：当此实例为远程实例时，泛型T只能为 TableInfo,
        ///       因为Webservice不支持用List T
        /// </summary>
        public IDataBaseAssiant IDatabase
        {
            get
            {
                if (IsSiteGroup)
                {
                    if (iDatabase_Remote == null)
                    {
                        if (string.IsNullOrEmpty(SiteConfigs.GetConfig().PassportServiceUrl))
                        {
                            We7.Framework.LogHelper.WriteLog(typeof(DataBaseForThinkment), new Exception("您还没有在site.config中设置好身份认证服务地址（PassportServiceUrl）的值！"));
                        }
                        else
                        {
                            iDatabase_Remote = new DataBaseAssiant_Remote() as IDataBaseAssiant; //远程Assistant
                        }
                    }
                    return iDatabase_Remote;
                }
                else
                {
                    if (iDatabase == null)
                    {
                        iDatabase = new DataBaseAssistant() as IDataBaseAssiant;
                    }
                    return iDatabase;
                }
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}