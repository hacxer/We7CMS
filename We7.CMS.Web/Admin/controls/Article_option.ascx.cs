using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Text;
using We7.CMS;
using We7.CMS.Controls;
using We7.CMS.Config;
using We7.CMS.Common;
using We7.CMS.Common.PF;
using We7.CMS.Common.Enum;
using We7.Framework.Config;
using We7.Framework;
using We7.Framework.Util;

namespace We7.CMS.Web.Admin.controls
{
    public partial class Article_option : BaseUserControl
    {

        public bool IsMyArticle
        {
            get { return Request["ismyarticle"] != null; }
        }


        public string OwnerID
        {
            get
            {
                if (Request["oid"] != null)
                    return Request["oid"];
                else
                {
                    if (ViewState["$VS_OwnerID"] == null)
                    {
                        if (ArticleID != null)
                        {
                            Article a = ArticleHelper.GetArticle(ArticleID, null);
                            ViewState["$VS_OwnerID"] = a.OwnerID;
                        }
                    }
                    return (string)ViewState["$VS_OwnerID"];
                }
            }
        }

        public string ArticleID
        {
            get { return Request["id"]; }
        }

        public bool IsWap
        {
            get { return Request["wap"] != null; }
        }

        public string ParentID
        {
            get
            {
                return Request["ParentID"];
            }
        }

        /// <summary>
        /// 当前文章：包含已有文章或新文章
        /// </summary>
        Article ThisArticle
        {
            get
            {
                Article a = new Article();
                if (ArticleID != null)
                    a = ArticleHelper.GetArticle(ArticleID);
                else
                {
                    if (ViewState["$NewArticle"] != null)
                        a = ViewState["$NewArticle"] as Article;
                    else
                    {
                        a.ID = We7Helper.CreateNewID();
                        ViewState["$NewArticle"] = a;
                    }
                }
                return a;
            }
        }

        #region 菜单优化

        /// <summary>
        /// 内容类型：文章、产品、展会等，来自?type=
        /// </summary>
        public string InfomationType
        {
            get
            {
                return Request["type"];
            }
        }
        public string ButtonVisble
        {
            get
            {
                if (InfomationType != null)
                {
                    return "";
                }
                else
                {
                    return "none";
                }
            }
        }

        public string ValidateVisble
        {
            get
            {
                if (CDHelper.Config.EnableLoginAuhenCode == "true")
                {
                    return "";
                }
                else
                {
                    return "none";
                }
            }
        }
        #endregion

        private void GenerateRandomCode()
        {
            Response.Cookies["AreYouHuman"].Value = CaptchaImage.GenerateRandomCode();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (ArticleProcessHelper.IsExistProcess(ArticleID))
                {
                    SaveButton.Style["display"] = "none";
                    Processing ap = ArticleProcessHelper.GetArticleProcess(ArticleHelper.GetArticle(ArticleID));
                    SaveButton2.Style["display"] = "none";
                    if (ap.ArticleState == ArticleStates.Stopped)
                    {
                        SaveButton2.Style["display"] = "block";
                    }
                }
                //初始化
                InitControls();
                //读文章
                LoadArticle();


                Channel channel = ChannelHelper.GetChannel(OwnerID, null);
                //栏目是否为审核栏目
                if (channel != null && channel.Process == "1")
                {
                    if (ArticleID != null && ArticleID != "")
                    {
                        Article a = ArticleHelper.GetArticle(ArticleID);
                        Processing ap = ArticleProcessHelper.GetArticleProcess(a);
                        //加判断启用dropdownlist
                        if (a.ProcessState != "99" && !string.IsNullOrEmpty(ap.ApproveName))
                        {
                            StateDropDownList.Enabled = false;
                        }
                    }
                    if (ArticleID != null && ArticleID != "")
                    { }
                    else
                    {
                        StateDropDownList.Items[0].Selected = false;
                        StateDropDownList.Items[2].Selected = true;
                    }
                }
                InitEditorParameter();

                if (InfomationType == null)
                {
                    ChannelDropDownList.Visible = false;
                }
                else
                    BindChannel();

                GenerateRandomCode();
            }
            if (Request["Submit"] != null)
            {
                if (Request["Submit"].ToString() == "0")
                {
                    Messages.ShowMessage(
                        "成功创建文章信息。<a href='/admin/addins/ArticleEdit.aspx?type=0'>继续添加</a>");
                }
                if (Request["Submit"].ToString() == "1")
                {
                    Messages.ShowMessage("文章信息已成功修改。");
                }
            }
        }

