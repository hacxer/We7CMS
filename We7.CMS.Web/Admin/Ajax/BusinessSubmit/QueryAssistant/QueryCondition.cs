using System;
using System.Collections.Generic;
using Thinkment.Data;
using We7.CMS.Data;

namespace We7.CMS.Web.Admin.Ajax.BusinessSubmit
{
    /// <summary>
    /// Ajax 请求实体
    /// </summary>
    [Serializable]
    public class QueryCondition : IQueryCondition, ICloneable
    {
        /// <summary>
        /// 输出Json
        /// </summary>
        public event ResponseDelegate OnToJsonEvent;
        /// <summary>
        /// 除数json之后
        /// </summary>
        public event ReaderXmlDelegate UnToJsonEvent; //回发json之后
        /// <summary>
        /// 扩展事件
        /// </summary>
        public event ExpandDelegate ExpandEvent;  //扩展事件

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="querystring"></param>
        public QueryCondition(System.Collections.Specialized.NameValueCollection querystring)
        {
            int i;
            Page = int.TryParse(querystring["_page"], out i) ? i : 1;

            OperType = int.TryParse(querystring["_oper"], out i) ? (Enum_operType) i : Enum_operType.Seach;

            Rows = int.TryParse(querystring["_rows"], out i) ? i : 10;

            Sort = querystring["_sort"];

            Sord = int.TryParse(querystring["_sord"], out i) ? i : 0;
            bool flag;
            if (bool.TryParse(querystring["_search"], out flag))
            {
                Search = flag;
            }
            T = querystring["_t"];

            C = querystring["_c"];

            TableName = querystring["_tb"];

            string fields = querystring["_f"];
            if (!string.IsNullOrEmpty(fields) && !fields.ToUpper().Contains(",ID,") && !fields.ToUpper().StartsWith("ID,") && !fields.ToUpper().EndsWith(",ID"))
            {
                fields += ",ID";
            }
            Fields = string.IsNullOrEmpty(fields) ? null : fields.Split(',');

            ID = querystring["_id"];

            _hasModelXml = querystring["_xml"];
            _modelName = querystring["_model"];

            _expandKey = querystring["expandKey"];
            if (int.TryParse(querystring["_isSiteGroup"], out i))
            {
                _isSiteGroup = (i == 1);
            }

            InstanceJoin(querystring["_join"]);

            _pIdKeyName = querystring["_pidname"];
        }

        /// <summary>
        /// 输出Json
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string ToJson(IQueryCondition condition)
        {
            //TODO:事务处理？OnToJsonEvent(condition,Iconnection,IsTransfer) && 验证过滤
            string result;
            if (OnToJsonEvent != null)
            {
                OnToJsonEvent(condition);
            }
            if (ExpandEvent != null)
            {
                ExpandEvent(this);
            }
            if (UnToJsonEvent != null)
            {
                UnToJsonEvent(this);
            }

            if (_jsonMessage.ContainsKey(Enum_operType.Seach.ToString()))
            {
                result = _jsonMessage[Enum_operType.Seach.ToString()];
            }
            else
            {
                TableInfo ti = new TableInfo();
                result = ti.ToJson(_jsonMessage, false);
            }

            return result;
        }

        #region 实体信息

        #region fields
        private IDictionary<string, string> _jsonMessage = new Dictionary<string, string>();
        private string _hasModelXml = string.Empty;
        /// <summary>
        /// xml数据模型字段名
        /// </summary>
        private string _modelName;
        private Enum_operType _oper;
        private bool _isSiteGroup;
        private string _id;
        private string _sort;
        private int _sord;
        private bool _search;
        private string _t;
        private string _c;
        private string _tb;
        private string[] _f;
        private List<FieldsDic> _conditionDic;
        private string _priMaryKeyName = "ID";
        private object _expandKey;
        private Criteria _condition;
        #endregion

        #region property
        /// <summary>
        /// 回发信息
        /// </summary>
        public IDictionary<string, string> JsonMessage
        {
            get
            {
                return _jsonMessage;
            }
            set
            {
                _jsonMessage = value;
            }
        }

        /// <summary>
        /// xml数据模型信息
        /// </summary>
        public string HasModelXml
        {
            get
            {
                return _hasModelXml;
            }
            set
            {
                _hasModelXml = value;
            }
        }

        /// <summary>
        /// 内容模型，模型名
        /// </summary>
        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public Enum_operType OperType
        {
            get { return _oper; }
            set { _oper = value; }
        }

        /// <summary>
        /// 是否多行
        /// </summary>
        public bool IsMulti
        {
            get
            {
                return !string.IsNullOrEmpty(_id) && _id.Split(',').Length > 0;
            }
            set { IsMulti = value; }
        }

        /// <summary>
        /// 站群
        /// </summary>
        public bool IsSiteGroup
        {
            get
            {
                return _isSiteGroup;
            }
            set
            {
                _isSiteGroup = value;
            }
        }

        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get
            {
                return _id;
            }
            set { _id = value; }
        }

