using System;
using System.Collections.Generic;
using System.Web;
using Thinkment.Data;
using System.Data;
using We7.Framework;
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
    public class DataBaseAssistant : IDataBaseAssiant
    {
        ObjectAssistant assistant;

        /// <summary>
        /// 当前Helper的数据访问对象
        /// </summary>
        public ObjectAssistant Assistant
        {
            get
            {
                if (assistant == null)
                {
                    assistant = HelperFactory.Instance.Assistant;
                }
                return assistant;
            }
            set { assistant = value; }
        }



        #region IDataBase接口实现
        /// <summary>
        /// 总数据库数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int Total(string tablename, Criteria condition, IConnection conn = null)
        {
            return null == conn ? Assistant.Count<TableInfo>(condition, tablename) : Assistant.Count<TableInfo>(conn, condition, tablename);
        }

        #region 查询List
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, null, 0, 0, (ListField[])null, tablename) : Assistant.List<T>(conn, condition, null, 0, 0, (ListField[])null);
            return (obj as List<T>);
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="o">排序字段</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, o, 0, 0, (ListField[])null, tablename) : Assistant.List<T>(conn, condition, o, 0, 0, (ListField[])null, tablename);
            return (obj as List<T>);
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="fildes">要返回的字段</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, string[] fildes, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, null, 0, 0, fildes, tablename) : Assistant.List<T>(conn, condition, null, 0, 0, fildes, tablename);
            return (obj as List<T>);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="o">排序字段</param>
        /// <param name="fildes">要返回的字段</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, string[] fildes, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, o, 0, 0, fildes, tablename) : Assistant.List<T>(conn, condition, o, 0, 0, fildes, tablename);
            return (obj as List<T>);
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <param name="o">排序字段</param>
        /// <param name="form">页码</param>
        /// <param name="count">查询个数</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, int form, int count, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, o, form, count, (ListField[])null, tablename) : Assistant.List<T>(conn, condition, o, form, count, (ListField[])null, tablename);
            return (obj as List<T>);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="condition">条件</param>
        /// <param name="o">排序数组</param>
        /// <param name="form">起始</param>
        /// <param name="count">要查询的个数</param>
        /// <param name="fildes">要显示的字段</param>
        /// <returns></returns>
        public List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, int form, int count, string[] fildes, IConnection conn = null)
        {
            object obj = Activator.CreateInstance(typeof(T));
            obj = null == conn ? Assistant.List<T>(condition, o, form, count, fildes, tablename) : Assistant.List<T>(conn, condition, o, form, count, fildes, tablename);
            return (obj as List<T>);
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int DeleteList<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            return null == conn ? Assistant.DeleteList<T>(condition, tablename) : Assistant.DeleteList<T>(conn, condition, tablename);
        }

        public int Update(string tablename, object obj, string[] fields, Criteria condition, IConnection conn = null)
        {
            return null == conn ? Assistant.Update(obj, fields, condition, tablename) : Assistant.Update(conn, obj, fields, condition, tablename);
        }

        public int Update(string tablename, object obj, string[] fields, IConnection conn = null)
        {
            return null == conn ? Assistant.Update(obj, fields, null, tablename) : Assistant.Update(conn, obj, fields, null, tablename);
        }
        public int Update(string tablename, object obj, IConnection conn = null)
        {
            return null == conn ? Assistant.Update(obj, null, null, tablename) : Assistant.Update(conn, obj, null, null, tablename);
        }
        public bool Delete(string tablename, object obj, IConnection conn = null)
        {
            return null == conn ? Assistant.Delete(obj, tablename) : Assistant.Delete(conn, obj, tablename);
        }
        public int Delete<T>(string tablename, Criteria condition, IConnection conn = null)
        {
            return null == conn ? Assistant.DeleteList<T>(condition, tablename) : Assistant.DeleteList<T>(conn, condition, tablename);
        }
        #endregion

        public IConnection CreateConnetion(Type key, bool isTransfer = false)
        {
            return Assistant.CreateConnetion(key, isTransfer);
        }
        public IConnection CreateConnetion(string key, bool isTransfer = false)
        {
            return Assistant.CreateConnetion(key, isTransfer);
        }

        public DataTable Join(DataTable result, IJoin join, IConnection conn = null)
        {
          /*
           * 这个方法的意思是：放一个result进来。 然后去数据库查询相关关联数据。完了之后返回。
           * 由于时间仓促，如果您觉得这个方法不适用，或者有些奇怪的代码。 不妨大胆的改进吧。
           * author:丁乐
           */
            if (join == null || join.JoinInfo == null) return result;
            if (result != null && result.Rows != null && result.Rows.Count > 0)
            {
                DataTable ResultDt = result;
                foreach (var item in join.JoinInfo)
                {
                    Criteria c = new Criteria(CriteriaType.None);
                    c.Mode = CriteriaMode.Or;
                    foreach (DataRow row in ResultDt.Rows)
                    {
                        c.AddOr(CriteriaType.Equals, item.Value.PriMaryKeyName, row[item.Key]);//外键列当前行的值
                    } //表连接条件
                    List<TableInfo> list;

                    TableInfo t = new TableInfo(item.Value.ToTableName);
                    list = new DataBaseForThinkment(item.Value.ToTableName).IDatabase.GetDtByCondition<TableInfo>(t.TableName, c, new string[] { item.Value.PriMaryKeyName, item.Value.ToField }, conn); //查询关联信息
                    if (list != null && list[0].Table != null && list[0].Table.Rows != null && list[0].Table.Rows.Count > 0)
                    { //填充
                        foreach (DataRow temp in ResultDt.Rows)
                        {
                            DataRow[] rows = list[0].Table.Select(item.Value.PriMaryKeyName + "= '" + temp[item.Key] + "'");
                            if (rows != null && rows.Length > 0)
                            {
                                temp[item.Key] = rows[0][1];
                            }
                        }
                    }

                }
            }
            return result;
        }
        #endregion
    }
}