using System;
using System.Collections.Generic;
using System.Text;
using Thinkment.Data;

namespace We7.CMS.Data
{
    /// <summary>
    /// 数据库表Join接口
    /// 功能:基于DataTable的关联查询
    /// author:丁乐
    /// </summary>
    public interface IJoin:ICloneable
    {
        /// <summary>
        /// 以关联字段为Key的字典（或者叫外键）
        /// </summary>
        IDictionary<string, IJoin> JoinInfo { get; set; }
        /// <summary>
        /// 主键名
        /// </summary>
        string PriMaryKeyName { get; set; }
        /// <summary>
        /// 主表名
        /// </summary>
        string ToTableName{get;set;}
        /// <summary>
        /// 主表字段（被关联）
        /// </summary>
        string ToField{get;set;}
        /// <summary>
        /// 关联字段
        /// </summary>
        string MainField { get; set; }
        /// <summary>
        /// 站群
        /// </summary>
        bool IsSiteGroup { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        Criteria Condition { get; set; }
    }
}
