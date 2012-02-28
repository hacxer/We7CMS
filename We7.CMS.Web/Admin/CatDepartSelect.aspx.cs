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
using System.Text.RegularExpressions;

using Thinkment.Data;
using We7.CMS.Common.Enum;
using We7.CMS.Common;
using We7.Framework;
using We7.CMS.Accounts;
using We7.Framework.Config;

namespace We7.CMS.Web.Admin
{
	public partial class CatDepartSelect : BasePage
	{
		protected override MasterPageMode MasterPageIs
		{
			get
			{
				return MasterPageMode.None;
			}
		}

		protected override bool NeedAnPermission
		{
			get
			{
				return false;
			}
		}
		static ArrayList CategoryList;
		string KeyWord
		{
			get { return Request["keyword"]; }
		}
		public string ColumnID
		{
			get
			{
				if (Request["wap"] == null)
				{
					string id = Request["id"];
					if (id == null)
					{
						return We7Helper.EmptyGUID;
					}
					return id;
				}
				else
				{
					string id = Request["id"];
					if (id == null)
					{
						return We7Helper.EmptyWapGUID;
					}
					return id;
				}
			}
		}

		string ChannelType
		{
			get { return Request["type"]; }
		}
		CategoryHelper CategoryHelper { get { return HelperFactory.Instance.GetHelper<CategoryHelper>(); } }

		protected IAccountHelper AccountHelper
		{
			get { return AccountFactory.CreateInstance(); }
		}
		string siteID = SiteConfigs.GetConfig().SiteGroupEnabled ? SiteConfigs.GetConfig().SiteID : string.Empty;
		protected override void Initialize()
		{
			string rawurl = Request.RawUrl;
			rawurl = We7Helper.RemoveParamFromUrl(rawurl, "keyword");
			string qString = @"<label class=""hidden"" for=""user-search-input"">搜索{0}:</label>
                <input type=""text"" class=""search-input"" id=""KeyWord"" name=""KeyWord"" value=""""  onKeyPress=""javascript:KeyPressSearch('{1}',event);""  />
                <input type=""button"" value=""搜索"" class=""button"" id=""SearchButton""  onclick=""javascript:doSearch('{1}');""  />";
			qString = string.Format(qString, "类别", rawurl);
			SearchSimpleLiteral.Text = qString;
			rawurl = We7Helper.GetParamValueFromUrl(rawurl, "type");
			CategoryList = new ArrayList();
			if (rawurl == "dep")
			{
				NameLabel.Text = "选择一个部门";
				List<Common.PF.Department> allDepartments = null;
				allDepartments = AccountHelper.GetDepartments(siteID, We7Helper.EmptyGUID, "", new string[] { "ID", "Name", "Description", "State" });
				foreach (Common.PF.Department d in allDepartments)
				{
					DrawTreeMenu(d, "");
				}
				CategoryList = FilterByKeyword();
				DetailGridView.DataSource = CategoryList;
				DetailGridView.Columns[2].Visible = false;
			}
			else
			{
				NameLabel.Text = "选择一个类别";
				List<Category> allCategory = null;
				allCategory = CategoryHelper.GetCategoryByParentID(We7Helper.EmptyGUID);
				foreach (Category c in allCategory)
				{
					DrawTreeMenu(c, "");
				}
				CategoryList = FilterByKeyword();
				DetailGridView.DataSource = CategoryList;
			}
			DetailGridView.DataBind();
		}

		void ShowMessage(string s)
		{
			MessageLabel.Text = s;
		}


		protected void DrawTreeMenu(object current, string prefix)
		{
			Category currentCategory = new Category();
			DepartmenEx currentDepart = null;
			if (current is Category)
			{
				currentCategory = (Category)current;
				currentCategory.FullPath = prefix + "<img src=\"/admin/images/filetype/folder.gif\" />&nbsp;" + currentCategory.Name;
				if (!CategoryList.Contains(currentCategory))
					CategoryList.Add(currentCategory);
				List<Category> temp = CategoryHelper.GetCategoryByParentID(currentCategory.ID);
				foreach (Category ca in temp)
					DrawTreeMenu(ca, prefix + "&nbsp;├─&nbsp;");
			}
			else if (current is Common.PF.Department)
			{
				currentDepart = new DepartmenEx(current as Common.PF.Department);
				currentDepart.FullPath = prefix + "<img src=\"/admin/images/filetype/folder.gif\" />&nbsp;" + currentDepart.Name;
				if (!CategoryList.Contains(currentDepart))
					CategoryList.Add(currentDepart);
				List<Common.PF.Department> temp = AccountHelper.GetDepartments(siteID, currentDepart.ID, "", new string[] { "ID", "Name", "Description", "State" });
				foreach (Common.PF.Department de in temp)
					DrawTreeMenu(de, prefix + "&nbsp;├─&nbsp;");
			}
		}

		/// <summary>
		/// 关键词过滤
		/// </summary>
		/// <returns></returns>
		ArrayList FilterByKeyword()
		{
			if (KeyWord == null || KeyWord == "") return CategoryList;

			ArrayList temp = new ArrayList();
			temp = (ArrayList)CategoryList.Clone();

			for (int i = temp.Count - 1; i >= 0; i--)
			{
				Category ca = null;
				DepartmenEx da = null;
				if (temp[i] is Category)
				{
					ca = (Category)temp[i];
					if (!Like(ca.Name, KeyWord))
					{
						temp.RemoveAt(i);
					}
				}
				else if (temp[i] is DepartmenEx)
				{
					da = (DepartmenEx)temp[i];
					if (!Like(da.Name, KeyWord))
					{
						temp.RemoveAt(i);
					}
				}
			}
			return temp;
		}

		/// <summary>
		/// 模糊匹配,若strPattern(匹配关键字)与strText(要比较的原字符串)模糊匹配,则返回true
		/// </summary>
		/// <param name="strText">要比较的原字符串</param>
		/// <param name="strPattern">匹配关键字</param>
		/// <returns>true or false</returns>
		public static bool Like(string strText, string strPattern)
		{
			strText = strText.ToLower();
			strPattern = strPattern.ToLower();

			//替换通配符*,?
			strPattern = strPattern.Replace("*", @"\w*");
			strPattern = strPattern.Replace("?", @"\w");

			string inputStrP = @"\w*" + strPattern + @"\w*";
			Regex myReg = new Regex(inputStrP);
			Match myMatch = myReg.Match(strText);
			if (myMatch.Success)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public class DepartmenEx : Common.PF.Department
	{
		public string FullPath { get; set; }
		public string KeyWord { get; set; }
		public DateTime CreateDate { get; set; }
		public DepartmenEx(Common.PF.Department dep)
		{
			this.ID = dep.ID;
			this.Name = dep.Name;
			this.Description = dep.Description;
			this.CreateDate = dep.Created;
		}

	}
}
