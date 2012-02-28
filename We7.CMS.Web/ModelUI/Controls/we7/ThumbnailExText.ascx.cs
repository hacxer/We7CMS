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
using We7.Model.UI.Controls;
using We7.Framework;
using We7.CMS.Common;
using System.Xml;
using We7.Framework.Util;
using System.Text.RegularExpressions;

namespace We7.CMS.Web.Admin.ContentModel.Controls
{
    public partial class ThumbnailExText : We7FieldControl
    {
        string pattern = "src:\'(?<img>[^\']*)";

        public override void InitControl()
        {
            if (Value != null)
            {
                MatchCollection ms = Regex.Matches(Value.ToString(), pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (ms.Count > 0)
                {
                    Img.ImageUrl = ms[1].Groups["img"].Value;
                }
            }
        }

        public override object GetValue()
        {
            return null;
        }

    }
}