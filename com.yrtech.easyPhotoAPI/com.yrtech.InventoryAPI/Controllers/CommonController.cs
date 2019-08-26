using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryDAL;
using Infragistics.Documents.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace com.yrtech.InventoryAPI.Controllers
{
    public class CommonController : Controller
    {
        AnswerService answerService = new AnswerService();
        MasterService masterService = new MasterService();
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        public void DownloadExcel(string excelName, string filePath, bool isDeleteAfterDownload = false)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            if (stream == null) return;
            if (string.IsNullOrEmpty(excelName))
            {
                excelName = "excel" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            }
            byte[] bytes = new byte[(int)stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Response.AddHeader("content-type", "application/x-msdownload");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(excelName, Encoding.GetEncoding("UTF-8")));
            Response.BinaryWrite(bytes);
            Response.End();
            if (isDeleteAfterDownload)
            {
                System.IO.File.Delete(filePath);
            }
        }
        public void DownloadReport(string projectId, string shopId)
        {
            List<AnswerDto> answerList = answerService.GetShopAnswerList(projectId, shopId, "", "", "", "");
            Workbook book = Workbook.Load(Server.MapPath("~") + @"Content\Excel\" + "easyPhotoExport.xls", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            Worksheet sheet1 = book.Worksheets[1];
            int rowIndex = 1;
            int rowIndex1 = 1;
            List<AnswerDto> answerList_N = answerList.Where(x => x.AddCheck == "N").ToList();
            List<AnswerDto> answerList_Y = answerList.Where(x => x.AddCheck == "Y").ToList();
            Projects project = masterService.GetProject("", projectId, "", "", "")[0];
            sheet.GetCell("D" + 2).Value = project.ProjectName;
            sheet1.GetCell("D" + 2).Value = project.ProjectName;
            if (project.ScoreShow == true)
            {
                sheet.GetCell("J" + 6).Value = "得分";
                sheet1.GetCell("G" + 6).Value = "得分";
            }
            #region 检查信息列表
            foreach (AnswerDto item in answerList_N)
            {
                //序号
                sheet.GetCell("A" + (rowIndex + 7)).Value = rowIndex.ToString();
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 7)).Value = item.ShopName;
                //VinCode
                sheet.GetCell("C" + (rowIndex + 7)).Value = item.CheckCode;
                // 
                sheet.GetCell("D" + (rowIndex + 7)).Value = item.CheckTypeName;
                // 
                if (item.answerPhotoList != null && item.answerPhotoList.Count > 0)
                {
                    string photoName = "";
                    foreach (AnswerPhotoDto photo in item.answerPhotoList)
                    {
                        if (photo == item.answerPhotoList[item.answerPhotoList.Count - 1])
                        {
                            photoName += photo.PhotoNameServer;
                        }
                        else
                        {
                            photoName += photo.PhotoNameServer + ";";
                        }
                    }
                    sheet.GetCell("E" + (rowIndex + 7)).Value = "1";
                    sheet.GetCell("F" + (rowIndex + 7)).Value = "";
                    sheet.GetCell("G" + (rowIndex + 7)).Value = photoName;
                }
                else
                {
                    sheet.GetCell("E" + (rowIndex + 7)).Value = "";
                    sheet.GetCell("F" + (rowIndex + 7)).Value = "1";
                    sheet.GetCell("G" + (rowIndex + 7)).Value = "无";
                }
                sheet.GetCell("H" + (rowIndex + 7)).Value = item.Remark;
                sheet.GetCell("I" + (rowIndex + 7)).Value = item.OtherProperty;
                if (project.ScoreShow == true)
                {
                    sheet.GetCell("J" + (rowIndex + 7)).Value = item.Score;
                }
                else
                {
                    sheet.GetCell("J" + (rowIndex + 7)).Value = "";
                }
                rowIndex++;
            }
            #endregion
            #region 新增

            foreach (AnswerDto item in answerList_Y)
            {
                //序号
                sheet1.GetCell("A" + (rowIndex + 7)).Value = rowIndex.ToString();
                //经销商名称
                sheet1.GetCell("B" + (rowIndex + 7)).Value = item.ShopName;
                //VinCode
                sheet1.GetCell("C" + (rowIndex + 7)).Value = item.CheckCode;
                // 
                if (item.answerPhotoList != null && item.answerPhotoList.Count > 0)
                {
                    string photoName = "";
                    foreach (AnswerPhotoDto photo in item.answerPhotoList)
                    {
                        if (photo == item.answerPhotoList[item.answerPhotoList.Count - 1])
                        {
                            photoName += photo.PhotoNameServer;
                        }
                        else
                        {
                            photoName += photo.PhotoNameServer + ";";
                        }
                    }
                    sheet1.GetCell("D" + (rowIndex + 7)).Value = photoName;
                }
                else
                {
                    sheet1.GetCell("D" + (rowIndex + 7)).Value = "";
                }
                sheet1.GetCell("E" + (rowIndex + 7)).Value = item.Remark;
                sheet1.GetCell("F" + (rowIndex + 7)).Value = item.OtherProperty;
                if (project.ScoreShow == true)
                {
                    sheet.GetCell("G" + (rowIndex + 7)).Value = item.Score;
                }
                else
                {
                    sheet.GetCell("G" + (rowIndex + 7)).Value = "";
                }
                rowIndex1++;
            }
            #endregion
            //保存excel文件
            string fileName = project.ProjectName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            string dirPath = Server.MapPath("~") + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);
            DownloadExcel(fileName, filePath, true);
        }
        public void DownLoadAnswerImportExcel()
        {
            string fileName = "easyPhotoImport";

            string dirPath = Server.MapPath("~") + @"\Content\Excel\";
            string dirPath_Copy = Server.MapPath("~") + @"\Temp\";
            System.IO.File.Copy(dirPath + fileName + ".xls", dirPath_Copy + fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls");
            DirectoryInfo dir = new DirectoryInfo(dirPath_Copy);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath_Copy + dirPath_Copy + fileName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            DownloadExcel(fileName+".xls", filePath, true);
        }


    }

}