        #region 加载栏目列表：无归属新文章选择

        void BindChannel()
        {
            string enumState = "";
            if (Int32.Parse(InfomationType) < 2)
            {
                enumState = "0" + InfomationType;
            }
            else
            {
                enumState = InfomationType;
            }

            //此处加载权限过滤栏目
            List<Channel> channelList = new List<Channel>();
            if (We7Helper.IsEmptyID(AccountID))
            {
                channelList = ChannelHelper.GetChannelsByType(enumState);

            }
            else
            {
                List<string> channelIds = AccountHelper.GetObjectsByPermission(AccountID, "Channel.Input");
                if (channelIds != null && channelIds.Count > 0)
                {
                    channelList = ChannelHelper.GetChannelByIDList(enumState, channelIds);
                }

            }
            List<Channel> removeList = new List<Channel>();
            foreach (Channel ch in channelList)
            {
                if (String.IsNullOrEmpty(ch.ModelName))
                    continue;
                if (String.Compare(ch.ModelName, "Article", true) == 0)
                    continue;
                if (String.Compare(ch.ModelName, "System.Article", true) == 0)
                    continue;
                removeList.Add(ch);
            }
            foreach (Channel ch in removeList)
            {
                channelList.Remove(ch);
            }

            if (channelList != null && channelList.Count > 0)
            {
                channelList.Sort();
                foreach (Channel ch in channelList)
                {
                    string name = ch.FullPath.Replace("//", "/").Replace("/", " 》");
                    string value = ch.ID;
                    ListItem item = new ListItem(name, value);
                    ChannelDropDownList.Items.Add(item);
                }
            }
        }

        #endregion

        #region 控件初始化
        void InitControls()
        {
            ActicleTypeDropDownList.Attributes["onchange"] = "ActicleTypeDropDownList_onchange(this,'" + this.ClientID + "')";
            SaveButton.Attributes["onclick"] = "return articleOptionCheck('" + this.ClientID + "');";
            SaveButton2.Attributes["onclick"] = "return articleOptionCheck('" + this.ClientID + "');";
            TitleTextBox.Attributes["onfocus"] = "autoSetTitleValue(this,'" + ContentTextBox.ClientID + "')";

            if (IsWap)
            {
                ActicleTypeDropDownList.Items.Clear();
                ListItem item = new ListItem("wap文章", "16");
                ActicleTypeDropDownList.Items.Add(item);
                ActicleTypeDropDownList.Enabled = false;
            }

            if (OwnerID != null && OwnerID != "")
                channelList.Visible = false;



            //设置作者默认为登录用户名
            this.AuthorTextBox.Value = GetAccountName(CurrentAccount);

            if (!string.IsNullOrEmpty(ParentID))
            {
                Article pa = ArticleHelper.GetArticle(ParentID);
                if (pa != null)
                {
                    ParentArticleTitle.Text = pa.Title;
                    ParentArticleID.Value = pa.ID;
                }
                ParentArticleDiv.Visible = true;
            }
        }

        /// <summary>
        /// 初始化需要传递给编辑器的文件存放路径
        /// </summary>
        void InitEditorParameter()
        {
            Session["ARTICLEDETAIL_CHANNELFILEPATH"] = ThisArticle.AttachmentUrlPath;
            Session["WorkPlace"] = "articleedit";

            string path = Server.MapPath("~/Config/general.config");
            if (!File.Exists(path))
            {
                ArticleHelper.Write(path);
                //记录日志
                string content = string.Format("创建了内容模型文件“{0}”", "general.config");
                AddLog("创建内容模型文件", content);
            }
            GeneralConfigInfo config = GeneralConfigs.Deserialize(We7Utils.GetMapPath("~/Config/general.config"));
            if (config.CutToMaxWidthOfUploadedImg == 1)
                Session["GENERALCONFIGINFO_CMAXWIDTHOFUPLOADEDIMG"] = config.MaxWidthOfUploadedImg;
        }

