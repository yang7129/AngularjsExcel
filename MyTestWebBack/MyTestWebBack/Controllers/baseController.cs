using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTestWebBack.Models;
using System.Data;

namespace MyTestWebBack.Controllers
{
    public class baseController : Controller
    {
        //=======================================
        private string APCODE = MyTestWebBack.Properties.Settings.Default.APCODE;
        public string UserName;
        public string UserAccount = "blueway423";
        public string UserEmail = "blueway423@soft-world.com.tw";
        public string CustIp; 
        private mbase[] ButtonStruct;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Request.ServerVariables["HTTP_CLIENTIP"] != null && Request.ServerVariables["HTTP_CLIENTIP"] != "")
                CustIp = Request.ServerVariables["HTTP_CLIENTIP"];
            else if (Request.ServerVariables["REMOTE_ADDR"] != null && Request.ServerVariables["REMOTE_ADDR"] != "")
                CustIp = Request.ServerVariables["REMOTE_ADDR"];
            else if (Request.UserHostAddress != null && Request.UserHostAddress != "")
                CustIp = Request.UserHostAddress;
            else
                CustIp = "127.0.1.1";
            ////檢查是否有權限
            //使用者所有的資訊

          
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            String Errormsg = String.Empty;

            //Exception unhandledException = Server.GetLastError();
            Exception unhandledException = filterContext.Exception;
            //Response.Clear();

            Exception httpException = unhandledException as Exception;

            Errormsg = "{3}發生例外網頁:{0}錯誤訊息:{1}堆疊內容:{2}";

            //if (httpException != null && !httpException.GetType().IsAssignableFrom(typeof(HttpException)))
            //{

            Errormsg = String.Format(Errormsg, Request.Path + Environment.NewLine,

                unhandledException.GetBaseException().Message + Environment.NewLine,
                unhandledException.StackTrace + Environment.NewLine,
                DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + Environment.NewLine + "User:" + UserName + Environment.NewLine + "IP:" + CustIp + Environment.NewLine);
            //寫入文字檔
            Session["Errormsg"] = unhandledException.GetBaseException().Message;
            System.IO.File.AppendAllText(Server.MapPath("~/baseController.txt"), Errormsg);



            
            Server.ClearError();
            //}
            base.OnException(filterContext);
            //ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            filterContext.Result = new ViewResult
            {
                ViewName = "Error"
            };
            filterContext.ExceptionHandled = true;
        }

#if DEBUG

#else

#endif
        public void WorkLogTxt(string Message)
        {
            //if (MyCardTagSetting.Properties.Settings.Default.DebMode.ToUpper() == "FALSE")
            //    System.IO.File.WriteAllText(Server.MapPath("~/ErrLogTxt.txt"), Message + Environment.NewLine);
            //else
            System.IO.File.AppendAllText(Server.MapPath("~/ErrLogTxt.txt"), Message + Environment.NewLine);
        }
    }
}
