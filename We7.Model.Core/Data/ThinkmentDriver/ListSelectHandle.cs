using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Thinkment.Data;

namespace We7.Model.Core.Data.ThinkmentDriver
{
    class ListSelectHandle: OperateHandle
    {
        public ListSelectHandle(string modelName):base(modelName)
        {
            Table = new DataTable();
        }

        public ListSelectHandle() { }

        public int From { get; set; }

        public int Count { get; set; }

        public DataTable Table { get; private set; }

        private void FormatFields()
        {
            if (Fields.IndexOf(",") <= 0)
            {
                Fields = Connect.Driver.FormatField(Adorns.None, Fields);
            }
            else
            {
                string[] tmp = Fields.Split(Convert.ToChar(","));
                Fields = "";
                foreach (var s in tmp)
                {
                    if(Fields.Length > 0 )
                        Fields += ",";
                    Fields += Connect.Driver.FormatField(Adorns.None, s);
                }
            }

            string systemFields = GetSystemFields(Fields);
            if (!string.IsNullOrEmpty((systemFields)))
                Fields += "," + systemFields;
        }

        protected override void Build()
        {
            if (String.IsNullOrEmpty(Fields))
                BuildFields();
            else
            {
                FormatFields();
            }
            if (ConditonCriteria != null)
            {
                BuildCindition();
            }
            else
            {
                Condition = String.Empty;
            }
            List<Order> os = new List<Order>();
            foreach (Order o in OrderList)
            {
                if (!Columns.Contains(o.Name))
                {
                    throw new ArgumentException("数据集中不存在当前字段:" + o.Name);
                }
                We7DataColumn dc = Columns[o.Name];
                if (dc.Direction == ParameterDirection.ReturnValue)
                    throw new ArgumentException("不能使用ReturnValue字段作为排序字段:" + o.Name);
                os.Add(new Order(o.Name, o.Mode));
            }
            if (os.Count == 0 && From <= 0)
            {
                os =BuildOrderList();
            }
            SQL.SqlClause = Connect.Driver.BuildPaging(
                Connect.Driver.FormatTable(ModelTable.Name), Fields, Condition, os, From, Count);
        }

        //public DataTable Execute(Criteria ct, List<Order> orders, int from, int count)
        //{
        //    ConditonCriteria = ct;
        //    OrderList = orders;
        //    From = from;
        //    Count = count;

        //    Execute();
        //    return Table;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="orders"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public DataTable Execute(Criteria ct, List<Order> orders, int @from, int count, string fields = null)
        {
            Fields = fields ?? "";
            ConditonCriteria = ct;
            OrderList = orders;
            From = from;
            Count = count;
            Execute();
            return Table;
        }


        private void Execute()
        {
            try
            {
                Build();
                Table = Connect.Query(SQL);
            }
            catch (Exception ex)
            {
                ThrowException(SQL, ex);
            }
        }

        

        //public void Execute(bool forContent)
        //{
        //    Table.Clear();
        //    Build(forContent);
        //    Table = Connect.Query(SQL);
        //}

        //protected override void Build(bool forContent)
        //{
        //    BuildFields(true, true);
        //    if (ConditonCriteria != null)
        //    {
        //        BuildCindition(ConListFieldDict);
        //    }
        //    else
        //    {
        //        Condition = String.Empty;
        //    }
        //    List<Order> os = new List<Order>();
        //    foreach (Order o in this.OrderList)
        //    {
        //        if (!ConListFieldDict.ContainsKey(o.Name))
        //        {
        //            throw new NotSupportedException("没有实现这个方法的内容，没有地方用到");
        //            //string msg = String.Format("Property '{0}' doesn't not belong to '{1}'.", o.Name, EntityObject.TypeName);
        //            //throw new DataException(msg, ErrorCodes.UnknownProperty);
        //        }

        //        ConListField f = ConListFieldDict[o.Name];
        //        o.AliasName = o.Name;
        //        o.Name = f.FieldName;
        //        os.Add(o);
        //    }

        //    string table = Connect.Driver.FormatTable(ModelTable.Name);
        //    SQL.SqlClause = Connect.Driver.BuildPaging(table, Fields, Condition, os, From, Count);
        //}

    }
}
