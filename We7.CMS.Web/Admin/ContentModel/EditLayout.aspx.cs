using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using We7.CMS.Common.Enum;
using We7.Framework.Util;
using We7.Model.Core;

namespace We7.CMS.Web.Admin.ContentModel
{
    public partial class EditLayout : BasePage
    {
        //protected override MasterPageMode MasterPageIs
        //{
        //    get
        //    {
        //        return MasterPageMode.NoMenu;
        //    }
        //}

        #region Props

        /// <summary>
        /// 选项卡ID
        /// </summary>
        public string TabID
        {
            get
            {
                return RequestHelper.Get<string>("tab", "1");
            }
        }

        public string ModelName
        {
            get
            {
                return RequestHelper.Get<string>("modelname");
            }

        }

        public string GroupName
        {
            get
            {
                return RequestHelper.Get<string>("group");
            }
        }

        /// <summary>
        /// 内容模型类型
        /// </summary>
        public ModelType ContentModelType;

        #endregion

        /// <summary>
        /// Page_load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            MenuTabLabel.Text = BuildNavString();
        }

        #region private method

        string BuildNavString()
        {
            string strActive = @"<LI class=TabIn id=tab{0} style='display:{2}'><A>{1}</A> </LI>";
            string strLink = @"<LI class=TabOut id=tab{0}  style='display:{2}'><A  href={3}>{1}</A> </LI>";
            int tab = 1;
            string tabString = "";
            string dispay = "";
            //string rawurl = Helper.RemoveParamFromUrl(Request.RawUrl, "tab");
            //rawurl = Helper.RemoveParamFromUrl(Request.RawUrl, "saved");
            //rawurl = Helper.AddParamToUrl(rawurl, "group", GroupName);
            //rawurl = Helper.AddParamToUrl(rawurl, "model", ModelName);

            string rawUrl = RequestHelper.RemoveParam(Request.RawUrl, "tab");

            rawUrl = RequestHelper.RemoveParam(rawUrl, "modelname");
            rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");

            ModelInfo modelInfo = ModelHelper.GetModelInfoByName(ModelName);
            ContentModelType = modelInfo.Type;

            #region Article
            if (modelInfo.Type == ModelType.ARTICLE)
            {
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "modelname", RequestHelper.Get<string>("modelname"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", RequestHelper.Get<string>("tab", "1"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", RequestHelper.Get<string>("panel", "edit"));

                //记录操作历史，按照操作历史延续


                if (TabID != null && We7Helper.IsNumber(TabID) && int.Parse(TabID) > 0)
                    tab = int.Parse(TabID);

                if (tab == 1)
                {
                    tabString += string.Format(strActive, 1, "后台信息录入", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_Edit.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "1");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "edit");
                    tabString += string.Format(strLink, 1, "后台信息录入", dispay, rawUrl);
                }

                if (tab == 4)
                {
                    tabString += string.Format(strActive, 4, "后台列表显示", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_List.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "4");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "list");
                    tabString += string.Format(strLink, 4, "后台列表显示", dispay, rawUrl);
                }
                if (tab == 2)
                {
                    tabString += string.Format(strActive, 2, "会员中心列表显示", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_List.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "2");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "multi");
                    tabString += string.Format(strLink, 2, "会员中心列表显示", dispay, rawUrl);
                }
                if (tab == 3)
                {
                    tabString += string.Format(strActive, 3, "会员中心录入", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_Edit.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "3");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "multi");
                    tabString += string.Format(strLink, 3, "会员中心录入", dispay, rawUrl);
                }
            }

            #endregion

            #region ADVICE
            else if (modelInfo.Type == ModelType.ADVICE)
            {
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "modelname", RequestHelper.Get<string>("modelname"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", RequestHelper.Get<string>("tab", "1"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", RequestHelper.Get<string>("panel", "edit"));
                if (TabID != null && We7Helper.IsNumber(TabID) && int.Parse(TabID) > 0)
                    tab = int.Parse(TabID);

                if (tab == 1)
                {
                    tabString += string.Format(strActive, 1, "前台编辑", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_Edit.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "1");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "edit");
                    tabString += string.Format(strLink, 1, "前台编辑", dispay, rawUrl);
                }

                if (tab == 2)
                {
                    tabString += string.Format(strActive, 2, "后台编辑", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_Edit.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "2");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "adminView");
                    tabString += string.Format(strLink, 2, "后台编辑", dispay, rawUrl);
                }
            }
            #endregion

            #region Account
            else if (modelInfo.Type == ModelType.ACCOUNT)
            {
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "modelname", RequestHelper.Get<string>("modelname"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", RequestHelper.Get<string>("tab", "1"));
                rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", RequestHelper.Get<string>("panel", "edit"));
                if (TabID != null && We7Helper.IsNumber(TabID) && int.Parse(TabID) > 0)
                    tab = int.Parse(TabID);

                if (tab == 1)
                {
                    tabString += string.Format(strActive, 1, "录入信息", dispay);
                    Control ctl = this.LoadControl("Controls/Panel_Edit.ascx");
                    ContentHolder.Controls.Add(ctl);
                }
                else
                {
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "panel");
                    rawUrl = RequestHelper.RemoveParam(rawUrl, "tab");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "tab", "1");
                    rawUrl = RequestHelper.AddOrUpdateParam(rawUrl, "panel", "edit");
                    tabString += string.Format(strLink, 1, "录入信息", dispay, rawUrl);
                }
            }
            #endregion

            return tabString;
        }

        /// <summary>
        /// 构建当前位置导航
        /// </summary>
        /// <returns></returns>
        string BuildPagePath()
        {
            return string.Empty;
        }

        #endregion
    }
}
