using System;
using System.Collections.Generic;
using System.Web;
using Thinkment.Data;
using System.Data;

namespace We7.CMS.Data
{
    /// <summary>
    /// 数据库处理接口
    /// 功能:增、删、改
    ///      基于DataTable的查询
    ///      分页查询
    /// author:丁乐
    /// </summary>
    public interface IDataBaseAssiant
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int Total(string tablename, Criteria condition, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="condition">条件</param>
        /// <param name="o">排序数组</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="fildes">定制字段信息(默认所有)</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, string[] fildes, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="o">排序</param>
        /// <param name="fildes">定制字段信息(默认所有)</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, string[] fildes, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录(分页)
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="o">排序</param>
        /// <param name="form">开始</param>
        /// <param name="count">记录数</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, int form, int count, IConnection conn = null);
        /// <summary>
        /// 根据条件取得记录(分页)
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="o">排序</param>
        /// <param name="form">开始</param>
        /// <param name="count">记录数</param>
        /// <param name="fildes">定制字段信息(默认所有)</param>
        /// <returns></returns>
        List<T> GetDtByCondition<T>(string tablename, Criteria condition, Order[] o, int form, int count, string[] fildes, IConnection conn = null);

        /// <summary>
        /// 根据条件删除一组数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        int DeleteList<T>(string tablename, Criteria condition, IConnection conn = null);

        /// <summary>
        /// 根据条件更新一组数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fields">需要更新的字段</param>
        /// <param name="condition">条件</param>
        /// <param name="conn">指定数据库</param>
        /// <returns></returns>
        int Update(string tablename, object obj, string[] fields, Criteria condition, IConnection conn = null);
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fields">有效字段</param>
        /// <returns></returns>
        int Update(string tablename, object obj, string[] fields, IConnection conn = null);
        /// <summary>
        /// 更新一条记录
        /// </summary>
        int Update(string tablename, object obj, IConnection conn = null);
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Delete(string tablename, object obj, IConnection conn = null);
        /// <summary>
        /// 删除一组数据
        /// </summary>
        /// <param name="obj">条件</param>
        /// <returns></returns>
        int Delete<T>(string tablename, Criteria condition, IConnection conn = null);
        /// <summary>
        /// 创建一个新连接
        /// </summary>
        /// <param name="key">类型</param>
        /// <param name="isTransfer">是否开启事务</param>
        /// <returns></returns>
        IConnection CreateConnetion(Type key, bool isTransfer = false);
        /// <summary>
        /// 创建一个新连接
        /// </summary>
        /// <param name="key">表名</param>
        /// <param name="isTransfer">是否开启事务</param>
        /// <returns></returns>
        IConnection CreateConnetion(string key, bool isTransfer = false);
        /// <summary>
        /// 链接查询
        /// </summary>
        /// <param name="result">需要处理的Result</param>
        /// <param name="join">关联信息</param>
        /// <param name="conn">一个数据库连接(缺省值为系统默认连接)</param>
        /// <returns></returns>
        DataTable Join(DataTable result,IJoin join, IConnection conn = null);
    }
}