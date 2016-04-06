using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyTestWebBack.Models;
using System.Data;
namespace MyTestWebBack.Controllers
{
    public class DefaultController : baseController
    {
        //
        // GET: /Default/ 
        public string InputValue = "";
        public string OutputValue = "";
        public ActionResult Index()
        {
           
            Session["StoreTagDS"] = null;
            return View();
        }
        public MyCardTagSettingWCF.ReturnValue MSM_View_GetStoreTag(string TagName, string Status) //取得資料
        {
            MyCardTagSettingWCF.Service1Client MyCardTagSettingWCF = new MyCardTagSettingWCF.Service1Client();
            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            InputValue = "MSM_View_GetStoreTag" + "|TagName|" + TagName + "|Status|" + Status;
            try
            {
                ReturnValue = MyCardTagSettingWCF.MSM_View_GetStoreTag(TagName, Status);
            }
            catch (Exception ex)
            {
                OutputValue = "|回應|" + "Exception|" + ex.ToString();
                ErrorLog(InputValue + OutputValue);
            }

            //DataSet DS = new DataSet(); 
            //DS.Tables.Add("DS");
            //DS.Tables["DS"].Columns.Add("Sn");
            //DS.Tables["DS"].Columns.Add("TagName");
            //DS.Tables["DS"].Columns.Add("State");
            //DS.Tables["DS"].Columns.Add("ImageURL");
            //DS.Tables["DS"].Columns.Add("Priority");
            //DataRow DR;
            //for (int i = 0; i < 508; i++)
            //{
            //    DR = DS.Tables["DS"].NewRow();
            //    DR["Sn"] = i ;
            //    DR["TagName"] = "TagName" + i ;
            //    if (((i != 0) && (i % 7 == 0)) || ((i != 0) && (i % 3 == 0)))
            //        DR["State"] = 0;
            //    else
            //        DR["State"] = 1;

            //    DR["ImageURL"] = "ImageURL" + i;
            //    DR["Priority"] =  i;
            //    DS.Tables["DS"].Rows.Add(DR);
            //}
            //ReturnValue.ReturnDataSet = DS;
            //ReturnValue.ReturnMsgNo = 1;
            //ReturnValue.ReturnMsg = "成功";
            return ReturnValue;
        }
        public JsonResult IndexDataJson(string TagName, string Status, string rowCount, string newpage)
        {
            int page = string.IsNullOrEmpty(newpage).Equals(true) ? 0 : int.Parse(newpage);
            int row = string.IsNullOrEmpty(rowCount).Equals(true) ? 0 : int.Parse(rowCount);


            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            DataSet StoreTagDS = new DataSet();
            GetStoreTagResult GetStoreTagResult = new GetStoreTagResult();

            if (Session["StoreTagDS"] == null)
            {
                ReturnValue = MSM_View_GetStoreTag(TagName, Status);
                StoreTagDS = ReturnValue.ReturnDataSet;
                Session["StoreTagDS"] = ReturnValue.ReturnDataSet;
                GetStoreTagResult.ReturnMsgNo = ReturnValue.ReturnMsgNo.ToString();
                GetStoreTagResult.ReturnMsg = ReturnValue.ReturnMsg;
                Session["StoreTagQuery"] = TagName + "|" + Status;
            }
            else
            {
                StoreTagDS = (DataSet)Session["StoreTagDS"];
                GetStoreTagResult.ReturnMsgNo = "1";
                GetStoreTagResult.ReturnMsg = "成功";
                if (Session["StoreTagQuery"].ToString() != TagName + "|" + Status)
                {
                    ReturnValue = MSM_View_GetStoreTag(TagName, Status);
                    StoreTagDS = ReturnValue.ReturnDataSet;
                    Session["StoreTagDS"] = ReturnValue.ReturnDataSet;
                    GetStoreTagResult.ReturnMsgNo = ReturnValue.ReturnMsgNo.ToString();
                    GetStoreTagResult.ReturnMsg = ReturnValue.ReturnMsg;
                    Session["StoreTagQuery"] = TagName + "|" + Status;
                }
                else
                {
                    if (StoreTagDS.Tables.Count == 0)
                    {
                        ReturnValue = MSM_View_GetStoreTag(TagName, Status);
                        StoreTagDS = ReturnValue.ReturnDataSet;
                        Session["StoreTagDS"] = ReturnValue.ReturnDataSet;
                        GetStoreTagResult.ReturnMsgNo = ReturnValue.ReturnMsgNo.ToString();
                        GetStoreTagResult.ReturnMsg = ReturnValue.ReturnMsg;
                    }
                }

            }
            GetStoreTagResult.pageCount = "0";
            GetStoreTagResult.nowpage = string.IsNullOrEmpty(newpage).Equals(true) ? "1" : newpage;
            List<DataName> DataList = new List<DataName>();

            if (StoreTagDS.Tables.Count > 0)
            {
                if (StoreTagDS.Tables[0].Rows.Count > 0)
                {
                    int mod = StoreTagDS.Tables[0].Rows.Count % row;
                    if (mod == 0)
                        GetStoreTagResult.pageCount = (Math.Floor((double)StoreTagDS.Tables[0].Rows.Count / row)).ToString();
                    else
                        GetStoreTagResult.pageCount = (Math.Floor((double)StoreTagDS.Tables[0].Rows.Count / row) + 1).ToString();
                    if (int.Parse(GetStoreTagResult.pageCount) <  int.Parse(GetStoreTagResult.nowpage))
                    {
                        GetStoreTagResult.nowpage = "1";
                        page = 1;
                    }

                    int Geti = 0; //新頁的第一個位置
                    if (page > 1)
                        Geti = ((page - 1) * row);
                    int totle = 0;//新頁的最後一個位置
                    if ((Geti + row) <= StoreTagDS.Tables[0].Rows.Count)
                        totle = (Geti + row);
                    else
                        totle = StoreTagDS.Tables[0].Rows.Count;

                    for (int i = Geti; i < totle; i++) //ReturnValue.ReturnDataSet.Tables[0].Rows.Count  //分內容
                    {
                        DataName DataName = new DataName();
                        DataName.Sn = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["Sn"]);
                        DataName.TagName = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["TagName"]);
                        DataName.State = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["State"]) == "0" ? "停用" : Convert.ToString(StoreTagDS.Tables[0].Rows[i]["State"]) == "1" ? "啟用" : "";
                        DataName.ImageURL = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["ImageURL"]);
                        DataName.Priority = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["Priority"]);
                        DataList.Add(DataName);
                    }
                }
            }
            GetStoreTagResult.ReturnData = DataList;

            return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertDataJson(string TagName, string Status, string ImageURL, string Priority)
        {
            GetStoreTagResult GetStoreTagResult = new GetStoreTagResult();
            int num;
            if (int.TryParse(Convert.ToString(Status), out num) == false) //使用int.tryparse出來會是布林值
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "標籤啟用狀態請輸入數字";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (int.TryParse(Convert.ToString(Priority), out num) == false) //使用int.tryparse出來會是布林值
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "權重請輸入數字";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(TagName).Equals(true))
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "標籤名稱必填寫";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }

            MyCardTagSettingWCF.Service1Client MyCardTagSettingWCF = new MyCardTagSettingWCF.Service1Client();
            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            InputValue = "MSM_SP_StoreTagInsert" + "|TagName|" + TagName + "|StoreTagURL|" + "" + "|Status|" + Status + "|ImageURL|" + ImageURL + "|Priority|" + Priority + "|User|" + User;
            try
            {
                ReturnValue = MyCardTagSettingWCF.MSM_SP_StoreTagInsert(TagName, "", int.Parse(Status), ImageURL, int.Parse(Priority), "User");
            }
            catch (Exception ex)
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "發生例外錯誤";
                OutputValue = "|回應|" + "Exception|" + ex.ToString();
                ErrorLog(InputValue + OutputValue);
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (ReturnValue.ReturnMsgNo != 1)
            {
                OutputValue = "|回應|" + "|ReturnMsgNo|" + ReturnValue.ReturnMsgNo.ToString() + "|ReturnMsg|" + ReturnValue.ReturnMsg;
                ErrorLog(InputValue + OutputValue);
            }
            GetStoreTagResult.ReturnMsgNo = ReturnValue.ReturnMsgNo.ToString();
            GetStoreTagResult.ReturnMsg = ReturnValue.ReturnMsg;
            return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpDataJson(string Sn, string TagName, string Status, string ImageURL, string Priority, string QueryTagName, string QueryStatus)
        {
            GetStoreTagResult GetStoreTagResult = new GetStoreTagResult();
            int num;
            if (int.TryParse(Convert.ToString(Status), out num) == false) //使用int.tryparse出來會是布林值
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "標籤啟用狀態請輸入數字";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (int.TryParse(Convert.ToString(Priority), out num) == false) //使用int.tryparse出來會是布林值
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "權重請輸入數字";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(TagName).Equals(true))
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "標籤名稱必填寫";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            MyCardTagSettingWCF.Service1Client MyCardTagSettingWCF = new MyCardTagSettingWCF.Service1Client();
            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            InputValue = "MSM_SP_StoreTagUpdate" + "|Sn|" + Sn + "|TagName|" + TagName + "|StoreTagURL|" + "" + "|Status|" + Status + "|ImageURL|" + ImageURL + "|Priority|" + Priority + "|User|" + User;
            try
            {
                ReturnValue = MyCardTagSettingWCF.MSM_SP_StoreTagUpdate(int.Parse(Sn), TagName, "", int.Parse(Status), ImageURL, int.Parse(Priority), "User");
            }
            catch (Exception ex)
            {
                GetStoreTagResult.ReturnMsgNo = "-999";
                GetStoreTagResult.ReturnMsg = "發生例外錯誤";
                OutputValue = "|回應|" + "Exception|" + ex.ToString();
                ErrorLog(InputValue + OutputValue);
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            if (ReturnValue.ReturnMsgNo != 1)
            {
                OutputValue = "|回應|" + "|ReturnMsgNo|" + ReturnValue.ReturnMsgNo.ToString() + "|ReturnMsg|" + ReturnValue.ReturnMsg;
                ErrorLog(InputValue + OutputValue);
            }
            GetStoreTagResult.ReturnMsgNo = ReturnValue.ReturnMsgNo.ToString();
            GetStoreTagResult.ReturnMsg = ReturnValue.ReturnMsg;
            //這邊重新查詢 
            DataSet StoreTagDS = new DataSet();
            MyCardTagSettingWCF.ReturnValue ReturnValue1 = new MyCardTagSettingWCF.ReturnValue();
            ReturnValue = MSM_View_GetStoreTag(QueryTagName, QueryStatus);
            StoreTagDS = ReturnValue.ReturnDataSet;
            Session["StoreTagDS"] = ReturnValue.ReturnDataSet;
            List<DataName> DataList = new List<DataName>();
            if (StoreTagDS.Tables.Count > 0)
            {
                if (StoreTagDS.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < StoreTagDS.Tables[0].Rows.Count; i++)
                    {
                        DataName DataName = new DataName();
                        DataName.Sn = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["Sn"]);
                        DataName.TagName = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["TagName"]);
                        DataName.State = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["State"]) == "0" ? "停用" : Convert.ToString(StoreTagDS.Tables[0].Rows[i]["State"]) == "1" ? "啟用" : "";
                        DataName.ImageURL = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["ImageURL"]);
                        DataName.Priority = Convert.ToString(StoreTagDS.Tables[0].Rows[i]["Priority"]);
                        DataList.Add(DataName);
                    }
                }
            }
            GetStoreTagResult.ReturnData = DataList;
            return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
        }
        #region 匯出excel報表
        public JsonResult ReportExcel(string FileName)
        {
            GetStoreTagResult GetStoreTagResult = new GetStoreTagResult();

            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            DataSet StoreTagDS = new DataSet();
            if (Session["StoreTagDS"] == null)
            {
                GetStoreTagResult.ReturnMsgNo = "-1";
                GetStoreTagResult.ReturnMsg = "請重新查詢";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                StoreTagDS = (DataSet)Session["StoreTagDS"];
                if (StoreTagDS.Tables.Count == 0)
                {
                    GetStoreTagResult.ReturnMsgNo = "-1";
                    GetStoreTagResult.ReturnMsg = "請重新查詢";
                    return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
                }
            }
            //重寫DS標題及內容
            StoreTagDS = ChangeDS(StoreTagDS);

            ////方法一  最簡單的一種
            //System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = StoreTagDS;
            //gv.DataBind();

            //System.IO.StringWriter sw = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            //gv.RenderControl(htw);

            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.Write("<meta http-equiv=Content-Type content=text/html;charset=utf-8>");
            //Response.AddHeader("content-disposition", "attachment; filename=我的EXCEL.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = ""; 
            //Response.Output.Write(sw.ToString());
            //Response.Flush();
            //Response.End();
            //方法二   無法匯出但有檔案 我不會OUT拉 但可以用方法呼叫
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (StoreTagDS.Tables.Count > 0)
            {
                if (StoreTagDS.Tables[0].Rows.Count > 0)
                {
                    sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                    sb.AppendLine("<table border=1 cellspacing=0 cellpadding=0>");
                    sb.AppendLine("<tr>");
                    for (int j = 0; j < StoreTagDS.Tables[0].Columns.Count; j++)
                    {
                        sb.AppendLine("<td>" + StoreTagDS.Tables[0].Columns[j].ColumnName + "</td>");
                    }
                    sb.AppendLine("</tr>");
                    for (int i = 0; i < StoreTagDS.Tables[0].Rows.Count; i++)
                    {
                        sb.AppendLine("<tr>");
                        for (int j = 0; j < StoreTagDS.Tables[0].Columns.Count; j++)
                        {
                            sb.AppendLine("<td>" + StoreTagDS.Tables[0].Rows[i][j] + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.AppendLine("</table>");
                }
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Server.MapPath("~/" + FileName + ".xls"), false, System.Text.Encoding.UTF8);
            sw.WriteLine(sb.ToString());
            sw.Close();

            GetStoreTagResult.ReturnMsgNo = "1";
            GetStoreTagResult.ReturnMsg = "成功";
            return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 重新改寫DS標題及內容
        public DataSet ChangeDS(DataSet OldDS)
        {
            DataSet ChangeDS = new DataSet();
            ChangeDS.Tables.Add("ChangeDS");
            ChangeDS.Tables["ChangeDS"].Columns.Add("標籤Sn");
            ChangeDS.Tables["ChangeDS"].Columns.Add("標籤名稱");
            ChangeDS.Tables["ChangeDS"].Columns.Add("標籤啟用狀態");
            ChangeDS.Tables["ChangeDS"].Columns.Add("標籤圖片網址");
            ChangeDS.Tables["ChangeDS"].Columns.Add("權重");
            DataRow DR;
            for (int i = 0; i < OldDS.Tables[0].Rows.Count; i++)
            {
                DR = ChangeDS.Tables["ChangeDS"].NewRow();
                DR["標籤Sn"] = OldDS.Tables[0].Rows[i]["Sn"];
                DR["標籤名稱"] = OldDS.Tables[0].Rows[i]["TagName"];
                DR["標籤啟用狀態"] = OldDS.Tables[0].Rows[i]["State"].ToString() == "0" ? "停用" : OldDS.Tables[0].Rows[i]["State"].ToString() == "1" ? "啟用" : "";
                DR["標籤圖片網址"] = OldDS.Tables[0].Rows[i]["ImageURL"];
                DR["權重"] = OldDS.Tables[0].Rows[i]["Priority"];
                ChangeDS.Tables["ChangeDS"].Rows.Add(DR);
            } 
            return ChangeDS;
        }
        #endregion
        #region 寄信excel報表
        public JsonResult EmailExcel(string FileName)
        {
            GetStoreTagResult GetStoreTagResult = new GetStoreTagResult();

            MyCardTagSettingWCF.ReturnValue ReturnValue = new MyCardTagSettingWCF.ReturnValue();
            DataSet StoreTagDS = new DataSet();
            if (Session["StoreTagDS"] == null)
            {
                GetStoreTagResult.ReturnMsgNo = "-1";
                GetStoreTagResult.ReturnMsg = "請重新查詢";
                return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                StoreTagDS = (DataSet)Session["StoreTagDS"];
                if (StoreTagDS.Tables.Count == 0)
                {
                    GetStoreTagResult.ReturnMsgNo = "-1";
                    GetStoreTagResult.ReturnMsg = "請重新查詢";
                    return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
                }
            }
            //重寫DS標題及內容
            StoreTagDS = ChangeDS(StoreTagDS);
            //方法二   無法匯出但有檔案 我不會OUT拉 但可以用方法呼叫
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (StoreTagDS.Tables.Count > 0)
            {
                if (StoreTagDS.Tables[0].Rows.Count > 0)
                {
                    sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                    sb.AppendLine("<table border=1 cellspacing=0 cellpadding=0>");
                    sb.AppendLine("<tr>");
                    for (int j = 0; j < StoreTagDS.Tables[0].Columns.Count; j++)
                    {
                        sb.AppendLine("<td>" + StoreTagDS.Tables[0].Columns[j].ColumnName + "</td>");
                    }
                    sb.AppendLine("</tr>");
                    for (int i = 0; i < StoreTagDS.Tables[0].Rows.Count; i++)
                    {
                        sb.AppendLine("<tr>");
                        for (int j = 0; j < StoreTagDS.Tables[0].Columns.Count; j++)
                        {
                            sb.AppendLine("<td>" + StoreTagDS.Tables[0].Rows[i][j] + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.AppendLine("</table>");
                }
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Server.MapPath("~/" + FileName + ".xls"), false, System.Text.Encoding.UTF8);
            sw.WriteLine(sb.ToString());
            sw.Close();
            //準備名單跟寄信
            System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(Server.MapPath("~/" + FileName + ".xls"), System.Net.Mime.MediaTypeNames.Application.Octet);
            System.Net.Mail.MailMessage newMail = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient(MyTestWebBack.Properties.Settings.Default.mailserviceip);


            newMail.From = new System.Net.Mail.MailAddress("lex@soft-world.com.tw", "Lex"); //從
            newMail.To.Add(new System.Net.Mail.MailAddress("lex@soft-world.com.tw", "Lex"));//到
            newMail.Attachments.Add(data);//檔案
            newMail.Subject = "我要寄信";
            newMail.IsBodyHtml = true;
            newMail.Body = "這是信件內容";
            newMail.Priority = System.Net.Mail.MailPriority.Normal;//優先權
            try
            {
                smtpMail.Send(newMail);
                GetStoreTagResult.ReturnMsgNo = "1";
                GetStoreTagResult.ReturnMsg = "寄信成功";
            }
            catch (Exception ex)
            {
                GetStoreTagResult.ReturnMsgNo = "1";
                GetStoreTagResult.ReturnMsg = "寄信失敗";
                ErrorLog("寄信失敗" + ex.ToString());
            }
            return Json(GetStoreTagResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region ErrorLog
        public void ErrorLog(string Message)
        {
            try
            {
                System.IO.File.WriteAllText(Server.MapPath("ErrLogTxt.txt"), Message + Environment.NewLine);
            }
            catch (Exception) { }
        }
        #endregion
    }
}

//類別
public class GetStoreTagResult
{
    public string ReturnMsgNo { get; set; }
    public string ReturnMsg { get; set; }
    public List<DataName> ReturnData { get; set; }
    //分頁面
    public string pageCount { get; set; }
    public string nowpage { get; set; }
}
public class DataName
{
    public string Sn { get; set; }
    public string TagName { get; set; }
    public string State { get; set; }
    public string ImageURL { get; set; }
    public string Priority { get; set; }
}