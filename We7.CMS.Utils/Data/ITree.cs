using System;
using System.Collections.Generic;
using System.Text;
using Thinkment.Data;

namespace We7.CMS.Data
{
    /// <summary>
    /// 数据库表树接口
    /// author:丁乐
    /// </summary>
    public interface ITree: ICloneable
    {
        IDictionary<string, ITree> TreeInfo { get; set; }
        /// <summary>
        /// 父ID字段名
        /// </summary>
        string PIDKeyName { get; set; }
        /// <summary>
        /// 显示标题
        /// </summary>
        string TreeTitle{get;set;}
        /// <summary>
        /// 是否有子节点
        /// </summary>
        bool HasNode { get; set; }
        /// <summary>
        /// 树List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> TreeList<T>() where T:class;
    }
}