        /// <summary>
        /// 用于排序的列名索引（一个名称）
        /// </summary>
        public string Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        /// <summary>
        /// 排序的顺序（值为 asc(0) 或 desc(1)）
        /// 默认为asc
        /// </summary>
        public int Sord
        {
            get { return _sord; }
            set { _sord = value; }
        }

        /// <summary>
        /// 是否搜索（如果不搜索，其值为 false，）
        /// </summary>
        public bool Search
        {
            get { return _search; }
            set { _search = value; }
        }

        /// <summary>
        /// 一个用于禁止缓存的时间戳
        /// </summary>
        public string T
        {
            get { return _t; }
            set { _t = value; }
        }

        /// <summary>
        /// 条件
        /// </summary>
        public string C
        {
            get { return _c; }
            set { _c = value; }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return _tb; }
            set { _tb = value; }
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string[] Fields
        {
            get
            {
                if (Enum_operType.Update == OperType)
                {
                    var fields = new List<string>();
                    _conditionDic.ForEach(delegate(FieldsDic f) { fields.Add(f.Key); });
                    _f = fields.ToArray();
                }
                return _f;
            }
            set { _f = value; }
        }
       
        /// <summary>
        /// 条件{多行处理时条件(字段名为ID)}
        /// </summary>
        public Criteria Condition
        {
            get
            {

                if (null == _condition && !string.IsNullOrEmpty(C) && !IsMulti)
                {
                    _condition = MakeCondition(C);
                }
                else if (null == _condition && !string.IsNullOrEmpty(_id) && IsMulti)
                {
                    _condition = new Criteria(CriteriaType.None) {Mode = CriteriaMode.Or};
                    foreach (var item in ID.Split(','))
                    {
                        _condition.AddOr(CriteriaType.Equals, _priMaryKeyName, item);
                    }
                }
                return _condition;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                Condition = value;
            }
        }


        /// <summary>
        /// 主键字段的名字
        /// 默认为"ID"
        /// </summary>
        public string PriMaryKeyName
        {
            get { return _priMaryKeyName; }
            set { _priMaryKeyName = value; }
        }

        /// <summary>
        /// 条件字典
        /// </summary>
        public List<FieldsDic> ConditionDic
        {
            get
            {
                if (_conditionDic == null)
                {
                    _conditionDic = new List<FieldsDic>();
                    if ((OperType == Enum_operType.Update || OperType == Enum_operType.Seach) && !string.IsNullOrEmpty(_c))
                    {
                        string[] cs = _c.Split('|');//条件数组
                        for (int i = 0; i < cs.Length; i++)
                        {
                            string[] value = cs[i].Split('@');
                            int type;
                            if (int.TryParse(value[1], out type))
                            {
                                if (value.Length == (int)EnumIsKeyValue.Yes)
                                {
                                    _conditionDic.Add(new FieldsDic(value[0], value[2]));
                                }
                            }
                        }
                    }
                }
                return _conditionDic;
            }
            set { _conditionDic = value; }
        }

        /// <summary>
        /// 扩展参数
        /// </summary>
        public object ExpandKey
        {
            get
            {
                return _expandKey;
            }
            set
            {
                _expandKey = value;
            }
        }
        #endregion

        #region 分页
        private int _total;
        /// <summary>
        /// 总数据数
        /// </summary>
        public int total
        {
            get { return _total; }
            set { _total = value; }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int totalPage
        {
            get
            {
                return ((total % Rows) == 0) ? total / Rows : (total / Rows) + 1;
            }
        }

        private int _page;
        /// <summary>
        /// 页码
        /// </summary>
        public int Page
        {
            get { return _page; }
            set {
                _page = value >= 1 ? value : 1;
            }
        }
        private int _rows;
        /// <summary>
        /// 行数
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            set {
                _rows = value >= 1 ? value : 1;
            }
        }
        /// <summary>
        /// 本页显示：从第几条开始
        /// </summary>
        public int Begin
        {
            get { return Rows * (Page - 1) + 1; }
        }

        /// <summary>
        /// 本页显示：到第几条结束
        /// </summary>
        public int End
        {
            get
            {
                int i = Rows * Page;
                return i < total ? i : total;
            }
        }
        /// <summary>
        /// 本页显示记录数
        /// </summary>
        public int Count
        {
            get { return End - Begin + 1; }
        }
        #endregion

        #endregion

        #region 表链接
        
        private string _toTableName;
        private string _toField;
        private string _mainField;
        private IDictionary<string, IJoin> _joinInfo;

        public string ToTableName
        {
            get
            {
                return _toTableName;
            }
            set
            {
                _toTableName = value;
            }
        }
       
        public string ToField
        {
            get
            {
                return _toField;
            }
            set
            {
                _toField = value;
            }
        }
       
