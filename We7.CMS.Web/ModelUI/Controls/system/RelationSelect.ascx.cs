using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using We7.Model.Core.UI;
using We7.Framework.Util;
using We7.CMS.Common;
using We7.Model.Core;
using System.Data;
using System.IO;
using We7.Framework.Config;
using We7.Model.Core.Data;
using Thinkment.Data;
using We7.CMS;
using We7.CMS.Data;

namespace We7.Model.UI.Controls.system
{
    public partial class RelationSelect : FieldControl
    {
        protected DataBaseForThinkment GetDataBase(TableInfo ti)
        {
            return new DataBaseForThinkment(ti.TableName);
        }

        public override object GetValue()
        {
            string textfield = Control.Params["df"];
            object value = TypeConverter.StrToObjectByTypeCode(ddlEnum.SelectedValue, Column.DataType);
            if (String.IsNullOrEmpty(textfield))
            {
                return value;
            }
            else
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add(Column.Name, value);
                dic.Add(textfield, ddlEnum.SelectedItem.Text);
                return dic;
            }
        }

        public override void InitControl()
        {
            ddlEnum.PreRender += new EventHandler(ddlEnum_PreRender);
            #region 已废弃 2011-12-13 by Brina.G
            //以下将被更改
            //string model = Control.Params["model"];
            //string valuefield = Control.Params["valuefield"];
            //string textfield = Control.Params["textfield"];

            //if (GeneralConfigs.GetConfig().EnableSingleTable)
            //{
            //    ModelDBHelper helper=ModelDBHelper.Create(model);
            //    Criteria c=new Criteria(CriteriaType.Equals,"State",1);
            //    DataTable dt=helper.Query(c, new List<Order>() { new Order("Created",OrderMode.Desc),new Order("ID",OrderMode.Desc)}, 0, 0);
            //    ddlEnum.DataSource = dt;
            //}
            //else
            //{
            //    List<Article> list = ArticleHelper.QueryArticleByModel(model);
            //    DataSet ds = ModelHelper.CreateDataSet(model);
            //    foreach (Article a in list)
            //    {
            //        TextReader reader = new StringReader(a.ModelXml);
            //        ds.ReadXml(reader);
            //    }

            //    ddlEnum.DataSource = ds.Tables[0];               
            //}
            #endregion




            //string model = Control.Params["table"];
            string model = Control.Params["model"];
            string valuefield = Control.Params["valuefield"];
            string textfield = Control.Params["textfield"];
            //默认启用单表存储，并且不可更改
            if (!string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(valuefield) && !string.IsNullOrEmpty(textfield))
            {
                //兼容以前版本的关联控件  老版本格式为：group.tableName
                model = model.Contains(".") ? (model.Substring(model.IndexOf(".") + 1)) : model;

                TableInfo ti = new TableInfo(model);
                Criteria c = null;
                if (GetDataBase(ti).IsSiteGroup)
                {
                    c = new Criteria(CriteriaType.Equals, "FromSiteID", SiteConfigs.GetConfig().SiteGroupEnabled ? SiteConfigs.GetConfig().SiteID : string.Empty);
                }
                List<TableInfo> list = GetDataBase(ti).IDatabase.GetDtByCondition<TableInfo>(ti.TableName, c, new string[] { valuefield, textfield });

                if (list != null && list.Count > 0)
                {
                    DataTable dt = list[0].Table;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ddlEnum.DataSource = dt;
                        ddlEnum.DataValueField = dt.Columns[0].ColumnName;// valuefield;
                        ddlEnum.DataTextField = dt.Columns[1].ColumnName; //textfield;
                        ddlEnum.DataBind();
                        ddlEnum.SelectedValue = Value == null ? Control.DefaultValue : Value.ToString();
                    }
                    ddlEnum.Items.Insert(0, new ListItem("请选择", ""));
                }
            }
            else
            {
                ddlEnum.Items.Insert(0, new ListItem("没有绑定数据源", ""));
            }

            if (!String.IsNullOrEmpty(Control.Width))
            {
                ddlEnum.Width = Unit.Parse(Control.Width);
            }
            if (!String.IsNullOrEmpty(Control.Height))
            {
                ddlEnum.Height = Unit.Parse(Control.Height);
            }

            Validator(ddlEnum);

            string urlParam = We7Helper.GetParamValueFromUrl(Request.RawUrl, Control.Name);
            if (!string.IsNullOrEmpty(urlParam)) ddlEnum.SelectedValue = urlParam;
        }

        void ddlEnum_PreRender(object sender, EventArgs e)
        {
            var convert = Control.Params["convert"];
            if (!String.IsNullOrEmpty(convert))
            {
                if (convert == "cat")
                {
                    CategoryHelper helper = We7.Framework.HelperFactory.Instance.GetHelper<CategoryHelper>();
                    foreach (ListItem item in ddlEnum.Items)
                    {
                        Category cat = helper.GetCategoryByKeyword(item.Text);
                        if (cat != null)
                        {
                            item.Text = cat.Name;
                        }
                    }
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            //InitControl();
            ddlEnum.SelectedValue = newID.Value;
        }
    }
}
