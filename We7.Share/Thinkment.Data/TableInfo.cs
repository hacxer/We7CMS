/*
 *Author：丁乐 
 * 功能：拿到表信息结构
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Thinkment.Data
{
    /// <summary>
    /// 响应实体
    /// 功能：拿到表信息结构
    /// </summary>
    [Serializable]
    public class TableInfo : IParametExtension<TableInfo>
    {
        #region 构造函数
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="id">处理删除或者修改的时候用</param>
        /// <param name="fields">字段键值对字典（功效同上）</param>
        public TableInfo(string tableName, string id = null, List<FieldsDic> fields=null)//Dictionary<string, string> fields = null
        {
            this.tableName = tableName;
            if (!string.IsNullOrEmpty(id)) this.id = id;
            if (null != fields && fields.Count > 0) this.fieldsDic = fields;   //this.fields = fields;
        }
        public TableInfo()
        {
        }

        /// <summary>
        /// 实例化此类
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pro">字段字典</param>
        public TableInfo(DataTable dt, Dictionary<string, Property> pro)
        {
            this._table = dt;
            this.FieldsInfo = pro;
        }
        #endregion

        #region 表信息
        #region fields
        private string primaryKeyName = "ID";
        private int _records;
        private DataTable _table;
        /// <summary>
        /// 表名
        /// </summary>
        private string tableName = string.Empty;


        /// <summary>
        /// ID
        /// </summary>
        private string id = string.Empty;


        //private Dictionary<string, string> fields;
        ///// <summary>
        ///// 字段Key Value
        ///// </summary>
        //public Dictionary<string, string> Fields
        //{
        //    get { return fields; }
        //    set { fields = value; }
        //}
        /// <summary>
        /// 字段描述
        /// </summary>
        private Dictionary<string, Property> FieldsInfo;

        private Dictionary<string, string> heard;
        #endregion

        #region property

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// 主键名(默认为ID)
        /// </summary>
        public string PrimaryKeyName
        {
            get { return primaryKeyName; }
            set { primaryKeyName = value; }
        }

        /// <summary>
        /// 当前有多少条记录
        /// </summary>
        public int records
        {
            get
            {
                if (this.Table != null && this.Table.Rows != null && this.Table.Rows.Count != 0)
                {
                    _records = this.Table.Rows.Count;
                }
                else
                {
                    _records = 0;
                }
                return _records;
            }
        }

        /// <summary>
        /// 当前Table
        /// </summary>
        public DataTable Table
        {
            get { return _table; }
            set { _table = value; }
        }

        /// <summary>
        /// 字段属性Key
        /// </summary>
        private Dictionary<string, string> Heard
        {
            get
            {
                if (heard == null)
                {
                    heard = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    if (FieldsInfo != null)
                    {
                        Dictionary<string, Property> _fields = FieldsInfo;
                        foreach (var item in _fields)
                        {
                            heard.Add(item.Value.Field, item.Value.Description);
                        }
                    }
                }
                return heard;
            }
        }
        #endregion

        #endregion

        #region method
        /// <summary>
        /// 获取Json
        /// {0}总记录数
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return this.Convert2Json(this.Table);
        }

        /// <summary>
        /// 获取Json
        /// 说明：[{key:xx,value:xx},{..}]
        /// 暂时支持类型：DataRow、DataSet、DataTable、IEnumerable、Hashtable
        /// <param name="o">数据类型</param>
        /// <param name="isKeyValue">是否键值对</param>
        /// </summary>
        /// <returns></returns>
        public string ToJson(object o, bool isKeyValue = true)
        {
            if (isKeyValue)
            {
                return this.Convert2Json(o);
            }
            else
            {
                string json = this.Convert2Json(o);
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                MatchCollection mc = Regex.Matches(json, "{\"[\\s|\\S]*?\":\"(?<Key>[\\s|\\S]*?)\",\"[\\s|\\S]*?\":\"(?<Value>[\\s|\\S]*?)\"}");
                for (int i = 0; i < mc.Count; i++)
                {
                    sb.Append("\"" + mc[i].Groups["Key"] + "\"").Append(":").Append("\"" + mc[i].Groups["Value"] + "\"");
                    if (i != (mc.Count - 1))
                    {
                        sb.Append(",");
                    }
                }
                return sb.Append("}").ToString();
            }
        }

        #region dataset,datatable,object等对象转Json
        private void WriteDataRow(StringBuilder sb, DataRow row)
        {
            sb.Append("{");
            foreach (DataColumn column in row.Table.Columns)
            {
                sb.AppendFormat("\"{0}\":", column.ColumnName);
                WriteValue(sb, row[column]);
                sb.Append(",");
            }
            // Remove the trailing comma.
            if (row.Table.Columns.Count > 0)
            {
                --sb.Length;
            }
            sb.Append("}");
        }

        private void WriteDataSet(StringBuilder sb, DataSet ds)
        {
            sb.Append("{\"Tables\":{");
            foreach (DataTable table in ds.Tables)
            {
                sb.AppendFormat("\"{0}\":", table.TableName);
                WriteDataTable(sb, table);
                sb.Append(",");
            }
            // Remove the trailing comma.
            if (ds.Tables.Count > 0)
            {
                --sb.Length;
            }
            sb.Append("}}");
        }

        private void WriteDataTable(StringBuilder sb, DataTable table)
        {
            sb.Append("{\"rows\":[");
            foreach (DataRow row in table.Rows)
            {
                WriteDataRow(sb, row);
                sb.Append(",");
            }
            // Remove the trailing comma.
            if (table.Rows.Count > 0)
            {
                --sb.Length;
            }

            sb.Append("],").Append("\"cols\":["); //字段
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string columnname = table.Columns[i].ColumnName;
                string key = (table.PrimaryKey != null && table.PrimaryKey.Length > 0) ? table.PrimaryKey[0].ColumnName : "ID";
                bool isKey = key.Equals(table.Columns[i].ColumnName);

                sb.Append("{\"name\":\"" + table.Columns[i].ColumnName + "\",");
                sb.Append("\"header\":\"" + (Heard.ContainsKey(columnname) ? Heard[columnname] : " ") + "\",");
                sb.Append("\"editable\":" + false.ToString().ToLower() + ",");
                sb.Append("\"sortable\":" + true.ToString().ToLower() + ",");
                sb.Append("\"hidden\":" + isKey.ToString().ToLower() + ",");
                sb.Append("\"sorttype\":\"" + "date" + "\",");
                sb.Append("\"identify\":" + isKey.ToString().ToLower() + ",");
                sb.Append("\"sortOnLoad\":" + false.ToString().ToLower() + "}");
                if (i != table.Columns.Count - 1)
                {
                    sb.Append(",");
                }
                else
                {
                    sb.Append("]");
                }
            }
            sb.Append(",").Append("\"Records\":\"" + records + "\"");  //当前条数
            sb.Append(",").Append("\"totalRecords\":\"{0}\"");  //总记录数
            sb.Append(",").Append("\"code\":\"{1}\"");  //状态码
            sb.Append(",").Append("\"message\":\"{2}\"");  //信息
            sb.Append(",").Append("\"currPage\":\"{3}\"");  //页码
            sb.Append(",").Append("\"totalPages\":\"{4}\"");  //总页数
            sb.Append("}");

        }

        private void WriteEnumerable(StringBuilder sb, IEnumerable e)
        {
            bool hasItems = false;
            sb.Append("[");
            foreach (object val in e)
            {
                WriteValue(sb, val);
                sb.Append(",");
                hasItems = true;
            }
            // Remove the trailing comma.
            if (hasItems)
            {
                --sb.Length;
            }
            sb.Append("]");
        }

        private void WriteHashtable(StringBuilder sb, Hashtable e)
        {
            bool hasItems = false;
            sb.Append("{");
            foreach (string key in e.Keys)
            {
                sb.AppendFormat("\"{0}\":", key.ToLower());
                WriteValue(sb, e[key]);
                sb.Append(",");
                hasItems = true;
            }
            // Remove the trailing comma.
            if (hasItems)
            {
                --sb.Length;
            }
            sb.Append("}");
        }

        private void WriteObject(StringBuilder sb, object o)
        {
            MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
            sb.Append("{");
            bool hasMembers = false;
            foreach (MemberInfo member in members)
            {
                bool hasValue = false;
                object val = null;
                if ((member.MemberType & MemberTypes.Field) == MemberTypes.Field)
                {
                    FieldInfo field = (FieldInfo)member;
                    val = field.GetValue(o);
                    hasValue = true;
                }
                else if ((member.MemberType & MemberTypes.Property) == MemberTypes.Property)
                {
                    PropertyInfo property = (PropertyInfo)member;
                    if (property.CanRead && property.GetIndexParameters().Length == 0)
                    {
                        val = property.GetValue(o, null);
                        hasValue = true;
                    }
                }
                if (hasValue)
                {
                    sb.Append("\"");
                    sb.Append(member.Name);
                    sb.Append("\":");
                    WriteValue(sb, val);
                    sb.Append(",");
                    hasMembers = true;
                }
            }
            if (hasMembers)
            {
                --sb.Length;
            }
            sb.Append("}");
        }

        private void WriteString(StringBuilder sb, string s)
        {
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");
        }
        private void WriteValue(StringBuilder sb, object val)
        {
            if (val == null || val == System.DBNull.Value)
            {
                sb.Append("null");
            }
            else if (val is string || val is Guid)
            {
                WriteString(sb, val.ToString());
            }
            else if (val is bool)
            {
                sb.Append(val.ToString().ToLower());
            }
            else if (val is double ||
                val is float ||
                val is long ||
                val is int ||
                val is short ||
                val is byte ||
                val is decimal)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "{0}", val);
            }
            else if (val.GetType().IsEnum)
            {
                sb.Append((int)val);
            }
            else if (val is DateTime)
            {
                //sb.Append("new Date(\"");
                //sb.Append(((DateTime)val).ToString("MMMM, d yyyy HH:mm:ss", new CultureInfo("en-US", false).DateTimeFormat));
                //sb.Append("\")");
                DateTime time = Convert.ToDateTime(val);
                sb.Append("\"").Append(time.ToString("yyyy-MM-dd HH:mm")).Append("\"");
            }
            else if (val is DataSet)
            {
                WriteDataSet(sb, val as DataSet);
            }
            else if (val is DataTable)
            {
                WriteDataTable(sb, val as DataTable);
            }
            else if (val is DataRow)
            {
                WriteDataRow(sb, val as DataRow);
            }
            else if (val is Hashtable)
            {
                WriteHashtable(sb, val as Hashtable);
            }
            else if (val is IEnumerable)
            {
                WriteEnumerable(sb, val as IEnumerable);
            }
            else
            {
                WriteObject(sb, val);
            }
        }
        /// <summary>
        /// 转换Json
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private string Convert2Json(object o)
        {
            StringBuilder sb = new StringBuilder();
            WriteValue(sb, o);
            return sb.ToString();
        }
        #endregion
        #endregion

        public List<FieldsDic> fieldsDic
        {
            get;
            set;
        }
        /// <summary>
        /// 获取键值对字段值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFieldValue(string key)
        {
            if (fieldsDic != null)
            {
                FieldsDic field = fieldsDic.Find(delegate(FieldsDic f) { if (string.Compare(key,f.Key,true)==0) { return true; } else { return false; } });
                return null == field ? string.Empty : field.Value;
            }
            else
            {
                return null;
            }
        }
        public void AddFieldsDic(string key, string value)
        {
            if (fieldsDic != null)
            {
                if (!fieldsDic.Exists(delegate(FieldsDic f) { if (string.Compare(key, f.Key, true) == 0) { return true; } else { return false; } }))
                {
                    fieldsDic.Add(new FieldsDic(key,value));
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        public void SetFieldsDic(List<FieldsDic> list)
        {
            fieldsDic = list;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        

    }

    [Serializable]
    public class FieldsDic
    {
        public FieldsDic()
        {

        }
        public FieldsDic(FieldsDic field)
        {
            this.key = field.Key;
            this.value = field.Value;
        }
        public FieldsDic(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        private string key;

        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        private string value;
        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}