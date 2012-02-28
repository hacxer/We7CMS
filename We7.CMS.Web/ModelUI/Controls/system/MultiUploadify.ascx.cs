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
using System.Text;
using We7.CMS.Accounts;

namespace We7.Model.UI.Controls.system
{
	public partial class MultiUploadify : We7FieldControl
	{
		public override object GetValue()
		{
			string[] values = pageValues.Value.Split('|');
			List<Dictionary<string, object>> dics = new List<Dictionary<string, object>>();
			for (int i = 0; i < values.Length; i = i + 3)
			{
				Dictionary<string, object> dic = new Dictionary<string, object>();
				string[] keyV = values[i].Split(':');
				dic.Add(keyV[0], HttpUtility.UrlDecode(keyV[1]));
				keyV = values[i + 1].Split(':');
				dic.Add(keyV[0], HttpUtility.UrlDecode(keyV[1]));
				keyV = values[i + 2].Split(':');
				dic.Add(keyV[0], HttpUtility.UrlDecode(keyV[1]));
				dic.Add("ID", We7Helper.CreateNewID());
				dic.Add("AccountID", Security.CurrentAccountID);
				dic.Add("Created", System.DateTime.Now);
				dic.Add("State", 1);
				dic.Add("Index", 999);
				dics.Add(dic);
			}
			return dics;
		}

		public string col1Name = string.Empty;
		public string col1Len = "0";
		public string col2Name = string.Empty;
		public string col2Len = "0";

		public override void InitControl()
		{
			if (!string.IsNullOrEmpty(Control.Params["col1"]))
			{
				col1Name = PanelContext.DataSet.Tables[0].Columns[Control.Params["col1"]].Label;
				col1Len = PanelContext.DataSet.Tables[0].Columns[Control.Params["col1"]].MaxLength.ToString();
			}
			if (!string.IsNullOrEmpty(Control.Params["col2"]))
			{
				col2Name = PanelContext.DataSet.Tables[0].Columns[Control.Params["col2"]].Label;
				col2Len = PanelContext.DataSet.Tables[0].Columns[Control.Params["col2"]].MaxLength.ToString();
			}
		}
	}


}