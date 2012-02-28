using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;

using We7.CMS.ShopService;
using We7.CMS.Config;
using We7.CMS.Common.Enum;
using We7.CMS.Common.PF;
using System.Xml;
using We7.Framework;
using We7.Framework.Config;
using We7.CMS.Accounts;
using Thinkment.Data;
using System.Text;
using Newtonsoft.Json;

namespace We7.CMS.Web.Admin.theme.classic
{
    public partial class ShopData : System.Web.UI.Page
    {
        #region 字段属性
        protected string RequestAction
        {
            get
            {
                return Request["action"];
            }
        }
        
        #endregion

        #region We7插件商城相关
        private  We7.CMS.ShopService.ShopService _ShopService;
        /// <summary>
        /// 商城Service地址 todo
        /// </summary>
        public  We7.CMS.ShopService.ShopService ShopService
        {
            get
            {
                if (_ShopService == null)
                {
                    _ShopService = new We7.CMS.ShopService.ShopService();
                    _ShopService.Timeout = 10000;
                    _ShopService.Url = GeneralConfigs.GetConfig().ShopService.TrimEnd('/').TrimEnd('\\') + "/Plugins/ShopPlugin/ShopService.asmx";
                }
                return _ShopService;
            }
        }

        /// <summary>
        /// 通过Web Service Ping接口测试插件商城接口是否可用
        /// </summary>
        /// <returns>true：可用</returns>
        public virtual bool IsShopServicesCanWork()
        {
            try
            {
                string result = ShopService.Ping();
                return result.Equals("Pong");
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(BasePage), ex);
                return false;
            }
        }


