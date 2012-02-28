using System;
using System.Collections.Generic;
using System.Text;
using Thinkment.Data;
using System.Data;

namespace We7.Model.Core.Data.ThinkmentDriver
{
    class OperateHandle : BaseHandle
    {
        List<Order> orderList;
        Dictionary<string, ListField> listFieldDict;
        Dictionary<string, ConListField> conListFieldDict;
        string fields;
        string orders;

        public OperateHandle(string modelName)
            : base(modelName)
        {
            orderList = new List<Order>();
            listFieldDict = new Dictionary<string, ListField>();
            conListFieldDict = new Dictionary<string, ConListField>();
        }

        public OperateHandle()
        {
            orderList = new List<Order>();
            listFieldDict = new Dictionary<string, ListField>();
            conListFieldDict = new Dictionary<string, ConListField>();
        }

        public string Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        public Dictionary<string, ListField> ListFieldDict
        {
            get { return listFieldDict; }
        }

        public Dictionary<string, ConListField> ConListFieldDict
        {
            get { return conListFieldDict; }
        }

        public List<Order> OrderList
        {
            get { return orderList; }
            set { orderList = value; }
        }

        public void AddFields(string f)
        {
            ListField f0 = new ListField(f);
            ListFieldDict.Add(f0.FieldName, f0);
        }

        protected void BuildOrders()
        {
            StringBuilder f0 = new StringBuilder();
            foreach (Order f1 in orderList)
            {
                CriteriaType ct = f1.Mode == OrderMode.Desc ? CriteriaType.Desc : CriteriaType.Asc;
                string ms = " " + Connect.Driver.GetCriteria(ct) + " ";
                AddSplitString(f0, f1.Name + ms);
            }
            orders = f0.ToString();
        }


        protected virtual void BuildFields()
        {
            BuildFields(false);
        }

        protected virtual void BuildFields(bool allowReadonly)
        {
            StringBuilder sb = new StringBuilder();
            foreach (We7DataColumn dc in Columns)
            {
                if (dc.Direction == ParameterDirection.ReturnValue)
                    continue;
                Adorns a = Adorns.None;
                if (ListFieldDict.Count > 0)
                {
                    if (ListFieldDict.ContainsKey(dc.Name))
                    {
                        a = listFieldDict[dc.Name].Adorn;
                    }
                    else
                    {
                        continue;
                    }
                }
                AddSplitString(sb, Connect.Driver.FormatField(a, dc.Name));
            }
            fields = sb.ToString();
        }

        protected  string GetSystemFields(string condition)
        {
            StringBuilder sb = new StringBuilder();
            foreach (We7DataColumn dc in Columns)
            {
                if (dc.Direction == ParameterDirection.ReturnValue)
                    continue;
                string field = Connect.Driver.FormatField(Adorns.None, dc.Name);
                if (!string.IsNullOrEmpty((condition)) && !condition.Contains(field) && dc.IsSystem)
                {
                    AddSplitString(sb, field);
                }
            }
            return sb.ToString();
        }


        protected void BuildFields(bool allowReadonly, bool forConten)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, ConListField> item in ConListFieldDict)
            {
                AddSplitString(sb, Connect.Driver.FormatField(item.Value));
            }
            fields = sb.ToString();
        }

        public string Orders
        {
            get { return orders; }
        }

        protected override void Build()
        {
        }

        protected override void Build(bool forContent)
        {
        }

        protected void ThrowException(SqlStatement statement, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Sql：：{0}　　　", statement.SqlClause);
            if (statement.Parameters != null)
            {
                for (int i = 0; i < statement.Parameters.Count; i++)
                {
                    sb.AppendFormat("param{0}：：name={1},value={2}　　　", i, statement.Parameters[i].ParameterName, statement.Parameters[i].Value ?? "null");
                }
            }
            sb.AppendFormat("Message：：{0}　　　", ex.Message);
            throw new Exception(sb.ToString());
        }
    }
}
