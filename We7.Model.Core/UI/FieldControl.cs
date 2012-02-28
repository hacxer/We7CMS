using System;
using System.Collections.Generic;
using System.Web.UI;
using We7.Model.Core;
using System.Web;
using We7.Model.Core.Config;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace We7.Model.Core.UI
{
    /// <summary>
    /// 字段控件
    /// </summary>
    public abstract class FieldControl:UserControl
    {
        private PanelContext panelContext;
        public PanelContext PanelContext
        {
            get
            {
                if (panelContext == null)
                    throw new Exception("当前模型信息为Null");
                return panelContext;
            }
            set
            {
                panelContext = value;
            }
        }

        private We7Control control;
        public We7Control Control
        {
            get
            {
                if (control == null)
                    throw new Exception("当前模型信息为Null");
                return control;
            }
            set
            {
                control = value;
            }
        }
        protected object Value
        {
            get { return DataField!=null?DataField.Value:null; }
        }

        private We7DataColumn column;
        protected We7DataColumn Column
        {
            get 
            {
                if (column == null)
                {
                    column = panelContext.Table.Columns[Control.Name];
                }
                return column;
            }
        }


        /// <summary>
        /// 验证控件是否绑定了验证规则 如果绑定了， 加入到控件属性中
        /// </summary>
        /// <param name="c">继承了WebControl或是IAttributeAccessor接口的控件 </param>
        protected void Validator(IAttributeAccessor c)
        {
            string validator = Control.Params["validator"] ?? "";
            if (validator.Length > 0)
            {
                List<Dictionary<string, string>> list = JavaScriptConvert.DeserializeObject<List<Dictionary<string, string>>>(validator);
                foreach (Dictionary<string, string> t in list)
                {
                    if (t.ContainsKey("rule") && t.ContainsKey("val"))
                    {
                        c.SetAttribute(t["rule"], t["val"]);
                    }
                }
            }
            //匹配以前的验证方式
            if (Control.Required && c.GetAttribute("required") == null)
            {
                c.SetAttribute("required", "required");
            }
        }


        private DataField dataField;
        private DataField DataField
        {
            get 
            {
                if (dataField == null)
                {
                    dataField=PanelContext.Row.IndexOf(Control.Name);
                }
                return dataField;
            }
        }

        internal void InitControl(PanelContext ctx,We7Control ctr)
        {
            Control = ctr;
            panelContext = ctx;
            InitControl();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        public abstract void InitControl();

        /// <summary>
        /// 取得控件值
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();

        /// <summary>
        /// 取得当前控件的Param值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetParam(string name)
        {
            string param = Control.Params[name];
            if (String.IsNullOrEmpty(param))
            {
                We7DataColumn dc = PanelContext.DataSet.Tables[0].Columns[Control.Name];
                param = dc != null ? dc.Params[name] : "";
            }
            return param;
        }

        protected string GetParam(string columnName, string name)
        {
            string param = Control.Params[name];
            if (String.IsNullOrEmpty(param))
            {
                We7DataColumn dc = PanelContext.DataSet.Tables[0].Columns[columnName];
                param = dc != null ? dc.Params[name] : "";
            }
            return param;
        }

        /// <summary>
        /// 是否是编辑状态
        /// </summary>
        public bool IsEdit
        {
            get
            {
                return ViewState["$IsEdit"] != null ? (bool)ViewState["$IsEdit"] : false;
            }
            set
            {
                ViewState["$IsEdit"] = value;
            }
        }

        protected string GetConstStr(string data)
        {
            if (String.IsNullOrEmpty(data))
                return String.Empty;
            XmlDocument doc = new XmlDocument();
            string path = Server.MapPath(Path.Combine(ModelConfig.ModelsDirectory, "Inc/Const.xml"));
            doc.Load(path);
            XmlElement xe = doc.DocumentElement.SelectSingleNode(data) as XmlElement;
            return xe != null ? xe.InnerText.Trim() : String.Empty;
        }
    }
}
