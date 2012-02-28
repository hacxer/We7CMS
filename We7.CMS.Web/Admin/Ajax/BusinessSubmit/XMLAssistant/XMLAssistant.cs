using System;
using System.Collections.Generic;
using System.Web;
using We7.Model.Core.UI;
using Thinkment.Data;
using System.Data;
using We7.Model.UI.Data;
using We7.Model.Core;
using We7.CMS.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    /// <summary>
    /// 内容模型非单表存储时用到。
    /// 现已过时。以后会去掉这个东西
    /// </summary>
    [Obsolete]
    public sealed class XMLAssistant : DataBaseForThinkment
    {
        public static DataBaseForThinkment getDataBase(TableInfo ti)
        {
            return new DataBaseForThinkment(ti.TableName);
        }

        /// <summary>
        /// 更新模型xml
        /// </summary>
        /// <returns></returns>
        public static void UpdataModel(IQueryCondition condition)
        {
            string ModelXml = condition.HasModelXml;  //数据模型信息
            if (!string.IsNullOrEmpty(ModelXml) && condition.OperType == Enum_operType.Update)
            {
                List<string> list = new List<string>(condition.Fields);
                list.Add(ModelXml);
                if (!list.Contains(condition.PriMaryKeyName)) list.Add(condition.PriMaryKeyName); //加上主键

                MoldPanel mp = new MoldPanel();
                PanelContext data = mp.GetPanelContext(condition.ModelName, "list");  //设置为List类型(暂时写死)

                Criteria criteria = new Criteria(CriteriaType.None);
                criteria.Mode = CriteriaMode.Or;
                foreach (var item in condition.ID.Split(','))
                {
                    criteria.AddOr(CriteriaType.Equals, condition.PriMaryKeyName, item);
                }

                TableInfo tableinfo = new TableInfo(data.Model.Type.ToString());

                var tablelist = getDataBase(tableinfo).IDatabase.GetDtByCondition<TableInfo>(data.Model.Type.ToString(), criteria, list.ToArray()); //获取对应字段，的table信息
                DataTable dt = null != tablelist ? tablelist[0].Table : null;

                condition.OperType = Enum_operType.Update; //设为修改类型
                foreach (DataRow item in dt.Rows)
                {
                    if (item[ModelXml] != DBNull.Value && item != null)
                    {
                        List<FieldsDic> Fileds = new List<FieldsDic>(); // 最终字典

                        DataSet ds = BaseDataProvider.CreateDataSet(data.Model);  //穿件数据集
                        BaseDataProvider.ReadXml(ds, item[ModelXml].ToString());

                        var files = condition.ConditionDic; //获取到的字典

                        if (!files.Exists(delegate(FieldsDic f) { if (string.Compare(f.Key, ModelXml, true) == 0)return true; else { return false; } }))  //添加XML扩展字段
                        {
                            files.Add(new FieldsDic(ModelXml, string.Empty));
                        }
                        foreach (var file in files)
                        {
                            if (!file.Key.ToUpper().Equals(condition.PriMaryKeyName) && ds.Tables[data.Table.Name].Columns.Contains(file.Key))
                            {
                                ds.Tables[data.Table.Name].Rows[0][file.Key] = file.Value; //赋值
                                if (!Fileds.Exists(delegate(FieldsDic f) { if (string.Compare(f.Key, condition.HasModelXml, true) == 0)return true; else { return false; } }))
                                {
                                    files.Add(new FieldsDic(file.Key, file.Value));
                                }
                            }
                        }

                        Fileds.Find(delegate(FieldsDic f) { 
                            if (string.Compare(f.Key, ModelXml, true) == 0) 
                                {   
                                    f.Value= BaseDataProvider.GetXml(ds); //拿到XML 赋值
                                    return true;
                                } 
                            else { return false; } });

                        TableInfo ti = new TableInfo(data.Model.Type.ToString(), item[condition.PriMaryKeyName].ToString(), Fileds);
                        list.Remove(condition.PriMaryKeyName);
                        TryUpData(condition, list, ti, data.Model.Type.ToString(), true);
                    }

                }
                QueryCondition.SetMessage(condition, "模型字段:" + condition.HasModelXml, false, "修改", string.Empty, dt.Rows.Count);

            }

        }

        /// <summary>
        /// 删除模型XML
        /// </summary>
        /// <param name="condition"></param>
        public static void DeleteModel(IQueryCondition condition)
        {
            if (!string.IsNullOrEmpty(condition.HasModelXml) && condition.OperType == Enum_operType.Del)
            {
                TableInfo ti = new TableInfo(condition.TableName);

                try
                {
                    int i = getDataBase(ti).IDatabase.Delete<TableInfo>(ti.TableName,condition.Condition);
                    QueryCondition.SetMessage(condition, condition.TableName, false, "删除", string.Empty, i);
                }
                catch (Exception ex)
                {
                    We7.Framework.LogHelper.WriteLog(typeof(XMLAssistant), ex);
                    QueryCondition.SetMessage(condition, condition.TableName, true, "删除", ex.Message);
                }

            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="list">字段名</param>
        /// <param name="ti">实体</param>
        /// <param name="key">表名</param>
        /// <param name="isTransfer">是否</param>
        private static void TryUpData(IQueryCondition condition, List<string> list, TableInfo ti, string key, bool isTransfer = false)
        {
            IConnection conn = getDataBase(ti).IDatabase.CreateConnetion(key, true);
            try
            {
                int i = 1;
                if (condition.IsMulti)
                {
                    i = getDataBase(ti).IDatabase.Update(ti.TableName,ti, list.ToArray(), condition.Condition);
                }
                else
                {
                    i = getDataBase(ti).IDatabase.Update(ti.TableName, ti, list.ToArray(), conn);
                }

                conn.Commit();
            }
            catch (Exception ex)
            {
                conn.Rollback();
                We7.Framework.LogHelper.WriteLog(typeof(XMLAssistant), ex);
                QueryCondition.SetMessage(condition, "模型字段:" + condition.HasModelXml, true, "修改", ex.Message);
            }
        }

    }
}