using System;
using System.Collections.Generic;
using System.Text;
using Thinkment.Data;
using We7.CMS.Accounts.WD;
using We7.Framework.Config;
using We7.Framework.Util;

namespace We7.CMS.Data
{
    /// <summary>
    /// 访问远程(WD)的数据
    /// </summary>
    public class DataBaseAssiant_Remote : IDataBaseAssiant
    {
        AccountWebService RemoteHelper
        {
            get
            {
                AccountWebService client = new AccountWebService();
                client.Url = SiteConfigs.GetConfig().PassportServiceUrl;
                return client;
            }
        }

        public int Total(string tablename, Criteria condition, IConnection conn = null)
        {
           return RemoteHelper.Total(tablename,condition);
        }

        #region search
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename, condition, null, 0, 0, null);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        }

        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename,condition, o, 0, 0, null);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        }

        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, string[] fildes, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename,condition, null, 0, 0, fildes);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        }

        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, string[] fildes, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename,condition, o, 0, 0, fildes);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        }

        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, int form, int count, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename,condition, o, form, count, null);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        }

        public List<T> GetDtByCondition<T>(string tablename,Criteria condition, Order[] o, int form, int count, string[] fildes, IConnection conn = null)
        {
            List<T> list;
            TableInfo[] ti = RemoteHelper.GetDtByCondition(tablename,condition, o, form, count, fildes);
            list = ((null == ti || ti.Length == 0) ? null : new List<TableInfo>(ti)) as List<T>;
            return list;
        } 
        #endregion

        #region 增删改
        public int DeleteList<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            return RemoteHelper.DeleteList(tablename,condition);
        }

        public int Update(string tablename, object obj, string[] fields, Criteria condition, IConnection conn = null)
        {
            return RemoteHelper.Update(tablename,obj, fields, condition);
        }

        public int Update(string tablename,object obj, string[] fields, IConnection conn = null)
        {
            return RemoteHelper.Update(tablename,obj, fields, null);
        }

        public int Update(string tablename, object obj, IConnection conn = null)
        {
            return RemoteHelper.Update(tablename,obj, null, null);
        }

        public bool Delete(string tablename, object obj, IConnection conn = null)
        {
            return RemoteHelper.Delete(tablename,obj);
        }

        public int Delete<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            return RemoteHelper.DeleteList(tablename,condition);
        } 
        #endregion

        public IConnection CreateConnetion(Type key, bool isTransfer = false)
        {
            return null;
        }

        public IConnection CreateConnetion(string key, bool isTransfer = false)
        {
            return null;
        }


        public System.Data.DataTable Join(System.Data.DataTable result, IJoin join, IConnection conn = null)
        {
            return null;
        }
    }
}