        public string MainField
        {
            get
            {
                return _mainField;
            }
            set
            {
                _mainField = value;
            }
        }
        
        public IDictionary<string, IJoin> JoinInfo
        {
            get
            {
                return _joinInfo;
            }
            set
            {
                _joinInfo = value;
            }
        }
        #endregion

        #region private Method
        private Criteria MakeCondition(string cS)
        {
            string c = cS;
            string[] cs = c.Split('|');//条件数组
            var criteria = new Criteria(CriteriaType.None);
            for (int i = 0; i < cs.Length; i++)
            {
                string[] value = cs[i].Split('@');

                int type;
                if (int.TryParse(value[1], out type))
                {
                    if (value.Length == (int)EnumIsKeyValue.Yes && !string.IsNullOrEmpty(value[2])) //且值不为空
                    {
                        criteria.Add((CriteriaType)type, value[0], value[2]);
                    }
                    else if ((value.Length == (int)EnumIsKeyValue.No))
                    {
                        criteria.Add((CriteriaType)type, value[0], null);
                    }
                }
                else //如果表达式不是数字。则直接false
                {
                    criteria.Add(CriteriaType.Equals, "1", "2");
                }
            }
            return criteria;
        }
        /// <summary>
        /// 实例化表连接操作
        /// </summary>
        /// <param name="querystring"></param>
        private void InstanceJoin(string querystring)
        {
            if (string.IsNullOrEmpty(querystring)) return;

            string[] joins = querystring.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            if (joins.Length == 0) return;
            _joinInfo = new Dictionary<string, IJoin>();
            foreach (var item in joins) //一组join
            {
                string[] join = item.Split('|');
                if (@join.Length != 4)
                {
                    _joinInfo = null;
                    break;
                }
                _mainField = join[0];
                _toTableName = join[1];
                _toField = join[2];
                _priMaryKeyName = join[3];
                if (!_joinInfo.ContainsKey(join[0]))
                {
                    _joinInfo.Add(join[0], MemberwiseClone() as QueryCondition);
                }
            }
        }
        #endregion

        /// <summary>
        /// 设置回发信息值
        /// </summary>
        /// <param name="condition">对象</param>
        /// <param name="key">显示名（表名）</param>
        /// <param name="isError">是否是错误信息</param>
        /// <param name="msgKey">“删除”或者“修改”这样的关键字</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="i">受影响的行数</param>
        public static void SetMessage(IQueryCondition condition, string key, bool isError, string msgKey, string errorMsg = "", int i = 0)
        {
            /*
             * ps:这个方法如果有时间，且正好看到的您不妨改改吧。太恶心了
             */
            if (!isError)
            {
                if (condition.JsonMessage.ContainsKey("code"))
                {
                    condition.JsonMessage["code"] = "200";
                }
                else
                {
                    condition.JsonMessage.Add("code", "200");
                }
                if (condition.JsonMessage.ContainsKey("message"))
                {
                    condition.JsonMessage["message"] += "|" + key + ":" + i + "条数据以" + msgKey;
                }
                else
                {
                    condition.JsonMessage.Add("message", key + ":" + i + "条数据以" + msgKey);
                }
            }
            else
            {
                if (condition.JsonMessage.ContainsKey("code"))
                {
                    condition.JsonMessage["code"] = "300";
                }
                else
                {
                    condition.JsonMessage.Add("code", "300");
                }
                if (condition.JsonMessage.ContainsKey("message"))
                {
                    condition.JsonMessage["message"] += "|" + key + ":" + msgKey + "失败：" + errorMsg;
                }
                else
                {
                    condition.JsonMessage.Add("message", key + ":" + msgKey + "失败：" + errorMsg);
                }
            }

        }

        /// <summary>
        /// 克隆此实例（浅拷贝）
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #region 树结构
        private string _pIdKeyName;
        private string _treeTitle;
        private bool _hasNode;

        public string PIDKeyName
        {
            get
            {
                return _pIdKeyName;
            }
            set
            {
                _pIdKeyName = value;
            }
        }
       
        public string TreeTitle
        {
            get
            {
                return _treeTitle;
            }
            set
            {
                _treeTitle = value;
            }
        }
       
        public bool HasNode
        {
            get
            {
                return _hasNode;
            }
            set
            {
                _hasNode = value;
            }
        }

        //todo:有时间再来实现
        public IDictionary<string, ITree> TreeInfo
        {
            get;
            set;
        }
        public List<T> TreeList<T>() where T : class
        {
            return null;
        }
        #endregion
    }

    #region 操作枚举
    /// <summary>
    /// 是否是KeyValue
    /// </summary>
    public enum EnumIsKeyValue
    {
        /// <summary>
        /// 是
        /// </summary>
        Yes = 3,
        /// <summary>
        /// 否
        /// </summary>
        No = 2
    }

    #endregion
}