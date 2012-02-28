using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Thinkment.Data;
using We7.Model.Core;
using We7.Model.Core.Command;
using We7.Model.Core.UI;
using We7.CMS.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    /// <summary>
    /// 数据库Json结果
    /// </summary>
    public class JsonResult : DataBaseForThinkment, IJsonResult
    {

        public void ToJson(IQueryCondition condition)
        {
            tablename = condition.TableName;
            var ti = new TableInfo(condition.TableName, condition.ID, condition.ConditionDic);
            switch (condition.OperType)
            {
                case Enum_operType.Add:
                    //todo:暂无添加需求
                    break;
                case Enum_operType.Del:
                    Delete(condition, ti);
                    break;
                case Enum_operType.Seach:
                    if (!string.IsNullOrEmpty(condition.PIDKeyName)) //树
                    {
                        new TreeAssistant().SetTree(condition);
                        break;
                    }
                    var mp = new MoldPanel();
                    bool isSingleTable = true;//是否单表（适用于内容模型）
                    if (!string.IsNullOrEmpty(condition.HasModelXml)) isSingleTable = mp.EnableSingleTable;
                    if (isSingleTable) Search(condition, ti); //单表
                    else Search(condition, ti, mp);
                    break;
                case Enum_operType.Update:
                    Update(condition, ti);
                    break;
                default:
                    break;
            }
        }

        #region private method
        private void Update(IQueryCondition condition, TableInfo ti)
        {
            try
            {
                int i = 0;
                var field = new List<string>(condition.Fields);
                field.Remove(condition.PriMaryKeyName);
                i = !condition.IsMulti ? IDatabase.Update(ti.TableName, ti, field.ToArray()) : IDatabase.Update(ti.TableName, ti, field.ToArray(), condition.Condition);
                QueryCondition.SetMessage(condition, condition.TableName, false, "修改", string.Empty, i);
            }
            catch (Exception ex)
            {
                QueryCondition.SetMessage(condition, condition.TableName, true, "修改", ex.Message);
                We7.Framework.LogHelper.WriteLog(typeof(JsonResult), ex);
            }
        }

        private void Search(IQueryCondition condition, TableInfo ti)
        {
            Order[] o = string.IsNullOrEmpty(condition.Sort) ? new Order[] { new Order(condition.PriMaryKeyName, OrderMode.Asc) } : new Order[] { new Order(condition.Sort, (OrderMode)condition.Sord), new Order(condition.PriMaryKeyName, OrderMode.Asc) };
            try
            {

                if (condition.JoinInfo != null) { InstanceCriteria(condition); } //初始化查询条件（有表连接的情况下）。
                tablename = ti.TableName; //设置表名
                condition.total = IDatabase.Total(ti.TableName, condition.Condition);  //总记录数
                var aList = IDatabase.GetDtByCondition<TableInfo>(ti.TableName, condition.Condition, o, condition.Begin - 1, condition.Count, condition.Fields);

                if (condition.JoinInfo != null) aList[0].Table = IDatabase.Join(aList[0].Table, condition); //表连接

                if (!condition.JsonMessage.ContainsKey(Enum_operType.Seach.ToString()))
                {
                    condition.JsonMessage.Add(Enum_operType.Seach.ToString(), aList[0].ToJson().Replace("{0}", condition.total.ToString(CultureInfo.InvariantCulture)).Replace("{1}", "200").Replace("{2}", "success").Replace("{3}", condition.Page.ToString(CultureInfo.InvariantCulture)).Replace("{4}", condition.totalPage.ToString(CultureInfo.InvariantCulture)));  //查询结果特殊处理（回发消息）
                }
            }
            catch (Exception ex)
            {
                QueryCondition.SetMessage(condition, condition.TableName, true, "search", ex.Message);
                We7.Framework.LogHelper.WriteLog(typeof(JsonResult), ex);
            }
        }

        /// <summary>
        /// 木有启用单表存储(适用于旧版本) 处理逻辑
        /// </summary>
        [Obsolete]
        private void Search(IQueryCondition condition, TableInfo ti, MoldPanel mp)
        {
            #region 木有启用单表存储(适用于旧版本) 处理逻辑
            //xml解析字段
            PanelContext data = mp.GetPanelContext(condition.ModelName, "list");
            data.PageIndex = condition.Page;
            data.c = new Criteria(CriteriaType.Equals, "ModelName", condition.ModelName); //设置条件
            if (condition.Condition != null)
            {
                data.c.Criterias.Add(condition.Condition);
            }

            ListResult result = new QueryCommand().Do(data) as ListResult;
            if (result != null)
            {
                DataTable dt = result.DataTable.Clone();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string name = dt.Columns[i].ColumnName.Clone().ToString();
                    bool isExist = FiledExists(condition.Fields, name);
                    if (!isExist)
                    {
                        result.DataTable.Columns.Remove(name);
                    }
                }
                //todo:XML数据源过滤信息？
                ti.Table = result.DataTable;
                string json = ti.ToJson();
                condition.total = result.RecoredCount;
                condition.Page = result.PageIndex;
                condition.JsonMessage.Add(Enum_operType.Seach.ToString(), json.Replace("{0}", condition.total.ToString()).Replace("{1}", "200").Replace("{2}", "数据成功返回").Replace("{3}", condition.Page.ToString()).Replace("{4}", condition.totalPage.ToString()));  //查询结果特殊处理
            }
            #endregion
        }

        private void Delete(IQueryCondition condition, TableInfo ti)
        {
            try
            {
                int i = 1;
                if (!condition.IsMulti)
                {
                    IDatabase.Delete(ti.TableName, ti);
                }
                else
                {
                    i = IDatabase.Delete<TableInfo>(ti.TableName, condition.Condition);
                }
                QueryCondition.SetMessage(condition, condition.TableName, false, "删除", string.Empty, i);
            }
            catch (Exception ex)
            {
                QueryCondition.SetMessage(condition, condition.TableName, true, "删除", ex.Message);
                We7.Framework.LogHelper.WriteLog(typeof(JsonResult), ex);
            }
        }

        private void InstanceCriteria(IQueryCondition condition)
        {
            if (condition.JoinInfo != null)
            {
                foreach (var join in condition.JoinInfo.Values)
                {
                    Criteria joincriteria = new Criteria(CriteriaType.None);
                    getJoinCriteria(condition,join, condition.Condition, ref joincriteria);
                    if (joincriteria.Criterias!=null&&joincriteria.Criterias.Count!=0)
                    {
                        tablename = join.ToTableName;
                        List<TableInfo> aList = IDatabase.GetDtByCondition<TableInfo>(join.ToTableName, joincriteria, new string[] { condition.PriMaryKeyName });
                        Criteria c = new Criteria(CriteriaType.None);
                        if (aList[0].Table != null && aList[0].Table.Rows != null && aList[0].Table.Rows.Count > 0)
                        {
                            c.Mode = CriteriaMode.Or;
                            foreach (DataRow item in aList[0].Table.Rows)
                            {
                                c.AddOr(CriteriaType.Equals, join.MainField, item[0]);
                            }
                            condition.Condition.Criterias.Add(c);
                        }
                        else
                        {
                            condition.Condition.Add(CriteriaType.Equals, condition.PriMaryKeyName, "no");
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 递归拼条件 用于表连接
        /// </summary>
        private void getJoinCriteria(IQueryCondition condition, IJoin join, Criteria c, ref Criteria joincriteria)
        {
            if (c==null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(c.Name) && join.MainField==c.Name)
            {
                joincriteria.Add(c.Type, join.JoinInfo[c.Name].ToField, c.Value);
                if (condition.Condition.Criterias != null)  //移除
                {
                    condition.Condition.Criterias.RemoveAll(delegate(Criteria cri)
                    {
                        if (cri.Type == c.Type && (string.Compare(cri.Name, c.Name, true) == 0) && string.Compare(cri.Value.ToString(), c.Value.ToString(), true)==0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                }
                    
            }
            if (c.Criterias != null&&c.Criterias.Count!=0)
            {
                for (int i = 0; i < c.Criterias.Count; i++)
                {
                    getJoinCriteria(condition,join, c.Criterias[i], ref joincriteria);
                }
            }
        }

        private static bool FiledExists(IEnumerable<string> fs, string f)
        {
            bool flag = false;
            foreach (var filed in fs)
            {
                if (!f.Equals(filed)) continue;
                flag = true;
                break;
            }
            return flag;
        }
        #endregion
    }
}