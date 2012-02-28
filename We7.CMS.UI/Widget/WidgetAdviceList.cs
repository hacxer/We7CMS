using System;
using System.Collections.Generic;
using System.Text;
using We7.CMS.WebControls.Core;
using Thinkment.Data;
using We7.CMS.WebControls;
using We7.CMS.Common;
using We7.Framework.Util;

namespace We7.CMS.UI.Widget
{
    [ControlGroupDescription(Label = "反馈模型部件", Description = "自动生成的反馈模型部件", Icon = "反馈模型部件", DefaultType = "")]
    [ControlDescription(Desc = "反馈模型详细列表部件(自动生成)")]
    public class WidgetAdviceList : AdviceProvider
    {
        /// <summary>
        /// 选择反馈类型
        /// </summary>
        [Parameter(Title = "反馈类型", Type = "KeyValueSelector", Data = "adviceTypeId", DefaultValue = "", Required = true)]
        public string AdviceType;

        /// <summary>
        /// 分页器
        /// </summary>
        [Children]
        public ControlPager Pager = new ControlPager();

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public override List<AdviceInfo> GetItems()
        {
            //string[] fieldsArray=(Fields.Length>0?(Fields.Contains(",")?Fields.Split(','):new string[]{Fields}):null);
            List<AdviceInfo> items = null;
            int pageSize = Pager.PageSize <= 0 ? 10 : Pager.PageSize;
            int pageIndex = Pager.PageIndex, startIndex, pageItemsCount;
            Utils.BuidlPagerParam(Pager.RecordCount, pageSize, ref pageIndex, out startIndex, out pageItemsCount);
            Order[] os = IsShow ? new Order[] { new Order("IsShow", OrderMode.Desc), new Order("Updated", OrderMode.Desc), new Order("ID", OrderMode.Desc) } : new Order[] { new Order("Updated", OrderMode.Desc), new Order("ID", OrderMode.Desc) };
            Criteria c = CreateListCriteria();
            items = HelperFactory.Assistant.List<AdviceInfo>(c, os, startIndex, pageItemsCount);
            return items ?? new List<AdviceInfo>();
        }

        /// <summary>
        /// 页面加载
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(AdviceType))
                    Pager.RecordCount = HelperFactory.Assistant.Count<AdviceInfo>(CreateListCriteria());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string AdviceTypeID
        {
            get
            {
                try
                {
                    return new AdviceTypeHelper().GetAdviceTypeByModelName(AdviceType).ID;
                }
                catch { return AdviceType; }
            }
        }

        /// <summary>
        /// 生成查询条件
        /// </summary>
        /// <returns></returns>
        protected override Criteria CreateListCriteria()
        {
            string modelName;
            try { modelName = new AdviceTypeHelper().GetAdviceType(AdviceType).ModelName; }
            catch { modelName = AdviceType; }
            Criteria c = new Criteria(CriteriaType.Equals, "ModelName", modelName);
            c.Add(CriteriaType.Equals, "TypeID", AdviceTypeID);
            if (IsShow)
            {
                c.Add(CriteriaType.Equals, "IsShow", 1);
            }
            return c;
        }
    }
}
