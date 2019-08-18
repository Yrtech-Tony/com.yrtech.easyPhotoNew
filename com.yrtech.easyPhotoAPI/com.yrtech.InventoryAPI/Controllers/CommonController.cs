using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryAPI.Service;
using com.yrtech.InventoryDAL;
using Infragistics.Documents.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        
        public void DownloadAnswerList(string projectCode, string shopCode)
        {
            //DownloadReport(projectCode, shopCode);
        }

        public void DownloadExcel(string excelName, string filePath, bool isDeleteAfterDownload = false)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            if (stream == null) return;
            if (string.IsNullOrEmpty(excelName))
            {
                excelName = "excel" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
            List<AnswerDto> answerList = answerService.GetShopAnswerList(projectId, shopId, "", "", "", "N");
            List<AnswerDto> answerList_new = answerService.GetShopAnswerList(projectId, shopId, "", "", "", "Y");
            foreach (AnswerDto answerDto in answerList)
            {
                answerDto.answerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString());
            }
            foreach (AnswerDto answerDto in answerList_new)
            {
                answerDto.answerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString());
            }
            int rowIndex = 1;
            int rowIndex1 = 1;
            #region 检查信息列表
            foreach (AnswerDto item in answerList)
            {
                //序号
                sheet.GetCell("A" + (rowIndex + 6)).Value = rowIndex.ToString();
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 6)).Value = shopName;
                //VinCode
                sheet.GetCell("C" + (rowIndex + 6)).Value = item.VinCode;
                //ModelName
                //sheet.GetCell("D" + (rowIndex + 6)).Value = item.ModelName;
                ////SubModelName
                //sheet.GetCell("E" + (rowIndex + 6)).Value = item.SubModelName;
                ////Stockage
                //sheet.GetCell("F" + (rowIndex + 6)).Value = item.StockAge;
                ////SaleFlag
                //sheet.GetCell("G" + (rowIndex + 6)).Value = item.SaleFlag;
                // 在库与否
                if (string.IsNullOrEmpty(item.PhotoName))
                {
                    sheet.GetCell("H" + (rowIndex + 6)).Value = "";
                    sheet.GetCell("I" + (rowIndex + 6)).Value = "1";
                    sheet.GetCell("J" + (rowIndex + 6)).Value = "无";
                }
                else
                {
                    sheet.GetCell("H" + (rowIndex + 6)).Value = "1";
                    sheet.GetCell("I" + (rowIndex + 6)).Value = "";
                    sheet.GetCell("J" + (rowIndex + 6)).Value = "有";
                }
                sheet.GetCell("K" + (rowIndex + 6)).Value = item.Remark;
                rowIndex++;
            }
            #endregion
            #region 在库
            sheet1.GetCell("B" + (2)).Value = shopName;
            foreach (Answer item in listY)
            {
                //序号
                sheet1.GetCell("A" + (rowIndex1 + 6)).Value = rowIndex1.ToString();
                //经销商名称
                sheet1.GetCell("B" + (rowIndex1 + 6)).Value = shopName;
                //VinCode
                sheet1.GetCell("C" + (rowIndex1 + 6)).Value = item.VinCode;
                //ModelName
                //sheet1.GetCell("D" + (rowIndex1 + 6)).Value = item.ModelName;
                //PhotoName
                string photoName = "";
                string[] photoNameList = item.PhotoName.Split(';');
                if (photoNameList.Length == 2)
                {
                    photoName = item.VinCode + ".jpg;" + item.VinCode + "_车尾.jpg";
                }
                if (photoNameList.Length == 3)
                {
                    photoName = item.VinCode + ".jpg;" + item.VinCode + "_车尾.jpg;" + item.VinCode + "_销售发票.jpg";
                }
                sheet1.GetCell("E" + (rowIndex1 + 6)).Value = photoName;
                //Remark
                sheet1.GetCell("F" + (rowIndex1 + 6)).Value = item.Remark;

                rowIndex1++;
            }
            #endregion
            //保存excel文件
            string fileName = shopName + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
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
    }
}