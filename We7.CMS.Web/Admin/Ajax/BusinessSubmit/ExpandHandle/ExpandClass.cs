using System;
using System.Collections.Generic;
using We7.Model.Core.UI;
using We7.Model.Core;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using Thinkment.Data;
using We7.CMS.Common.Enum;
using We7.CMS.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    public class ExpandClass : DataBaseForThinkment, IJsonResult
    {
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static void SubmitAuditCommand(IQueryCondition condition)
        {
            MoldPanel mp = new MoldPanel();
            PanelContext data = GetPanelContext(condition, mp);
            We7.Model.UI.Command.SubmitAuditCommand sac = new Model.UI.Command.SubmitAuditCommand();
            try
            {
                sac.Do(data);
                QueryCondition.SetMessage(condition, condition.TableName, false, "操作", string.Empty, condition.ID.Split(',').Length);
            }
            catch (Exception ex)
            {
                QueryCondition.SetMessage(condition, condition.TableName, true, "操作", ex.Message);
                We7.Framework.LogHelper.WriteLog(typeof(ExpandClass), ex);
            }
        }

        /// <summary>
        /// 获取模型的各种数量
        /// </summary>
        /// <param name="condition"></param>
        public static void GetModelCount(IQueryCondition condition)
        {
            TableInfo ti = new TableInfo(condition.TableName);
            DataBaseForThinkment db = new DataBaseForThinkment(condition.TableName);
            try
            {
                int total = db.IDatabase.Total(ti.TableName, null);
                Criteria c = new Criteria(CriteriaType.Equals, "state", (int)ArticleStates.Started);
                int yetPublish = db.IDatabase.Total(ti.TableName, c);
                c = new Criteria(CriteriaType.Equals, "state", (int)ArticleStates.Stopped);
                int draft = db.IDatabase.Total(ti.TableName, c);
                c = new Criteria(CriteriaType.Equals, "state", (int)ArticleStates.Checking);
                int Checking = db.IDatabase.Total(ti.TableName, c);
                c = new Criteria(CriteriaType.Equals, "state", (int)ArticleStates.Overdued);
                int Overdued = db.IDatabase.Total(ti.TableName, c);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("yetPublish", yetPublish.ToString());
                dic.Add("draft", draft.ToString());
                dic.Add("Checking", Checking.ToString());
                dic.Add("Overdued", Overdued.ToString());
                dic.Add("total", total.ToString());
                condition.JsonMessage.Add(Enum_operType.Seach.ToString(), ti.ToJson(dic, false));
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(ExpandClass), ex);
            }
        }

        private static PanelContext GetPanelContext(IQueryCondition qc, MoldPanel mp, string type = "list")
        {
            PanelContext data = mp.GetPanelContext(qc.ModelName, type);  //设置为List类型(暂时写死)

            if (!string.IsNullOrEmpty(qc.ID))
            {
                List<DataKey> dataKeys = new List<DataKey>();
                foreach (var item in qc.ID.Split(','))
                {
                    OrderedDictionary dic = new OrderedDictionary();
                    dic.Add(qc.PriMaryKeyName, item);
                    dataKeys.Add(new DataKey(dic as IOrderedDictionary));
                }
                data.State = dataKeys;
            }

            return data;
        }
    }
}