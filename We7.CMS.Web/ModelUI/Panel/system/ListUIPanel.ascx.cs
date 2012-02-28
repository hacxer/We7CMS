using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using We7.Model.Core.UI;
using We7.Model.Core;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;


namespace We7.Model.UI.Panel.system
{
    public partial class ListUIPanel : UserControl
    {
        public MoldPanel mp;
        public ListUIPanel()
        { 
            mp= new MoldPanel();
            ListpanelContext = mp.GetPanelContext(HttpContext.Current.Request.QueryString["model"], "list");
            columns = ListpanelContext.Panel.ListInfo.Groups[0].Columns;
            condition = ListpanelContext.Panel.ConditionInfo;
            InvokeAjaxTemplate();
        }

        #region fields
        ColumnInfoCollection columns;
        PanelContext ListpanelContext;
        private ConditionInfo condition; 
        #endregion

        /// <summary>
        /// 装载Ajax模板信息
        /// </summary>
        private void InvokeAjaxTemplate()
        {
            StringBuilder content = new StringBuilder();
            foreach (ColumnInfo field in columns) //处理字段
            {
                if (!field.Visible)
                    continue;
                ModelControlField lc = ModelHelper.GetDataControl(field.Type);
                if (lc != null)
                {
                    string attr = " width=\"" + field.Width + "\"";
                    if ("Manage".Equals(field.Name))  //构造管理列 打上标签(type='view' type='edit')
                    {
                        content.Append("<td " + attr + " header=\"" + field.Label + "\" width='100' algin='center'><a type='view' href='/admin/AddIns/ModelViewer.aspx?notiframe=1&model=" + ListpanelContext.Model.ModelName + "&ID=${ID}'>查看</a>&nbsp;&nbsp;<a type='edit' href='/admin/AddIns/ModelEditor.aspx?notiframe=1&model=" + ListpanelContext.Model.ModelName + "&ID=${ID}&groupIndex=0'>编辑</a></td>"); 
                    }
                    else
                    {
                       content.Append("<td " + attr + " header=\"" + field.Label + "\">${this[\"" + field.Name + "\"]}</td>");
                    }
                    fields = string.IsNullOrEmpty(fields) ? "\"" + field.Name + "\":{}" : fields + ",\"" + field.Name + "\":{}";

                    if (!string.IsNullOrEmpty(field.Params["model"])) //这个块的意思是：如果有有表连接，则拼出post串来。从右至左的意思为 字段名 链接表 链接表字段 链接表字段值(或者叫外键)
                    {
                        join += field.Name + "|" + field.Params["model"] + "|" + field.Params["textfield"] + "|" + field.Params["valuefield"] + "||";
                    }
                }
            }
            
            foreach (We7Control item in condition.Controls)
            {
                int querytype = -1;
                //查询操作符构造
                if (string.IsNullOrEmpty(item.Params["operater"])) querytype = (int)OperationType.LIKE;
                else querytype = (int)ModelHelper.GetOperation(item.Params["operater"]);
                conditionInnerHtml += item.Label + ":<input type='" + item.Type + "' field='" + item.Name + "' queryType='" + querytype + "'/>";
            }
            tableContent = content.ToString();
            if (mp.EnableSingleTable)
            {
                TableName = ListpanelContext.Model.Name;
            }
            else
            {
                TableName= ListpanelContext.Model.Type.ToString();
            }
            Caption = ListpanelContext.Model.Desc;
        }

       

        #region Ajax请求实体

        private string tableName;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        private string fields;

        public string Fields
        {
            get { return fields; }
            set { fields = value; }
        }
        private string sortField;

        public string SortField
        {
            get { return sortField; }
            set { sortField = value; }
        }
        private string sortOrder;

        public string SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
        private string caption;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        private string tableContent = string.Empty;

        public string TableContent
        {
            get { return tableContent; }
            set { tableContent = value; }
        }
        private string conditionInnerHtml = string.Empty;
        /// <summary>
        /// 查询条件HTML
        /// </summary>
        public string ConditionInnerHtml
        {
            get { return conditionInnerHtml; }
            set { conditionInnerHtml = value; }
        }
        private string join=string.Empty;
        public string Join
        {
            get { return join.Contains("||")? join.Remove(join.LastIndexOf("||")):""; }
            set {join=value; }
        }
        

        #endregion
    }
}