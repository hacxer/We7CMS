using System;
using System.Collections.Generic;
using System.Text;
using Thinkment.Data;
using We7.CMS.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    /// <summary>
    /// 构造SQL接口
    /// </summary>
    public interface IQueryCondition : IJoin, ITree, IPages,ICloneable
    {
        /// <summary>
        /// 输出Json
        /// </summary>
        event ResponseDelegate OnToJsonEvent;
        /// <summary>
        /// 输出Json以后
        /// </summary>
        event ReaderXmlDelegate UnToJsonEvent;

        /// <summary>
        /// 扩展事件用来干更多的事情
        /// </summary>
        event ExpandDelegate ExpandEvent;

        /// <summary>
        /// 输出Json
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        string ToJson(IQueryCondition condition);

        /// <summary>
        /// 扩展参数
        /// 用来处理更多的情况
        /// </summary>
        object ExpandKey { get; set; }

        /// <summary>
        /// 数据字典，用于回发消息
        /// </summary>
        IDictionary<string, string> JsonMessage { get; set; }

        /// <summary>
        /// 请求操作类型
        /// </summary>
        Enum_operType OperType { get; set; }

        /// <summary>
        /// xml数据模型信息(没有则为空)
        /// </summary>
        string HasModelXml { get; set; }

        /// <summary>
        /// 模型名(内容模型)
        /// </summary>
        string ModelName { get; set; }

        /// <summary>
        /// 是否多行操作
        /// </summary>
        bool IsMulti { get; set; }

        /// <summary>
        /// 主键ID（逗号隔开）
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// 用于排序的列名索引（一个名称）
        /// </summary>
        string Sort { get; set; }

        /// <summary>
        /// 排序的顺序（值为 asc(0) 或 desc(1)）
        /// 默认为asc
        /// </summary>
        int Sord { get; set; }

        /// <summary>
        /// 是否搜索（如果不搜索，其值为 false，）
        /// </summary>
        bool Search { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// 需要处理的字段数组
        /// </summary>
        string[] Fields { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        Criteria Condition { get; set; }

        /// <summary>
        /// 主键名
        /// </summary>
        string PriMaryKeyName { get; set; }

        /// <summary>
        /// 条件字典：键值对
        /// </summary>
        List<FieldsDic> ConditionDic { get; set; }

        /// <summary>
        /// 站群
        /// </summary>
        bool IsSiteGroup { get; set; }
    }
    public delegate void ResponseDelegate(IQueryCondition condition);
    public delegate void ReaderXmlDelegate(IQueryCondition condition);
    public delegate object ExpandDelegate(object o);

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum Enum_operType
    {
        Add = 0,
        Del = 1,
        Seach = 2,
        Update = 3
    }
}
