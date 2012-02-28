using System;
using System.Collections.Generic;
using System.Text;
using We7.Model.Core.UI;
using We7.CMS;
using We7.CMS.Common.PF;
using We7.CMS.Accounts;
using We7.Framework;
using We7.CMS.Common;

namespace We7.Model.Core.ListControl
{
    public class ModelToAdviceConvert : IUxConvert
    {
        /// <summary>
        /// 业务助手工厂
        /// </summary>
        protected HelperFactory HelperFactory
        {
            get
            {
                return HelperFactory.Instance;
            }
        }
        /// <summary>
        /// 反馈类别助手
        /// </summary>
        protected AdviceTypeHelper AdviceTypeHelper
        {
            get { return HelperFactory.GetHelper<AdviceTypeHelper>(); }
        }
        /// <summary>
        /// 反馈助手
        /// </summary>
        protected AdviceHelper2 AdviceHelper
        {
            get { return HelperFactory.GetHelper<AdviceHelper2>(); }
        }

        #region IUxConvert 成员

        public string GetText(object dataItem, ColumnInfo columnInfo)
        {                                   
            string result = "#";
            string v = ModelControlField.GetValue(dataItem, columnInfo.Name);
            string[] columnInfoSZ = columnInfo.Expression.Split(new char[] { '|' });
            if (columnInfoSZ.Length > 1 && !string.IsNullOrEmpty(columnInfoSZ[1]))
            {
                ModelInfo mi = ModelHelper.GetModelInfoByName(columnInfoSZ[1]);
                if (mi != null && !string.IsNullOrEmpty(mi.RelationModelName))
                {
                    AdviceType adviceType = AdviceTypeHelper.GetAdviceTypeByModelName(mi.RelationModelName);
                    if (adviceType != null && !string.IsNullOrEmpty(adviceType.ID))
                    {
                        string adviceTypeID = adviceType.ID;
                        result = "<a href='/admin/Advice/AdviceListEx.aspx?typeID=" + adviceTypeID + "&RelationID=" + v + "'>查看反馈</a>";
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
