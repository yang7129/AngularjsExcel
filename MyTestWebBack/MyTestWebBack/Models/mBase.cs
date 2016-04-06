using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyTestWebBack.Models
{
    public class mbase
    {
        public mbase(string sButtonCode)
        {
            switch (sButtonCode.ToUpper().Trim())
            {
                case "A":
                    intButtonCode = EnumButtonCode.Add;
                    break;
                case "D":
                    intButtonCode = EnumButtonCode.Delete;
                    break;
                case "E":
                    intButtonCode = EnumButtonCode.Edit;
                    break;
                case "P":
                    intButtonCode = EnumButtonCode.Print;
                    break;
                case "Q":
                    intButtonCode = EnumButtonCode.Query;
                    break;
                case "M":
                    intButtonCode = EnumButtonCode.Mail;
                    break;
                case "EX":
                    intButtonCode = EnumButtonCode.Export;
                    break;
                default:
                    intButtonCode = EnumButtonCode.None;
                    break;
            }

        }
        public EnumButtonCode intButtonCode;
        public enum EnumButtonCode
        {
            None,
            //無
            Add,
            //新增
            Delete,
            //刪除   
            Edit,
            //修改
            Print,
            //列印
            Query,
            //查詢
            Mail,
            //寄信
            Export
            //匯出

        }
    }
}