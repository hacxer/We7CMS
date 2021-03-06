﻿/***************************
 * 时间：2010/10/28 11:23:19  
 * CLR版本：2.0.50727.4952  
 * 唯一标识：d4d4d4ec-2f53-4293-9341-e23aa48e495c  
 * 功能：为Advice列表显示与详细信息查询提供数据
 * *****************************/

using System;
using System.Collections.Generic;
using System.Text;
using We7.CMS.Common;
using Thinkment.Data;
using System.Data;
using We7.CMS.Common.PF;
using System.Xml;
using System.IO;
using We7.Framework.Util;

namespace We7.CMS.WebControls
{
    public partial class AdviceProviderEx : BaseWebControl
    {
        private List<AdviceInfo> items;
        private AdviceInfo item;
        private string adviceTypeID;
        private string queryKey = "advicetype";
        public List<AdviceReplyInfo> replies;

        /// <summary>
        /// Css样式
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// 是否处理
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 当前反馈的模型
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string QueryKey
        {
            get { return queryKey; }
            set { queryKey = value; }
        }

        /// <summary>
        /// 反馈类型
        /// </summary>
        public virtual string AdviceTypeID
        {
            get
            {
                if (String.IsNullOrEmpty(adviceTypeID))
                {
                    string query = Request[QueryKey];
                    if (!String.IsNullOrEmpty(query))
                    {
                        query = query.Trim().ToLower();
                        string path = We7Utils.GetMapPath("/Config/advicemapping.xml");
                        if (File.Exists(path))
                        {
                            XmlNode node = XmlHelper.GetXmlNode(path, "//item[@key='" + query + "']");
                            if (node != null)
                            {
                                XmlElement xe = node as XmlElement;
                                adviceTypeID = (xe.GetAttribute("value") ?? "").Trim();
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(adviceTypeID))
                    {
                        throw new Exception("advicemapping.xml不存在对应的类型");
                    }
                }
                return adviceTypeID;
            }
            set { adviceTypeID = value; }
        }

        /// <summary>
        /// 导航栏目
        /// </summary>
        public string OwnerID { get; set; }

        public string GetAccountName(string accountID)
        {
            Account act = AccountHelper.GetAccount(accountID, new[] { "FirstName", "MiddleName", "LastName" });
            if (act != null)
            {
                return String.Format("{0}{1}{2}", act.FirstName, act.MiddleName, act.LastName);
            }
            else
            {
                return String.Empty;
            }
        }

        public DataTable Data
        {
            get
            {
                //查询
                return new DataTable();
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public List<AdviceInfo> Items
        {
            get
            {
                if (items == null)
                {
                    items = GetItems();
                    string modelName = GetModelName();
                    foreach (AdviceInfo item in items)
                    {
                        item.ModelName = modelName;
                    }
                }
                return items;
            }
        }

        /// <summary>
        /// 取得当前记录的信息
        /// </summary>
        public AdviceInfo Item
        {
            get
            {
                if (item == null)
                {
                    item = GetItem();
                    if (item != null)
                        item.ModelName = GetModelName();
                }
                return item;
            }
        }

        public List<AdviceReplyInfo> Replies
        {
            get
            {
                if (replies == null)
                {
                    replies = AdviceFactory.Create().QueryReplies(GetAdviceID());
                    if (replies == null) replies = new List<AdviceReplyInfo>();
                }
                return replies;
            }
        }

        protected string GetUrl(string id)
        {
            return UrlHelper.GetUrl(OwnerID, id);
        }

        /// <summary>
        /// 创建列表查询条件
        /// </summary>
        /// <returns></returns>
        protected virtual Criteria CreateListCriteria()
        {
            Criteria c = new Criteria(CriteriaType.Equals, "TypeID", AdviceTypeID);
            if (IsShow)
            {
                c.Add(CriteriaType.Equals, "IsShow", 1);
            }
            return c;
        }

        /// <summary>
        /// 创建信息排序数组
        /// </summary>
        /// <returns></returns>
        protected virtual Order[] CreateOrderArray()
        {
            return new Order[] { new Order("Created", OrderMode.Desc)};
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IncludeJavaScript("/Admin/Ajax/advicecheck.js");
        }
    }

    public partial class AdviceProviderEx
    {
        /// <summary>
        /// 要据查询条件取得反馈列表
        /// </summary>
        /// <returns></returns>
        private List<AdviceInfo> GetItems()
        {
            List<AdviceInfo> items = null;
            RecordCount = HelperFactory.Assistant.Count<AdviceInfo>(CreateListCriteria());
            items = HelperFactory.Assistant.List<AdviceInfo>(CreateListCriteria(), CreateOrderArray(), StartItem, PageItemsCount);
            return items != null ? items : new List<AdviceInfo>();
        }

        /// <summary>
        /// 取得当前记录
        /// </summary>
        /// <returns></returns>
        private AdviceInfo GetItem()
        {
            Criteria c = new Criteria(CriteriaType.Equals, "ID", GetAdviceID());
            List<AdviceInfo> list = HelperFactory.Assistant.List<AdviceInfo>(c, null);
            return list != null && list.Count > 0 ? list[0] : new AdviceInfo();
        }

        /// <summary>
        /// 报得反馈ID
        /// </summary>
        /// <returns></returns>
        private string GetAdviceID()
        {
            return HelperFactory.GetHelper<ArticleHelper>().GetArticleIDFromURL();
        }

        /// <summary>
        /// 取得当前反馈的模型名称
        /// </summary>
        /// <returns></returns>
        protected virtual string GetModelName()
        {
            if (String.IsNullOrEmpty(ModelName))
            {
                if (String.IsNullOrEmpty(AdviceTypeID))
                    throw new Exception("反馈类型为空");
                AdviceType adviceType = HelperFactory.GetHelper<AdviceTypeHelper>().GetAdviceType(AdviceTypeID);
                ModelName= adviceType != null ? adviceType.ModelName : String.Empty;
            }
            return ModelName;
        }
    }
}
