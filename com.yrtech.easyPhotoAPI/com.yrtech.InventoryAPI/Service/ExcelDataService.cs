using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using Infragistics.Documents.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace com.yrtech.InventoryAPI.Service
{
    public class ExcelDataService
    {
        string basePath = HostingEnvironment.MapPath(@"~/");
        MasterService masterService = new MasterService();
        AnswerService answerService = new AnswerService();
        // 导出用户信息
        public string UserInfoExport(string projectId, string key)
        {
            List<UserInfo> list = masterService.GetUserInfo(projectId, key);
            Workbook book = Workbook.Load(basePath + @"Content\Excel\" + "UserInfoExport.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (UserInfo item in list)
            {
                //经销商代码
                sheet.GetCell("A" + (rowIndex + 1)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("B" + (rowIndex + 1)).Value = item.ShopName;
                //密码
                sheet.GetCell("C" + (rowIndex + 1)).Value = item.Password;
                //过期日期
                if (item.ExpireDateTime == null)
                {
                    sheet.GetCell("D" + (rowIndex + 1)).Value = "";
                }
                else
                {
                    sheet.GetCell("D" + (rowIndex + 1)).Value = item.ExpireDateTime.ToString();
                }

                rowIndex++;
            }

            //保存excel文件
            string fileName = "UserInfo" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);
            return filePath.Replace(basePath, ""); ;
        }

        // 清单导入
        public List<AnswerDto> AnswerImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Content\Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<AnswerDto> list = new List<AnswerDto>();
            for (int i = 0; i < 100000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 2)).Value == null ? "" : sheet.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                AnswerDto answer = new AnswerDto();
                answer.ShopCode = shopCode;
                answer.ShopName = sheet.GetCell("B" + (i + 2)).Value == null ? "" : sheet.GetCell("B" + (i + 2)).Value.ToString().Trim();
                answer.CheckCode = sheet.GetCell("C" + (i + 2)).Value == null ? "" : sheet.GetCell("C" + (i + 2)).Value.ToString().Trim();
                answer.CheckTypeName = sheet.GetCell("D" + (i + 2)).Value == null ? "" : sheet.GetCell("D" + (i + 2)).Value.ToString().Trim();
                answer.Column1 = sheet.GetCell("E" + (i + 2)).Value == null ? "" : sheet.GetCell("E" + (i + 2)).Value.ToString().Trim();
                answer.Column2 = sheet.GetCell("F" + (i + 2)).Value == null ? "" : sheet.GetCell("F" + (i + 2)).Value.ToString().Trim();
                answer.Column3 = sheet.GetCell("G" + (i + 2)).Value == null ? "" : sheet.GetCell("G" + (i + 2)).Value.ToString().Trim();
                answer.Column4 = sheet.GetCell("H" + (i + 2)).Value == null ? "" : sheet.GetCell("H" + (i + 2)).Value.ToString().Trim();
                answer.Column5 = sheet.GetCell("I" + (i + 2)).Value == null ? "" : sheet.GetCell("I" + (i + 2)).Value.ToString().Trim();
                answer.Column6 = sheet.GetCell("J" + (i + 2)).Value == null ? "" : sheet.GetCell("J" + (i + 2)).Value.ToString().Trim();
                answer.Column7 = sheet.GetCell("K" + (i + 2)).Value == null ? "" : sheet.GetCell("K" + (i + 2)).Value.ToString().Trim();
                answer.Column8 = sheet.GetCell("L" + (i + 2)).Value == null ? "" : sheet.GetCell("L" + (i + 2)).Value.ToString().Trim();
                answer.Column9 = sheet.GetCell("M" + (i + 2)).Value == null ? "" : sheet.GetCell("M" + (i + 2)).Value.ToString().Trim();
                list.Add(answer);
            }
            return list;
        }

        // 导出清单
        public string AnswerExport(string projectId, string shopCode)
        {
            List<AnswerDto> list_N = answerService.GetShopAnswerList("",projectId, shopCode, "", "", "","N","");
            List<AnswerDto> list_Y = answerService.GetShopAnswerList("",projectId, shopCode, "", "", "", "Y","");
            foreach (AnswerDto answerDto in list_N)
            {
                answerDto.AnswerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString(), "", "");
            }
            foreach (AnswerDto answerDto in list_Y)
            {
                answerDto.AnswerPhotoList = answerService.GetAnswerPhotoList(answerDto.AnswerId.ToString(), "", "");
            }
            // 扩展列
            List<ExtendColumnProjectDto> ColumnList_List = masterService.GetExtendColumnProject(projectId, "");
            // 在新增显示的扩展列
            List<ExtendColumnProjectDto> ColumnList_add = ColumnList_List.Where(x => x.AddShowChk==true).ToList();

            Workbook book = Workbook.Load(basePath + @"Content\Excel\" + "AnswerExport.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            
            // 填充表头
            string[] head ={ "H", "I", "J", "K", "L", "M", "N", "O", "P" };
            for (int i = 0;i< ColumnList_List.Count;i++)
            {
                sheet.GetCell(head[i]+5).Value = ColumnList_List[i].ColumnName;
            }
            int rowIndex = 1;
            foreach (AnswerDto item in list_N)
            {
                sheet.GetCell("A" + (rowIndex + 5)).Value = rowIndex.ToString();
                //经销商代码
                sheet.GetCell("B" + (rowIndex + 5)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("C" + (rowIndex + 5)).Value = item.ShopName;
                //检查代码
                sheet.GetCell("D" + (rowIndex + 5)).Value = item.CheckCode;
                //检查类型
                sheet.GetCell("E" + (rowIndex + 5)).Value = item.CheckTypeName;
                //是否有照片
                if (item.AnswerPhotoList != null && item.AnswerPhotoList.Count > 0)
                {
                    sheet.GetCell("F" + (rowIndex + 5)).Value = "有";
                }
                else
                {
                    sheet.GetCell("F" + (rowIndex + 5)).Value = "无";
                }
                sheet.GetCell("G" + (rowIndex + 5)).Value = item.RemarkName;
                sheet.GetCell("H" + (rowIndex + 5)).Value = item.Column1;
                sheet.GetCell("I" + (rowIndex + 5)).Value = item.Column2;
                sheet.GetCell("J" + (rowIndex + 5)).Value = item.Column3;
                sheet.GetCell("K" + (rowIndex + 5)).Value = item.Column4;
                sheet.GetCell("L" + (rowIndex + 5)).Value = item.Column5;
                sheet.GetCell("M" + (rowIndex + 5)).Value = item.Column6;
                sheet.GetCell("N" + (rowIndex + 5)).Value = item.Column7;
                sheet.GetCell("O" + (rowIndex + 5)).Value = item.Column8;
                sheet.GetCell("P" + (rowIndex + 5)).Value = item.Column9;
                rowIndex++;
            }
            //填充数据
            Worksheet sheet1 = book.Worksheets[1];
            // 填充表头
            string[] head1 = { "G","H", "I", "J", "K", "L", "M", "N", "O" };
            for (int i = 0; i < ColumnList_add.Count; i++)
            {
                sheet1.GetCell(head1[i] + 5).Value = ColumnList_add[i].ColumnName;
            }
            int rowIndex1 = 1;
            foreach (AnswerDto item in list_Y)
            {
                sheet1.GetCell("A" + (rowIndex1 + 5)).Value = rowIndex1.ToString();
                //经销商代码
                sheet1.GetCell("B" + (rowIndex1 + 5)).Value = item.ShopCode;
                //经销商名称
                sheet1.GetCell("C" + (rowIndex1 + 5)).Value = item.ShopName;
                //检查代码
                sheet1.GetCell("D" + (rowIndex1 + 5)).Value = item.CheckCode;
                //是否有照片
                if (item.AnswerPhotoList != null && item.AnswerPhotoList.Count > 0)
                {
                    sheet1.GetCell("E" + (rowIndex1 + 5)).Value = "有";
                }
                else
                {
                    sheet1.GetCell("E" + (rowIndex1 + 5)).Value = "无";
                }
                sheet1.GetCell("F" + (rowIndex1 + 5)).Value = item.RemarkName;
                sheet1.GetCell("G" + (rowIndex1 + 5)).Value = item.Column1;
                sheet1.GetCell("H" + (rowIndex1 + 5)).Value = item.Column2;
                sheet1.GetCell("I" + (rowIndex1 + 5)).Value = item.Column3;
                sheet1.GetCell("J" + (rowIndex1 + 5)).Value = item.Column4;
                sheet1.GetCell("K" + (rowIndex1 + 5)).Value = item.Column5;
                sheet1.GetCell("L" + (rowIndex1 + 5)).Value = item.Column6;
                sheet1.GetCell("M" + (rowIndex1 + 5)).Value = item.Column7;
                sheet1.GetCell("N" + (rowIndex1 + 5)).Value = item.Column8;
                sheet1.GetCell("O" + (rowIndex1 + 5)).Value = item.Column9;
                rowIndex1++;
            }

            //保存excel文件
            string shopName = "经销商导出清单";
            List<UserInfo> userInfoList = masterService.GetUserInfo(projectId, shopCode);
            if (userInfoList != null && userInfoList.Count > 0)
            {
                shopName = userInfoList[0].ShopName;
            }
            string fileName = shopName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string dirPath = basePath + @"\Temp\";
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string filePath = dirPath + fileName;
            book.Save(filePath);
            return filePath.Replace(basePath, ""); ;
        }

    }
}