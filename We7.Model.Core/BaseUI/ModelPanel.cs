using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using We7.Framework;
using We7.Framework.Util;
using System.Web;
using We7.Framework.Config;

namespace We7.Model.Core.UI
{
    /// <summary>
    /// 模型容器基类
    /// </summary>
    public class MoldPanel 
    {
        /// <summary>
        /// 获取容器
        /// </summary>
        /// <param name="ModelName">模型名称</param>
        /// <param name="PanelName">容器类型名称</param>
        /// <returns></returns>
        public PanelContext GetPanelContext(string ModelName, string PanelName)
        {
            string name = ModelHelper.GetModelName(ModelName);
            return ModelHelper.GetPanelContext(name, PanelName);
        }
        /// <summary>
        /// 是否启用单表存储
        /// </summary>
        public bool EnableSingleTable
        {
            get
            {
                return GeneralConfigs.GetConfig().EnableSingleTable;
            }
        }

    }
}
