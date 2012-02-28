using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using We7.Model.UI.Controls;
using We7.Framework.Util;

namespace We7.Model.UI.Controls.we7
{
    public partial class ThreeCascadeText : We7FieldControl
    {
        string dataSourceType, emptyText, field1, field2, field3;

        string field1TextMapping, field1ValueMapping, field2TextMapping, field2ValueMapping, field3TextMapping, field3ValueMapping, tableName;
        We7.Model.Core.DataField field1DataField, field2DataField, field3DataField;

        public override void InitControl()
        {
            dataSourceType = Control.Params["dataSourceType"];
            emptyText = Control.Params["emptyText"];
            field1 = Control.Params["field1"];
            field2 = Control.Params["field2"];
            field3 = Control.Params["field3"];
            field1DataField = PanelContext.Row.IndexOf(field1);
            field2DataField = PanelContext.Row.IndexOf(field2);
            field3DataField = PanelContext.Row.IndexOf(field3);

            InitLable();

            if (field1DataField.Value != null)
                value1.Text = field1DataField.Value.ToString();
            if (field2DataField.Value != null)
                value2.Text = field2DataField.Value.ToString();
            if (field3DataField.Value != null)
                value3.Text = field3DataField.Value.ToString();
        }

        public override object GetValue()
        {
            return null;
        }


        void InitLable()
        {
            //一级类别标签
            string strField1Lable = PanelContext.DataSet.Tables[0].Columns[field1].Label;
            this.Field1Lable.Text = strField1Lable;
            //二级类别标签
            string strField2Lable = PanelContext.DataSet.Tables[0].Columns[field2].Label;
            this.Field2Label.Text = strField2Lable;
            //三级类别标签
            string strField3Lable = PanelContext.DataSet.Tables[0].Columns[field3].Label;
            this.Field3Label.Text = strField3Lable;
        }
    }
}