using System;
using System.Collections.Generic;
using System.Text;

namespace We7.CMS.Data
{
    public class JoinEx : IJoin
    {
        public void AddDic(IJoin ex)
        {
            joinInfo.Add(ex.MainField, ex);
        }
        private Dictionary<string, IJoin> joinInfo=new Dictionary<string,IJoin>();
        public IDictionary<string, IJoin> JoinInfo
        {
            get
            {
                return joinInfo;
            }
            set
            {
                joinInfo = value as Dictionary<string, IJoin>;
            }
        }

        private string priMaryKeyName;
        public string PriMaryKeyName
        {
            get
            {
                return priMaryKeyName;
            }
            set
            {
                priMaryKeyName = value;
            }
        }

        private string toTableName;
        public string ToTableName
        {
            get
            {
                return toTableName;
            }
            set
            {
                toTableName = value;
            }
        }
        private string toField;
        public string ToField
        {
            get
            {
                return toField;
            }
            set
            {
                toField = value;
            }
        }
        private string mainField;
        public string MainField
        {
            get
            {
                return mainField;
            }
            set
            {
                mainField = value;
            }
        }

        private bool isSiteGroup;
        public bool IsSiteGroup
        {
            get
            {
                return isSiteGroup;
            }
            set
            {
                isSiteGroup = value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }


        public Thinkment.Data.Criteria Condition
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
