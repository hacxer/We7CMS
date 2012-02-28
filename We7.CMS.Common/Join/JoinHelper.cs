using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Data;

namespace We7.CMS.Common
{
    /*begin
        * 说明：此段代码的大致意思是 去反馈模型的配置文件里拿表关联信息
        *       然后反射去Util里走表关联逻辑(涉及到站群)。您之所以看到此段代码出现，是因为
        *       确实没地方控制了，所以才采用了如此极端的做法。它还缺乏优化。
        *       如果您觉得这段代码实在是难以维护，且写法实在丑陋。 那您不妨修改它吧。
        *       如果您没什么把握修改它，而它能正常运行。我劝您别在此段代码上浪费时间
        * time:2011/12/28
        * author:丁乐
        */
   public class JoinHelper
    {
        public JoinHelper(string modelconfig)
        {
            this.ModelConfig = modelconfig;
        }
        private string ModelConfig;

        private Dictionary<string, string> adviceConfigDic;

        /// <summary>
        /// 反馈表连接配置信息
        /// 具体说明请看配置文件
        /// </summary>
        public Dictionary<string, string> AdviceConfigDic
        {
            /*
             * 暂未加缓存。 
             */
            get
            {
                if (adviceConfigDic == null)
                {
                    string configpath = AppDomain.CurrentDomain.BaseDirectory + "//Config//AdviceJoinConfig.config";
                    if (File.Exists(configpath))
                    {
                        adviceConfigDic = new Dictionary<string, string>();
                        XmlDocument xml = new XmlDocument();
                        xml.Load(configpath);
                        XmlNode root = xml.SelectSingleNode("//appSettings");
                        foreach (XmlNode n in root.ChildNodes)
                        {
                            if (n.NodeType != XmlNodeType.Comment && n.Name.ToLower() == "item")
                            {
                                if (!adviceConfigDic.ContainsKey(n.Attributes["key"].Value))
                                {
                                    adviceConfigDic.Add(n.Attributes["key"].Value, n.Attributes["value"].Value);
                                }
                            }
                        }
                    }

                }
                return adviceConfigDic;
            }
        }