        /// <summary>
        /// 获取推荐店铺
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<StoreModel> GetRecommendStore(int count)
        {
            try
            {
                StoreModel[] stores = ShopService.GetRecommendStore(count);
                List<StoreModel> listStores = null;
                if (stores.Length > 0)
                {
                    listStores = new List<StoreModel>(stores);
                }
                return listStores;
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(BasePage), ex);
                return null;
            }
        }

        /// <summary>
        /// 获取推荐商品
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<ProductInfo> GetRecommendProduct(int count)
        {
            try
            {
                List<ProductInfo> listProducts = null;
                ProductInfo[] products = ShopService.GetRecommendProduct(count);
                if (products.Length > 0)
                {
                    listProducts = new List<ProductInfo>(products);
                }
                return listProducts;
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(BasePage), ex);
                return null;
            }
        }

        /// <summary>
        /// 获取评级对应的星号字串
        /// </summary>
        /// <param name="str">0-6,颗星</param>
        /// <returns>3星,例：★★★☆☆</returns>
        public virtual string GetLevelString(string str)
        {
            int stars = 0;
            int.TryParse(str, out stars);

            int max = 5;
            int nostar = max - stars;
            StringBuilder sb = new StringBuilder();
            sb.Append(new string('★', stars));
            sb.Append(new string('☆', nostar));

            return sb.ToString();
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="input">输入字串</param>
        /// <param name="count">最大字数</param>
        /// <param name="omit">省略符</param>
        /// <returns></returns>
        public virtual string GetChopString(string input, int count, string omit)
        {
            string result = input;
            if (input.Length > count)
            {
                result = We7.Framework.Util.Utils.CutString(input, 0, count - omit.Length);
                result += omit;
            }
            return result;
        }

        /// <summary>
        /// 获取没有html符号的字串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual string GetClearHtml(string input)
        {
            return We7Helper.RemoveHtml(input);
        }

        /// <summary>
        /// 获取免费的模板
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<ProductInfo> GetFreeTemplates(int count)
        {
            try
            {
                List<ProductInfo> listProducts = null;
                ProductInfo[] products = ShopService.GetRecommendProductByType(count, "mb", -1);
                if (products.Length > 0)
                {
                    listProducts = new List<ProductInfo>(products);
                }
                return listProducts;
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(BasePage), ex);
                return null;
            }
        }

        /// <summary>
        /// 获取文件尺寸对应的M数
        /// </summary>
        /// <param name="productSize">字节数</param>
        /// <returns></returns>
        public string GetProductFileSize(string productSize)
        {
            string result = string.Empty;
            int sizes;
            int.TryParse(productSize, out sizes);
            if (sizes == 0)
                result = "0";
            if (sizes > 0)
                result = ((double)sizes / (double)1048576).ToString("f2");
            return result + "M";
        }

        /// <summary>
        /// 根据价格字段查询商品是否免费
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsFree(object input)
        {
            int price = 0;
            int.TryParse(input.ToString(), out price);

            return price.Equals(0);
        }

        /// <summary>
        /// 站点是否绑定商城
        /// </summary>
        /// <returns></returns>
        public bool IsSiteBindShop()
        {
            string sln = SiteConfigs.GetConfig().ShopLoginName.Trim();
            if (string.IsNullOrEmpty(sln))
                return false;

            try
            {
                //帐号检验
                SiteConfigInfo si = SiteConfigs.GetConfig();
                string[] states = ShopService.CheckSite(si.ShopLoginName, si.ShopPassword, si.SiteUrl);
                if (states != null && states.Length > 0 && states[0] == "1")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                We7.Framework.LogHelper.WriteLog(typeof(BasePage), ex);
                return false;
            }
        }
        #endregion

                /// <summary>
        /// 页面初始化
        ///     检查商城Service是否可用
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (RequestAction)
            {
                case "shop":
                    GetShop();
                    break;
                case "product":
                    GetProducts();
                    break;
            }
        }

        protected void GetProducts()
        {
            //检查商城的ShopService是否可用
            bool isShopServiceAvailable = IsShopServicesCanWork();

            if (isShopServiceAvailable)
            {
                 List<We7.CMS.ShopService.ProductInfo> list =  GetRecommendProduct(5);
                 if (list != null && list.Count > 0)
                 {
                     List<JsonProductModel> listResult = new List<JsonProductModel>();
                     for (int i = 0; i < list.Count; i++)
                     {
                         JsonProductModel model = new JsonProductModel();
                         model.Name = list[i].Name;
                         model.NameHtml = GetChopString(model.Name, 8, "...");
                         model.LevelHtml = GetLevelString(list[i].Level.ToString());
                         model.PageUrl = list[i].PageUrl;
                         model.Thumbnail = list[i].Thumbnail;
                         model.Point = list[i].Point.ToString();
                         model.Price = list[i].Price.ToString();
                         model.Sales = list[i].Sales.ToString();
                         listResult.Add(model);
                     }
                     string result =  JavaScriptConvert.SerializeObject(listResult);
                     Response.Clear();
                     Response.Write(result);
                     Response.Flush();
                     Response.End();
                 }
            }
        }

        protected void GetShop()
        {
            //检查商城的ShopService是否可用
            bool isShopServiceAvailable = IsShopServicesCanWork();

            if (isShopServiceAvailable)
            {
                List<We7.CMS.ShopService.StoreModel> list = GetRecommendStore(5);
                if (list != null && list.Count > 0)
                {
                    List<JsonShopModel> listResult = new List<JsonShopModel>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        JsonShopModel model = new JsonShopModel();
                        model.Url = list[i].Url;
                        model.NameHtml = GetChopString(list[i].StoreName, 10, "...");
                        model.LevelHtml = GetLevelString(list[i].Level.ToString());
                        model.StoreIntro = GetClearHtml(list[i].StoreIntro);
                        model.Face = list[i].Face;
                        model.TechnicalLevel = list[i].TechnicalLevel;
                        listResult.Add(model);
                    }
                    string result = JavaScriptConvert.SerializeObject(listResult);
                    Response.ContentType = "text/plain";
                    Response.Clear();
                    Response.Write(result);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }

    /// <summary>
    /// 推荐产品JSON模型
    /// </summary>
    public class JsonProductModel
    {
        public string PageUrl { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }
        public string NameHtml { get; set; }
        public string Price { get; set; }
        public string Sales { get; set; }
        public string LevelHtml { get; set; }
        public string Point { get; set; }
    }

    /// <summary>
    /// 推荐商铺JSON模型
    /// </summary>
    public class JsonShopModel
    {
        public string Url { get; set; }
        public string StoreIntro { get; set; }
        public string Face { get; set; }
        public string NameHtml { get; set; }
        public string TechnicalLevel { get; set; }
        public string LevelHtml { get; set; }
    }
    
}