        #endregion

        #region 加载与保存文章

        void LoadArticle()
        {
            SourceTextBox.Value = GeneralConfigs.GetConfig().ArticleSourceDefault;

            if (ArticleID != null)
            {
                Article a = ThisArticle;
                TitleTextBox.Value = a.Title;
                SubTitleTextBox.Value = a.SubTitle;
                DescriptionTextBox.Value = a.Description;
                CreatedLabel.Text = a.Created.ToString();
                UpdatedTextBox.Value = a.Updated.ToString();
                ContentUrlTextBox.Value = a.ContentUrl;
                CreatorLabel.Text = GetAccountName(a.AccountID);
                txtInvalidDate.Value = a.Overdue.ToString();
                AuthorTextBox.Value = a.Author;
                IndexTextBox.Value = a.Index.ToString();
                SourceTextBox.Value = a.Source;
                //SourceTextBox.Disabled = true;
                if (a.KeyWord != null)
                {
                    KeywordTextBox.Value = a.KeyWord.ToString();
                }
                if (a.DescriptionKey != null)
                {
                    DescriptionKeyTextBox.Value = a.DescriptionKey.ToString();
                }
                SetDropdownList(StateDropDownList, a.State.ToString());
                EnumLibrary.HomeRecommend type = (EnumLibrary.HomeRecommend)StateMgr.GetStateValueEnum(a.EnumState, EnumLibrary.Business.HomeRecommend);
                if (type == EnumLibrary.HomeRecommend.RecommendArticle)
                {
                    IsShowCheckBox.Checked = true;
                }
                //IsShowCheckBox.Checked = (a.IsShow == 1) ? true : false;

                if (a.ContentType.ToString() != "")
                    ActicleTypeDropDownList.SelectedValue = a.ContentType.ToString();

                if ((TypeOfArticle)a.ContentType == TypeOfArticle.LinkArticle)
                {
                    BodyDiv.Visible = false;
                    linkSpan.Attributes["style"] = "";
                }

                chkBold.Checked = !String.IsNullOrEmpty(a.FontWeight);
                chkItalic.Checked = !String.IsNullOrEmpty(a.FontStyle);
                ddlColor.SelectedValue = a.Color;

                //TODO:thj add it to get the state of AllowCommentsCheckBox
                if (a.AllowComments == 1)
                    AllowCommentsCheckBox.Checked = true;
                else
                    AllowCommentsCheckBox.Checked = false;

                if (a.Content != null && a.Content != "")
                    ContentTextBox.Value = We7Helper.ConvertPageBreakFromCharToVisual(a.Content);

                if (!string.IsNullOrEmpty(a.ParentID) && !We7Helper.IsEmptyID(a.ParentID))
                {
                    Article pa = ArticleHelper.GetArticle(a.ParentID);
                    if (pa != null)
                    {
                        ParentArticleTitle.Text = pa.Title;
                        ParentArticleID.Value = pa.ID;
                    }
                    ParentArticleDiv.Visible = true;
                }
            }
        }

