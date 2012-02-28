using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using We7.CMS.Common.Enum;
using Thinkment.Data;

namespace We7.CMS.Web.Admin.ContentModel
{
    public partial class ChooseTableColumn : BasePage
    {

        /// <summary>
        /// json
        /// </summary>
        public StringBuilder result = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO:目前读取CD.xml和ContentModel的XML ，取出信息不全，可能会和数据库中有一定差异，无法读取table的描述
           result.Append("[");
            foreach (var item in Assistant.DicForTable().Values)
            {
                result.Append("{\"Table\":\"" + item.CurObject.TableName + "\",\"Desc\":\"" + item.CurObject.Description + "\",\"PK\":\"" + item.CurObject.PrimaryKeyName + "\",\"Columns\":[");
               
                foreach (var p in item.CurObject.PropertyDict)
                {
                    if(p.Value.Name.ToUpper()!="ID")
                        result.Append("\"" + p.Value.Name + "\",");
                }
             if(result.ToString().LastIndexOf(",")>0)
                 result.Remove(result.ToString().LastIndexOf(","), 1);
                result.Append("]},");
            }
            if (result.ToString().LastIndexOf(",") > 0)
                result.Remove(result.ToString().LastIndexOf(","), 1);
          
            result.Append("]");
        }
        protected override MasterPageMode MasterPageIs
        {
            get
            {
                return MasterPageMode.NoMenu;
            }
        }


        ObjectAssistant _assistant;
        /// <summary>
        /// 当前Helper的数据访问对象
        /// </summary>
        protected ObjectAssistant Assistant
        {
            get { return _assistant ?? (_assistant = HelperFactory.Assistant); }
            set { _assistant = value; }
        }
    }
}