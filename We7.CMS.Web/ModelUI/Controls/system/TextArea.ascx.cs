using System;
using We7.Model.Core.UI;
using System.Data;
using We7.Framework.Util;
using System.Web.UI.WebControls;
using System.Web;
using We7.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using We7;


namespace CModel.Controls.system
{
    public partial class TextArea : FieldControl
    {
        public override void InitControl()
        {
            
            if (!String.IsNullOrEmpty(Control.Width))
            {
                txtInput.Width = Unit.Parse(Control.Width);
            }
            if (!String.IsNullOrEmpty(Control.Height))
            {
                txtInput.Height = Unit.Parse(Control.Height);
            }

            if (Column.DataType == TypeCode.String
                || Column.DataType == TypeCode.Char)
            {
                txtInput.Text = Value == null ? Control.DefaultValue : Value.ToString();
                txtInput.Text = DeCode(txtInput.Text);
            }
            else
            {
                txtInput.Text = Value == null ? Control.DefaultValue : Value.ToString();
            }

            Validator(txtInput);

        }

        public override object GetValue()
        {
            string htmlFormat = Control.Params["htmlFormat"];
            if (Column.DataType == TypeCode.String
                || Column.DataType == TypeCode.Char)
            {
                txtInput.Text = HttpUtility.HtmlEncode(txtInput.Text);
                return EnCode(txtInput.Text);
            }
            else
            {
                return TypeConverter.StrToObjectByTypeCode(We7Helper.FilterHtmlChars(txtInput.Text), Column.DataType);
            }
        }

        string EnCode(string txt)
        {
            Regex regex = new Regex(@"\r\n", RegexOptions.Compiled | RegexOptions.Singleline);
            txt=regex.Replace(txt, "<br/>");
            regex=new Regex(@"\s",RegexOptions.Compiled|RegexOptions.Singleline);
            txt=regex.Replace(txt,"&nbsp;&nbsp;");
            return txt;
        }

        string DeCode(string txt)
        {
            Regex regex = new Regex(@"<br\s*?/>", RegexOptions.Compiled | RegexOptions.Singleline|RegexOptions.IgnoreCase);
            txt = regex.Replace(txt, "\r\n");
            regex = new Regex(@"&nbsp;&nbsp;", RegexOptions.Compiled | RegexOptions.Singleline);
            txt = regex.Replace(txt, " ");
            return txt;
        }
    }
}