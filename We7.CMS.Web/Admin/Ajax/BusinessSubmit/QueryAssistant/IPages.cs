using System;
using System.Collections.Generic;
using System.Web;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    public interface IPages
    {
        #region 分页
        /// <summary>
        /// 总条数
        /// </summary>
        int total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        int totalPage { get; }
        /// <summary>
        /// 页码
        /// </summary>
        int Page { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        int Rows { get; set; }
        /// <summary>
        /// 本页显示：从第几条开始
        /// </summary>
        int Begin { get; }
        /// <summary>
        /// 本页显示：到第几条结束
        /// </summary>
        int End { get; }
        /// <summary>
        /// 本页显示记录数
        /// </summary>
        int Count { get; }
        #endregion
    }
}