        void SaveArticle()
        {
            try
            {
                if (CDHelper.Config.EnableLoginAuhenCode == "true" && this.ValidateTextBox.Text != Request.Cookies["AreYouHuman"].Value)
                {
                    Messages.ShowError("错误：您输入的验证码不正确，请重新输入！");
                    // Clear the input and create a new random code.
                    this.ValidateTextBox.Text = "";
                    Response.Cookies["AreYouHuman"].Value = CaptchaImage.GenerateRandomCode();
                    return;
                }

                string ownerId = OwnerID;
                if (ownerId == null)
                {
                    ownerId = ChannelDropDownList.SelectedValue;
                }

                if (String.IsNullOrEmpty(ownerId))
                {
                    Messages.ShowError("文章栏目不能为空!");
                    return;
                }

                Article a = ThisArticle;
                a.Description = DescriptionTextBox.Value;
                a.Title = TitleTextBox.Value;
                a.SubTitle = SubTitleTextBox.Value;
                a.Color = ddlColor.SelectedValue;
                a.FontStyle = chkItalic.Checked ? "Italic" : "";
                a.FontWeight = chkBold.Checked ? "Bold" : "";
                //a.ID = ArticleID;
                if (IndexTextBox.Value.Trim() == "")
                    a.Index = 0;
                else
                    a.Index = Convert.ToInt32(IndexTextBox.Value);
                a.Source = SourceTextBox.Value;
                a.AllowComments = AllowCommentsCheckBox.Checked ? 1 : 0;
                //if (IsShowCheckBox.Checked)
                //{
                //    a.EnumState = StateMgr.StateInitialize();
                //    a.EnumState = StateMgr.StateProcess(a.EnumState, EnumLibrary.Business.HomeRecommend, 1);
                //}
                //a.IsShow = IsShowCheckBox.Checked ? 1 : 0;
                a.Author = AuthorTextBox.Value;
                a.State = Convert.ToInt32(StateDropDownList.SelectedValue);
                a.ContentType = Convert.ToInt32(ActicleTypeDropDownList.SelectedValue);
                //a.IsImage = (TypeOfArticle)a.ContentType == TypeOfArticle.QuoteArticle || (TypeOfArticle)a.ContentType == TypeOfArticle.ShareArticle ? 1 : 0;
                a.ContentUrl = ContentUrlTextBox.Value;
                a.Content = We7Helper.ConvertPageBreakFromVisualToChar(ContentTextBox.Value);
                a.KeyWord = KeywordTextBox.Value;
                a.DescriptionKey = DescriptionKeyTextBox.Value;

                if (GeneralConfigs.GetConfig().AllowParentArticle && !string.IsNullOrEmpty(ParentArticleID.Value))
                    a.ParentID = ParentArticleID.Value;
                else
                    a.ParentID = We7Helper.EmptyGUID;

                if (UpdatedTextBox.Value.Trim() == "")
                    a.Updated = DateTime.Now;
                else
                    a.Updated = Convert.ToDateTime(UpdatedTextBox.Value);
                if (txtInvalidDate.Value != "")
                {
                    a.Overdue = Convert.ToDateTime(txtInvalidDate.Value.Trim());
                }
                else
                {
                    GeneralConfigInfo si = GeneralConfigs.GetConfig();
                    int OverdueDateTime = si.OverdueDateTime;
                    a.Overdue = a.Updated.AddDays(OverdueDateTime);
                }
                if (ArticleID == null)
                {
                    a.AccountID = AccountID;
                    a.OwnerID = ownerId;
                    Channel ch = ChannelHelper.GetChannel(ownerId, null);
                    if (ch.FullUrl != null && ch.FullUrl != "")
                    {
                        a.ChannelFullUrl = ch.FullUrl;
                    }
                    if (ch.Process != null && ch.Process == "1")
                    {
                        a.State = 2;
                    }
                    //a.ChannelName = ch.ChannelName;
                    /*这儿把上面一句注了，改成了下面的。上面一句与老系统不兼容*/
                    a.ChannelName = ch.Name;
                    a.FullChannelPath = ch.FullPath;
                    //int type = StateMgr.GetStateValue(ch.EnumState, EnumLibrary.Business.ChannelContentType);
                    //a.EnumState = StateMgr.StateProcess(a.EnumState, EnumLibrary.Business.ArticleType, type);
                    // a.State = 0;

                    //如果禁用
                    if (StateDropDownList.SelectedValue == "0")
                    {
                        a.state = 0;
                    }
                    Article article = ArticleHelper.AddArticles(a);
                    // 往全文检索里插入数据

                    ArticleIndexHelper.InsertData(article.ID, 0);

                    #region 自动提交一审（mxy2011-10-18）
                    if (ch.Process != null && ch.Process == "1" && StateDropDownList.SelectedValue != "0")
                    {
                        Processing ap = ArticleProcessHelper.GetArticleProcess(article);
                        if (ap.ArticleState != ArticleStates.Checking)
                        {
                            string accName = AccountHelper.GetAccount(AccountID, new string[] { "LastName" }).LastName;
                            ap.ProcessState = ProcessStates.FirstAudit;
                            ap.ProcessDirection = ((int)ProcessAction.Next).ToString();
                            ap.ProcessAccountID = AccountID;
                            ap.ApproveName = accName;
                            ArticleProcessHelper.SaveFlowInfoToDB(article, ap);
                        }
                    }
                    #endregion

                    //记录日志
                    string content = string.Format("新建文章:“{0}”", a.Title);
                    AddLog("新建文章", content);

                }
                else
                {
                    Channel ch = ChannelHelper.GetChannel(ownerId, null);
                    string[] fields = new string[] { "Description", "Title", "Content", "Updated", "Index", "Source", "AllowComments", "Author", "State", "IsShow", "IsImage", "SubTitle", "ContentUrl", "ContentType", "IsDeleted", "Overdue", "KeyWord", "DescriptionKey", "ParentID", "FullChannelPath", "ChannelFullUrl", "Color", "FontWeight", "FontStyle" };

                    ArticleHelper.UpdateArticle(a, fields);

                    // 往全文检索里更新数据
                    ArticleIndexHelper.InsertData(a.ID, 0);

                    if (ch.Process != null && ch.Process == "1" && StateDropDownList.SelectedValue != "0")
                    {

                        Processing ap = ArticleProcessHelper.GetArticleProcess(a);
                        if (ap.ArticleState != ArticleStates.Checking && ap.ProcessState != ProcessStates.EndAudit)
                        {

                            //编辑审核启用
                            if (ch.Process != null && ch.Process == "1" && StateDropDownList.SelectedValue != "0")
                            {
                                a.State = 2;
                                ArticleHelper.UpdateArticle(a, new string[] { "State" });
                            }

                            string accName = AccountHelper.GetAccount(AccountID, new string[] { "LastName" }).LastName;
                            ap.ProcessState = ProcessStates.FirstAudit;
                            ap.ProcessDirection = ((int)ProcessAction.Next).ToString();
                            ap.ProcessAccountID = AccountID;
                            ap.ApproveName = accName;
                            ArticleProcessHelper.SaveFlowInfoToDB(a, ap);
                        }
                    }
                    //记录日志
                    string content = string.Format("修改了文章“{0}”", a.Title);
                    AddLog("编辑文章", content);
                }

                string rawurl = We7Helper.RemoveParamFromUrl(Request.RawUrl, "saved");
                if (!String.IsNullOrEmpty(ArticleID))
                {
                    rawurl = We7Helper.AddParamToUrl(rawurl, "saved", "1");
                    rawurl = We7Helper.AddParamToUrl(rawurl, "Submit", "1");
                }
                else
                {
                    rawurl = We7Helper.RemoveParamFromUrl(Request.RawUrl, "oid");
                    rawurl = We7Helper.RemoveParamFromUrl(rawurl, "ParentID");
                    rawurl = We7Helper.AddParamToUrl(rawurl, "id", a.ID);
                    rawurl = We7Helper.AddParamToUrl(rawurl, "Submit", "0");
                }

                Response.Redirect(rawurl);

            }
            catch (FormatException)
            {
                Messages.ShowError("无法保存文章信息：可能是文章排序或修改日期格式不正确。");
                return;
            }
            catch (Exception ex)
            {
                Messages.ShowError("无法保存文章信息：" + ex.Message);
            }

        }
        #endregion

        protected void SaveButton_ServerClick(object sender, EventArgs e)
        {
            SaveArticle();
        }

        protected void SetDropdownList(DropDownList list, string value)
        {
            int i = 0;
            foreach (ListItem item in list.Items)
            {
                if (item.Value == value)
                {
                    list.SelectedIndex = i;
                    return;
                }
                i++;
            }
        }

        protected void ChannelDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ownerId = OwnerID;
            if (ownerId == null)
            {
                ownerId = ChannelDropDownList.SelectedValue;
            }
            if (ChannelHelper.GetChannel(ownerId, null).Process == "1")
            {
                if (ArticleID != null && ArticleID != "")
                {
                    Article a = ArticleHelper.GetArticle(ArticleID);
                    if (a.ProcessState.Trim() != "99")
                    {
                        StateDropDownList.Enabled = false;
                    }
                }
                StateDropDownList.Items[0].Selected = false;
                StateDropDownList.Items[1].Selected = true;
            }
        }
    }
}