        private Assembly joinAssembly;
        private Assembly JoinAssembly
        {
            get { return joinAssembly ?? AppDomain.CurrentDomain.Load(AdviceConfigDic["JoinAssembly"]); }
        }
        private object joinAssistan;
        public object JoinAssistan
        {
            get { return joinAssistan == null && JoinAssembly != null ? JoinAssembly.CreateInstance(AdviceConfigDic["DataBaseAssistant"]) : joinAssistan; }
        }
        private XmlDocument configDocument;
        public XmlDocument ConfigDocument
        {
            get
            {
                if (configDocument == null)
                {
                    configDocument = new XmlDocument();
                    configDocument.LoadXml(ModelConfig);
                }
                return configDocument;
            }
        }
        public string JoinField(string fieldName, string value)
        {
            Boolean flag;
            Boolean.TryParse(AdviceConfigDic["IsOpen"], out flag); //开关
            if (!flag) { return value; }
            if (!string.IsNullOrEmpty(fieldName))
            {
                XmlDocument document = ConfigDocument;
                Dictionary<string, object> PropertyDic;
                object DataAssiant = JoinAssistan;
                Type DataAssiantType = DataAssiant.GetType();
                object joinex = JoinAssembly.CreateInstance(AdviceConfigDic["JoinEx"]);
                Type jointype = joinex.GetType();
                XmlNode ControlNode = document.SelectSingleNode("/ModelInfo/dataSet/dataTable//dataColumn[@mapping='" + fieldName + "']");
                if (ControlNode != null)
                {
                    string dataColumnName = ControlNode.Attributes["name"].Value;

                    XmlNode control = document.SelectSingleNode("/ModelInfo/layout//panel[@name='edit']/edit//group[@index='0']//control[@name='" + dataColumnName + "']");
                    if (control != null && null != control.SelectSingleNode("child::param[@name='model']"))
                    {
                        PropertyDic = new Dictionary<string, object>();
                        PropertyDic.Add("MainField", fieldName);
                        PropertyDic.Add("ToTableName", control.SelectSingleNode("child::param[@name='model']").Attributes["value"].Value);
                        PropertyDic.Add("ToField", control.SelectSingleNode("child::param[@name='textfield']").Attributes["value"].Value);
                        PropertyDic.Add("PriMaryKeyName", control.SelectSingleNode("child::param[@name='valuefield']").Attributes["value"].Value);
                        foreach (var item in PropertyDic)
                        {
                            object c = Convert.ChangeType(item.Value, item.Value.GetType()); //动态转换类型
                            jointype.GetProperty(item.Key).SetValue(joinex, c, null);
                        }
                        jointype.GetMethod(AdviceConfigDic["AddJoinMethod"]).Invoke(joinex, new object[] { jointype.GetMethod("Clone").Invoke(joinex, null) });
                        if (null != joinex)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn(fieldName));
                            dt.Rows.Add(new Object[] { value });
                            dt = DataAssiantType.GetMethod("Join").Invoke(DataAssiant, new object[] { dt, joinex, null }) as DataTable;
                            value = dt.Rows[0][0].ToString();
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 表连接方法
        /// </summary>
        /// <param name="ds">ds</param>
        /// <param name="fieldnames">字段</param>
        /// <returns></returns>
        public DataSet JoinField(DataSet ds)
        {
            Boolean flag;
            Boolean.TryParse(AdviceConfigDic["IsOpen"], out flag);
            if (!flag) { return ds; }
            #region 字段
            string[] fieldNames;
            List<string> list = new List<string>();
            foreach (DataColumn item in ds.Tables[0].Columns)
            {
                list.Add(item.ColumnName);
            }
            fieldNames = list.Count == 0 ? null : list.ToArray();

            #endregion
            if (fieldNames == null || fieldNames.Length == 0)
            {
                return ds;
            }
            else
            {
                Dictionary<string, string> dic;
                Dictionary<string, object> PropertyDic;
                XmlDocument document = new XmlDocument();
                document = ConfigDocument;

                Assembly DataAss = JoinAssembly;
                object joinex = DataAss.CreateInstance(AdviceConfigDic["JoinEx"]);
                Type jointype = joinex.GetType();
                object DataAssiant = JoinAssistan;
                Type DataAssiantType = DataAssiant.GetType();


                foreach (var fieldName in fieldNames)
                {
                    dic = new Dictionary<string, string>();

                    XmlNode ControlNode = document.SelectSingleNode("/ModelInfo/layout//panel[@name='edit']/edit//group[@index='0']//control[@name='" + fieldName + "']"); //此段意思大概就是选取控件配置信息的集合。
                    if (ControlNode != null && null != ControlNode.SelectSingleNode("child::param[@name='model']"))
                    {
                        PropertyDic = new Dictionary<string, object>();
                        PropertyDic.Add("MainField", fieldName);

                        if (null != ControlNode.SelectSingleNode("child::param[@name='model']") && null != ControlNode.SelectSingleNode("child::param[@name='model']").Attributes["value"])
                        {
                            PropertyDic.Add("ToTableName", ControlNode.SelectSingleNode("child::param[@name='model']").Attributes["value"].Value);
                        }
                        if (null != ControlNode.SelectSingleNode("child::param[@name='textfield']") && null != ControlNode.SelectSingleNode("child::param[@name='textfield']").Attributes["value"])
                        {
                            PropertyDic.Add("ToField", ControlNode.SelectSingleNode("child::param[@name='textfield']").Attributes["value"].Value);
                        }
                        if (null != ControlNode.SelectSingleNode("child::param[@name='valuefield']") && null != ControlNode.SelectSingleNode("child::param[@name='valuefield']").Attributes["value"])
                        {
                            PropertyDic.Add("PriMaryKeyName", ControlNode.SelectSingleNode("child::param[@name='valuefield']").Attributes["value"].Value);
                        }

                        foreach (var item in PropertyDic)
                        {
                            object c = Convert.ChangeType(item.Value, item.Value.GetType()); //动态转换类型
                            jointype.GetProperty(item.Key).SetValue(joinex, c, null);
                        }
                        jointype.GetMethod(AdviceConfigDic["AddJoinMethod"]).Invoke(joinex, new object[] { jointype.GetMethod("Clone").Invoke(joinex, null) });
                    }
                }

                if (null != joinex)
                {
                    DataTable dt = DataAssiantType.GetMethod("Join").Invoke(DataAssiant, new object[] { ds.Tables[0], joinex, null }) as DataTable;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ds.Tables.Clear();
                        ds.Tables.Add(dt);
                    }
                }
                return ds;
            }
        }
    }
}
