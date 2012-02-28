using System;
using System.Collections.Generic;
using System.Web;
using Thinkment.Data;
using We7.CMS.Data;
using System.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    public class TreeAssistant : DataBaseForThinkment, IJsonResult
    {
        private const string HASNODE_KEY = "hasnode";  

        public void SetTree(IQueryCondition condition)
        {
            TableInfo ti = new TableInfo(condition.TableName, condition.ID, condition.ConditionDic);
            IDataBaseAssiant IDatabase = new DataBaseForThinkment(condition.TableName).IDatabase;
            Order[] o;
            if (string.IsNullOrEmpty(condition.Sort)) o = new Order[] { new Order(condition.PriMaryKeyName, OrderMode.Asc) };
            else o = new Order[] { new Order(condition.Sort, (OrderMode)condition.Sord), new Order(condition.PriMaryKeyName, OrderMode.Asc) };  //处理排序（默认主键）
            try
            {
                List<TableInfo> aList = IDatabase.GetDtByCondition<TableInfo>(ti.TableName,condition.Condition, o, condition.Fields);

                if (condition.JoinInfo != null) aList[0].Table= IDatabase.Join(aList[0].Table, condition); //表连接

                if (aList[0].Table != null)
                {
                    if (!aList[0].Table.Columns.Contains(HASNODE_KEY)) aList[0].Table.Columns.Add(HASNODE_KEY, typeof(Boolean)); //添加hasnode节点
                    foreach (DataRow item in aList[0].Table.Rows) //处理hasnode
                    {
                        Criteria c = new Criteria(CriteriaType.Equals, condition.PIDKeyName, item[condition.PriMaryKeyName]);
                        int childNodeCount = IDatabase.Total(ti.TableName, c); //获取子节点数目
                        item[HASNODE_KEY] = childNodeCount > 0 ? true : false;
                    }
                }

                condition.JsonMessage.Add(Enum_operType.Seach.ToString(), aList[0].ToJson().Replace("{0}", condition.total.ToString()).Replace("{1}", "200").Replace("{2}", "数据成功返回").Replace("{3}", condition.Page.ToString()).Replace("{4}", condition.totalPage.ToString()));  //查询结果特殊处理
            }
            catch (Exception ex)
            {
                QueryCondition.SetMessage(condition, condition.TableName, true, "查询", ex.Message);
                We7.Framework.LogHelper.WriteLog(typeof(JsonResult), ex);
            }
        }
    }
}