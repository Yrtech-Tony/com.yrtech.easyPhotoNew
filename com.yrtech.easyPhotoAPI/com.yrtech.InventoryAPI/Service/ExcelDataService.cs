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
            List<UserInfo> list = masterService.GetUserInfo(projectId, key, "");
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

        // 导入用户信息
        public List<UserInfoDto> UserInfoImport(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Content\Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            Worksheet sheet = book.Worksheets[0];
            List<UserInfoDto> list = new List<UserInfoDto>();
            for (int i = 0; i < 100000; i++)
            {
                string shopCode = sheet.GetCell("A" + (i + 2)).Value == null ? "" : sheet.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(shopCode)) break;
                UserInfoDto userInfo = new UserInfoDto();
                userInfo.ShopCode = shopCode;
                if (string.IsNullOrEmpty(sheet.GetCell("B" + (i + 2)).Value.ToString()))
                {
                    userInfo.ExpireDateTime = null;
                }
                else
                {
                    userInfo.ExpireDateTime = Convert.ToDateTime(sheet.GetCell("B" + (i + 2)).Value.ToString() + " 23:59:59");
                }
                if (string.IsNullOrEmpty(sheet.GetCell("C" + (i + 2)).Value.ToString()))
                {
                    userInfo.PhotoExpireDateTime = null;
                }
                else
                {
                    userInfo.PhotoExpireDateTime = Convert.ToDateTime(sheet.GetCell("C" + (i + 2)).Value.ToString() + " 23:59:59");
                }
                list.Add(userInfo);
            }
            return list;
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
                answer.Column10 = sheet.GetCell("N" + (i + 2)).Value == null ? "" : sheet.GetCell("N" + (i + 2)).Value.ToString().Trim();
                answer.Column11 = sheet.GetCell("O" + (i + 2)).Value == null ? "" : sheet.GetCell("O" + (i + 2)).Value.ToString().Trim();
                answer.Column12 = sheet.GetCell("P" + (i + 2)).Value == null ? "" : sheet.GetCell("P" + (i + 2)).Value.ToString().Trim();
                answer.Column13 = sheet.GetCell("Q" + (i + 2)).Value == null ? "" : sheet.GetCell("Q" + (i + 2)).Value.ToString().Trim();
                answer.Column14 = sheet.GetCell("R" + (i + 2)).Value == null ? "" : sheet.GetCell("R" + (i + 2)).Value.ToString().Trim();
                answer.Column15 = sheet.GetCell("S" + (i + 2)).Value == null ? "" : sheet.GetCell("S" + (i + 2)).Value.ToString().Trim();
                answer.Column16 = sheet.GetCell("T" + (i + 2)).Value == null ? "" : sheet.GetCell("T" + (i + 2)).Value.ToString().Trim();
                answer.Column17 = sheet.GetCell("U" + (i + 2)).Value == null ? "" : sheet.GetCell("U" + (i + 2)).Value.ToString().Trim();
                answer.Column18 = sheet.GetCell("V" + (i + 2)).Value == null ? "" : sheet.GetCell("V" + (i + 2)).Value.ToString().Trim();
                answer.Column19 = sheet.GetCell("W" + (i + 2)).Value == null ? "" : sheet.GetCell("W" + (i + 2)).Value.ToString().Trim();
                answer.Column20 = sheet.GetCell("X" + (i + 2)).Value == null ? "" : sheet.GetCell("X" + (i + 2)).Value.ToString().Trim();
                list.Add(answer);
            }
            return list;
        }

        // 任务
        public List<TaskDto> TaskAndAnswer(string ossPath)
        {
            // 从OSS下载文件
            string downLoadFilePath = basePath + @"Content\Excel\ExcelImport\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            OSSClientHelper.GetObject(ossPath, downLoadFilePath);
            Workbook book = Workbook.Load(downLoadFilePath, false);
            #region 任务解析
            Worksheet sheet_task = book.Worksheets[0];
            List<TaskDto> taskList = new List<TaskDto>();
            for (int i = 0; i < 100000; i++)
            {
                string taskCode = sheet_task.GetCell("A" + (i + 2)).Value == null ? "" : sheet_task.GetCell("A" + (i + 2)).Value.ToString().Trim();
                if (string.IsNullOrEmpty(taskCode)) break;
                TaskDto task = new TaskDto();
                // 任务代码
                task.TaskCode = taskCode;
                // 任务名称
                task.TaskName = sheet_task.GetCell("B" + (i + 2)).Value == null ? "" : sheet_task.GetCell("B" + (i + 2)).Value.ToString().Trim();
                // 开始日期
                object startDate = sheet_task.GetCell("C" + (i + 2)).Value;
                if (startDate == null)
                {
                    task.StartDate = null;
                }
                else
                {
                    DateTime startDate_date;
                    if (DateTime.TryParse(startDate.ToString(), out startDate_date))
                    {
                        task.StartDate = Convert.ToDateTime(startDate);
                    }

                }
                // 结束日期
                object endDate = sheet_task.GetCell("D" + (i + 2)).Value;
                if (endDate == null)
                {
                    task.ExpireDateTime = null;
                }
                else
                {
                    DateTime endDate_date;
                    if (DateTime.TryParse(endDate.ToString(), out endDate_date))
                    {
                        task.ExpireDateTime = Convert.ToDateTime(endDate);
                    }

                }
                taskList.Add(task);
            }

            #endregion
           
            return taskList;
        }
        // 导出清单
        public string AnswerExport(string projectId, string shopCode)
        {
            //List<Projects> projectList = masterService.GetProject("","", projectId, "", "", );
            ////string path = "";
            //if (projectList != null && projectList.Count > 0 && projectList[0].ExportRecheck == true)
            //{
            //   return  AnswerExportRecheck(projectId, shopCode);
            //}
            //else {
            //   return  AnswerExportNoRecheck(projectId, shopCode);
            //}
            return "";
        }
        public string AnswerExportRecheck(string projectId, string shopCode)
        {
            List<AnswerDto> list_N = answerService.GetShopAnswerListAll("", projectId, "", "", "", "", "N", shopCode);
            List<AnswerDto> list_Y = answerService.GetShopAnswerListAll("", projectId, "", "", "", "", "Y", shopCode);
            List<AnswerPhotoDto> list_Photo = answerService.GetAnswerPhotoExport("", projectId, shopCode);
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
            List<ExtendColumnProjectDto> ColumnList_add = ColumnList_List.Where(x => x.AddShowChk == true).ToList();

            Workbook book = Workbook.Load(basePath + @"Content\Excel\" + "AnswerExportRecheck.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];

            // 填充表头
            string[] head = { "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB" };
            for (int i = 0; i < ColumnList_List.Count; i++)
            {
                sheet.GetCell(head[i] + 5).Value = ColumnList_List[i].ColumnName;
            }
            int rowIndex = 1;
            foreach (AnswerDto item in list_N)
            {
                sheet.GetCell("A" + (rowIndex + 5)).Value = rowIndex.ToString();
                //通过与否
                if (item.RecheckStatus == true)
                {
                    sheet.GetCell("B" + (rowIndex + 5)).Value = "是";
                }
                else
                {
                    sheet.GetCell("B" + (rowIndex + 5)).Value = "否";
                }

                //经销商代码

                sheet.GetCell("C" + (rowIndex + 5)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("D" + (rowIndex + 5)).Value = item.ShopName;
                //检查代码
                sheet.GetCell("E" + (rowIndex + 5)).Value = item.CheckCode;
                //检查类型
                sheet.GetCell("F" + (rowIndex + 5)).Value = item.CheckTypeName;
                //是否有照片
                if (item.AnswerPhotoList != null && item.AnswerPhotoList.Count > 0)
                {
                    sheet.GetCell("G" + (rowIndex + 5)).Value = "有";
                }
                else
                {
                    sheet.GetCell("G" + (rowIndex + 5)).Value = "无";
                }
                sheet.GetCell("H" + (rowIndex + 5)).Value = item.RemarkName;
                List<ExtendColumnProjectDto> ColumnList_List_Column1 = ColumnList_List.Where(x => x.ColumnCode == "Column1").ToList();
                if (ColumnList_List_Column1 != null && ColumnList_List_Column1.Count > 0 && ColumnList_List_Column1[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column1))
                { sheet.GetCell("I" + (rowIndex + 5)).Value = DateTime.Parse(item.Column1); }
                else
                {
                    sheet.GetCell("I" + (rowIndex + 5)).Value = item.Column1;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column2 = ColumnList_List.Where(x => x.ColumnCode == "Column2").ToList();
                if (ColumnList_List_Column2 != null && ColumnList_List_Column2.Count > 0 && ColumnList_List_Column2[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column2))
                { sheet.GetCell("J" + (rowIndex + 5)).Value = DateTime.Parse(item.Column2); }
                else
                {
                    sheet.GetCell("J" + (rowIndex + 5)).Value = item.Column2;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column3 = ColumnList_List.Where(x => x.ColumnCode == "Column3").ToList();
                if (ColumnList_List_Column3 != null && ColumnList_List_Column3.Count > 0 && ColumnList_List_Column3[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column3))
                { sheet.GetCell("K" + (rowIndex + 5)).Value = DateTime.Parse(item.Column3); }
                else
                {
                    sheet.GetCell("K" + (rowIndex + 5)).Value = item.Column3;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column4 = ColumnList_List.Where(x => x.ColumnCode == "Column4").ToList();
                if (ColumnList_List_Column4 != null && ColumnList_List_Column4.Count > 0 && ColumnList_List_Column4[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column4))
                { sheet.GetCell("L" + (rowIndex + 5)).Value = DateTime.Parse(item.Column4); }
                else
                {
                    sheet.GetCell("L" + (rowIndex + 5)).Value = item.Column4;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column5 = ColumnList_List.Where(x => x.ColumnCode == "Column5").ToList();
                if (ColumnList_List_Column5 != null && ColumnList_List_Column5.Count > 0 && ColumnList_List_Column5[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column5))
                { sheet.GetCell("M" + (rowIndex + 5)).Value = DateTime.Parse(item.Column5); }
                else
                {
                    sheet.GetCell("M" + (rowIndex + 5)).Value = item.Column5;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column6 = ColumnList_List.Where(x => x.ColumnCode == "Column6").ToList();
                if (ColumnList_List_Column6 != null && ColumnList_List_Column6.Count > 0 && ColumnList_List_Column6[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column6))
                { sheet.GetCell("N" + (rowIndex + 5)).Value = DateTime.Parse(item.Column6); }
                else
                {
                    sheet.GetCell("N" + (rowIndex + 5)).Value = item.Column6;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column7 = ColumnList_List.Where(x => x.ColumnCode == "Column7").ToList();
                if (ColumnList_List_Column7 != null && ColumnList_List_Column7.Count > 0 && ColumnList_List_Column7[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column7))
                { sheet.GetCell("O" + (rowIndex + 5)).Value = DateTime.Parse(item.Column7); }
                else
                {
                    sheet.GetCell("O" + (rowIndex + 5)).Value = item.Column7;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column8 = ColumnList_List.Where(x => x.ColumnCode == "Column8").ToList();
                if (ColumnList_List_Column8 != null && ColumnList_List_Column8.Count > 0 && ColumnList_List_Column8[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column8))
                { sheet.GetCell("P" + (rowIndex + 5)).Value = DateTime.Parse(item.Column8); }
                else
                {
                    sheet.GetCell("P" + (rowIndex + 5)).Value = item.Column8;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column9 = ColumnList_List.Where(x => x.ColumnCode == "Column9").ToList();
                if (ColumnList_List_Column9 != null && ColumnList_List_Column9.Count > 0 && ColumnList_List_Column9[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column9))
                { sheet.GetCell("Q" + (rowIndex + 5)).Value = DateTime.Parse(item.Column9); }
                else
                {
                    sheet.GetCell("Q" + (rowIndex + 5)).Value = item.Column9;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column10 = ColumnList_List.Where(x => x.ColumnCode == "Column10").ToList();
                if (ColumnList_List_Column10 != null && ColumnList_List_Column10.Count > 0 && ColumnList_List_Column10[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column10))
                { sheet.GetCell("R" + (rowIndex + 5)).Value = DateTime.Parse(item.Column10); }
                else
                {
                    sheet.GetCell("R" + (rowIndex + 5)).Value = item.Column10;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column11 = ColumnList_List.Where(x => x.ColumnCode == "Column11").ToList();
                if (ColumnList_List_Column11 != null && ColumnList_List_Column11.Count > 0 && ColumnList_List_Column11[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column11))
                { sheet.GetCell("S" + (rowIndex + 5)).Value = DateTime.Parse(item.Column11); }
                else
                {
                    sheet.GetCell("S" + (rowIndex + 5)).Value = item.Column11;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column12 = ColumnList_List.Where(x => x.ColumnCode == "Column12").ToList();
                if (ColumnList_List_Column12 != null && ColumnList_List_Column12.Count > 0 && ColumnList_List_Column12[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column12))
                { sheet.GetCell("T" + (rowIndex + 5)).Value = DateTime.Parse(item.Column12); }
                else
                {
                    sheet.GetCell("T" + (rowIndex + 5)).Value = item.Column12;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column13 = ColumnList_List.Where(x => x.ColumnCode == "Column13").ToList();
                if (ColumnList_List_Column13 != null && ColumnList_List_Column13.Count > 0 && ColumnList_List_Column13[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column13))
                { sheet.GetCell("U" + (rowIndex + 5)).Value = DateTime.Parse(item.Column13); }
                else
                {
                    sheet.GetCell("U" + (rowIndex + 5)).Value = item.Column13;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column14 = ColumnList_List.Where(x => x.ColumnCode == "Column14").ToList();
                if (ColumnList_List_Column14 != null && ColumnList_List_Column14.Count > 0 && ColumnList_List_Column14[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column14))
                { sheet.GetCell("V" + (rowIndex + 5)).Value = DateTime.Parse(item.Column14); }
                else
                {
                    sheet.GetCell("V" + (rowIndex + 5)).Value = item.Column14;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column15 = ColumnList_List.Where(x => x.ColumnCode == "Column15").ToList();
                if (ColumnList_List_Column15 != null && ColumnList_List_Column15.Count > 0 && ColumnList_List_Column15[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column15))
                {
                    sheet.GetCell("W" + (rowIndex + 5)).Value = DateTime.Parse(item.Column15);
                }
                else
                {
                    sheet.GetCell("W" + (rowIndex + 5)).Value = item.Column15;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column16 = ColumnList_List.Where(x => x.ColumnCode == "Column16").ToList();
                if (ColumnList_List_Column16 != null && ColumnList_List_Column16.Count > 0 && ColumnList_List_Column16[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column16))
                {
                    sheet.GetCell("X" + (rowIndex + 5)).Value = DateTime.Parse(item.Column16);
                }
                else
                {
                    sheet.GetCell("X" + (rowIndex + 5)).Value = item.Column16;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column17 = ColumnList_List.Where(x => x.ColumnCode == "Column17").ToList();
                if (ColumnList_List_Column17 != null && ColumnList_List_Column17.Count > 0 && ColumnList_List_Column17[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column17))
                {
                    sheet.GetCell("Y" + (rowIndex + 5)).Value = DateTime.Parse(item.Column17);
                }
                else
                {
                    sheet.GetCell("Y" + (rowIndex + 5)).Value = item.Column17;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column18 = ColumnList_List.Where(x => x.ColumnCode == "Column18").ToList();
                if (ColumnList_List_Column18 != null && ColumnList_List_Column18.Count > 0 && ColumnList_List_Column18[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column18))
                {
                    sheet.GetCell("Z" + (rowIndex + 5)).Value = DateTime.Parse(item.Column18);
                }
                else
                {
                    sheet.GetCell("Z" + (rowIndex + 5)).Value = item.Column18;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column19 = ColumnList_List.Where(x => x.ColumnCode == "Column19").ToList();
                if (ColumnList_List_Column19 != null && ColumnList_List_Column19.Count > 0 && ColumnList_List_Column19[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column19))
                {
                    sheet.GetCell("AA" + (rowIndex + 5)).Value = DateTime.Parse(item.Column19);
                }
                else
                {
                    sheet.GetCell("AA" + (rowIndex + 5)).Value = item.Column19;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column20 = ColumnList_List.Where(x => x.ColumnCode == "Column20").ToList();
                if (ColumnList_List_Column20 != null && ColumnList_List_Column20.Count > 0 && ColumnList_List_Column20[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column20))
                {
                    sheet.GetCell("AB" + (rowIndex + 5)).Value = DateTime.Parse(item.Column20);
                }
                else
                {
                    sheet.GetCell("AB" + (rowIndex + 5)).Value = item.Column20;
                }
                rowIndex++;
            }
            //填充数据
            Worksheet sheet1 = book.Worksheets[1];
            // 填充表头
            string[] head1 = { "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA" };
            for (int i = 0; i < ColumnList_add.Count; i++)
            {
                sheet1.GetCell(head1[i] + 5).Value = ColumnList_add[i].ColumnName;
            }
            int rowIndex1 = 1;
            foreach (AnswerDto item in list_Y)
            {
                sheet1.GetCell("A" + (rowIndex1 + 5)).Value = rowIndex1.ToString();
                //通过与否
                if (item.RecheckStatus == true)
                {
                    sheet1.GetCell("B" + (rowIndex1 + 5)).Value = "是";
                }
                else
                {
                    sheet1.GetCell("B" + (rowIndex1 + 5)).Value = "否";
                }
                //经销商代码
                sheet1.GetCell("C" + (rowIndex1 + 5)).Value = item.ShopCode;
                //经销商名称
                sheet1.GetCell("D" + (rowIndex1 + 5)).Value = item.ShopName;
                //检查代码
                sheet1.GetCell("E" + (rowIndex1 + 5)).Value = item.CheckCode;
                //是否有照片
                if (item.AnswerPhotoList != null && item.AnswerPhotoList.Count > 0)
                {
                    sheet1.GetCell("F" + (rowIndex1 + 5)).Value = "有";
                }
                else
                {
                    sheet1.GetCell("F" + (rowIndex1 + 5)).Value = "无";
                }
                sheet1.GetCell("G" + (rowIndex1 + 5)).Value = item.RemarkName;
                List<ExtendColumnProjectDto> ColumnList_List_Column1 = ColumnList_List.Where(x => x.ColumnCode == "Column1").ToList();
                if (ColumnList_List_Column1 != null && ColumnList_List_Column1.Count > 0 && ColumnList_List_Column1[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column1))
                { sheet1.GetCell("H" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column1); }
                else
                {
                    sheet1.GetCell("H" + (rowIndex1 + 5)).Value = item.Column1;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column2 = ColumnList_List.Where(x => x.ColumnCode == "Column2").ToList();
                if (ColumnList_List_Column2 != null && ColumnList_List_Column2.Count > 0 && ColumnList_List_Column2[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column2))
                { sheet1.GetCell("I" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column2); }
                else
                {
                    sheet1.GetCell("I" + (rowIndex1 + 5)).Value = item.Column2;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column3 = ColumnList_List.Where(x => x.ColumnCode == "Column3").ToList();
                if (ColumnList_List_Column3 != null && ColumnList_List_Column3.Count > 0 && ColumnList_List_Column3[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column3))
                { sheet1.GetCell("J" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column3); }
                else
                {
                    sheet1.GetCell("J" + (rowIndex1 + 5)).Value = item.Column3;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column4 = ColumnList_List.Where(x => x.ColumnCode == "Column4").ToList();
                if (ColumnList_List_Column4 != null && ColumnList_List_Column4.Count > 0 && ColumnList_List_Column4[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column4))
                { sheet1.GetCell("K" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column4); }
                else
                {
                    sheet1.GetCell("K" + (rowIndex1 + 5)).Value = item.Column4;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column5 = ColumnList_List.Where(x => x.ColumnCode == "Column5").ToList();
                if (ColumnList_List_Column5 != null && ColumnList_List_Column5.Count > 0 && ColumnList_List_Column5[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column5))
                { sheet1.GetCell("L" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column5); }
                else
                {
                    sheet1.GetCell("L" + (rowIndex1 + 5)).Value = item.Column5;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column6 = ColumnList_List.Where(x => x.ColumnCode == "Column6").ToList();
                if (ColumnList_List_Column6 != null && ColumnList_List_Column6.Count > 0 && ColumnList_List_Column6[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column6))
                { sheet1.GetCell("M" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column6); }
                else
                {
                    sheet1.GetCell("M" + (rowIndex1 + 5)).Value = item.Column6;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column7 = ColumnList_List.Where(x => x.ColumnCode == "Column7").ToList();
                if (ColumnList_List_Column7 != null && ColumnList_List_Column7.Count > 0 && ColumnList_List_Column7[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column7))
                { sheet1.GetCell("N" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column7); }
                else
                {
                    sheet1.GetCell("N" + (rowIndex1 + 5)).Value = item.Column7;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column8 = ColumnList_List.Where(x => x.ColumnCode == "Column8").ToList();
                if (ColumnList_List_Column8 != null && ColumnList_List_Column8.Count > 0 && ColumnList_List_Column8[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column8))
                { sheet1.GetCell("O" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column8); }
                else
                {
                    sheet1.GetCell("O" + (rowIndex1 + 5)).Value = item.Column8;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column9 = ColumnList_List.Where(x => x.ColumnCode == "Column9").ToList();
                if (ColumnList_List_Column9 != null && ColumnList_List_Column9.Count > 0 && ColumnList_List_Column9[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column9))
                { sheet1.GetCell("P" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column9); }
                else
                {
                    sheet1.GetCell("P" + (rowIndex1 + 5)).Value = item.Column9;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column10 = ColumnList_List.Where(x => x.ColumnCode == "Column10").ToList();
                if (ColumnList_List_Column10 != null && ColumnList_List_Column10.Count > 0 && ColumnList_List_Column10[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column10))
                { sheet1.GetCell("Q" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column10); }
                else
                {
                    sheet1.GetCell("Q" + (rowIndex1 + 5)).Value = item.Column10;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column11 = ColumnList_List.Where(x => x.ColumnCode == "Column11").ToList();
                if (ColumnList_List_Column11 != null && ColumnList_List_Column11.Count > 0 && ColumnList_List_Column11[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column11))
                { sheet1.GetCell("R" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column11); }
                else
                {
                    sheet1.GetCell("R" + (rowIndex1 + 5)).Value = item.Column11;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column12 = ColumnList_List.Where(x => x.ColumnCode == "Column12").ToList();
                if (ColumnList_List_Column12 != null && ColumnList_List_Column12.Count > 0 && ColumnList_List_Column12[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column12))
                { sheet1.GetCell("S" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column12); }
                else
                {
                    sheet1.GetCell("S" + (rowIndex1 + 5)).Value = item.Column12;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column13 = ColumnList_List.Where(x => x.ColumnCode == "Column13").ToList();
                if (ColumnList_List_Column13 != null && ColumnList_List_Column13.Count > 0 && ColumnList_List_Column13[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column13))
                { sheet1.GetCell("T" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column13); }
                else
                {
                    sheet1.GetCell("T" + (rowIndex1 + 5)).Value = item.Column13;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column14 = ColumnList_List.Where(x => x.ColumnCode == "Column14").ToList();
                if (ColumnList_List_Column14 != null && ColumnList_List_Column14.Count > 0 && ColumnList_List_Column14[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column14))
                { sheet1.GetCell("U" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column14); }
                else
                {
                    sheet1.GetCell("U" + (rowIndex1 + 5)).Value = item.Column14;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column15 = ColumnList_List.Where(x => x.ColumnCode == "Column15").ToList();
                if (ColumnList_List_Column15 != null && ColumnList_List_Column15.Count > 0 && ColumnList_List_Column15[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column15))
                {
                    sheet1.GetCell("V" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column15);
                }
                else
                {
                    sheet1.GetCell("V" + (rowIndex1 + 5)).Value = item.Column15;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column16 = ColumnList_List.Where(x => x.ColumnCode == "Column16").ToList();
                if (ColumnList_List_Column16 != null && ColumnList_List_Column16.Count > 0 && ColumnList_List_Column16[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column16))
                {
                    sheet1.GetCell("W" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column16);
                }
                else
                {
                    sheet1.GetCell("W" + (rowIndex1 + 5)).Value = item.Column16;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column17 = ColumnList_List.Where(x => x.ColumnCode == "Column17").ToList();
                if (ColumnList_List_Column17 != null && ColumnList_List_Column17.Count > 0 && ColumnList_List_Column17[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column17))
                {
                    sheet1.GetCell("X" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column17);
                }
                else
                {
                    sheet1.GetCell("X" + (rowIndex1 + 5)).Value = item.Column17;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column18 = ColumnList_List.Where(x => x.ColumnCode == "Column18").ToList();
                if (ColumnList_List_Column18 != null && ColumnList_List_Column18.Count > 0 && ColumnList_List_Column18[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column18))
                {
                    sheet1.GetCell("Y" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column18);
                }
                else
                {
                    sheet1.GetCell("Y" + (rowIndex1 + 5)).Value = item.Column18;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column19 = ColumnList_List.Where(x => x.ColumnCode == "Column19").ToList();
                if (ColumnList_List_Column19 != null && ColumnList_List_Column19.Count > 0 && ColumnList_List_Column19[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column19))
                {
                    sheet1.GetCell("Z" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column19);
                }
                else
                {
                    sheet1.GetCell("Z" + (rowIndex1 + 5)).Value = item.Column19;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column20 = ColumnList_List.Where(x => x.ColumnCode == "Column20").ToList();
                if (ColumnList_List_Column20 != null && ColumnList_List_Column20.Count > 0 && ColumnList_List_Column20[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column20))
                {
                    sheet1.GetCell("AA" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column20);
                }
                else
                {
                    sheet1.GetCell("AA" + (rowIndex1 + 5)).Value = item.Column20;
                }
                rowIndex1++;
            }
            //填充数据
            Worksheet sheet2 = book.Worksheets[2];
            int rowIndex2 = 1;
            foreach (AnswerPhotoDto item in list_Photo)
            {
                sheet2.GetCell("A" + (rowIndex2 + 5)).Value = rowIndex2.ToString();
                //经销商代码
                sheet2.GetCell("B" + (rowIndex2 + 5)).Value = item.ShopCode;
                //经销商名称
                sheet2.GetCell("C" + (rowIndex2 + 5)).Value = item.ShopName;
                //检查代码
                sheet2.GetCell("D" + (rowIndex2 + 5)).Value = item.CheckCode;
                //检查类型
                sheet2.GetCell("E" + (rowIndex2 + 5)).Value = item.CheckTypeName;
                //是否新增
                sheet2.GetCell("F" + (rowIndex2 + 5)).Value = item.AddCheck;
                //是否新增
                sheet2.GetCell("G" + (rowIndex2 + 5)).Value = item.PhotoName;
                //是否有照片
                sheet2.GetCell("H" + (rowIndex2 + 5)).Value = item.Photo;
                rowIndex2++;
            }
            //保存excel文件
            string shopName = "经销商导出清单";
            List<UserInfo> userInfoList = masterService.GetUserInfo(projectId, shopCode, shopCode);
            if (userInfoList != null && userInfoList.Count == 1)
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
        public string AnswerExportNoRecheck(string projectId, string shopCode)
        {
            List<AnswerDto> list_N = answerService.GetShopAnswerListAll("", projectId, "", "", "", "", "N", shopCode);
            List<AnswerDto> list_Y = answerService.GetShopAnswerListAll("", projectId, "", "", "", "", "Y", shopCode);
            List<AnswerPhotoDto> list_Photo = answerService.GetAnswerPhotoExport("", projectId, shopCode);
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
            List<ExtendColumnProjectDto> ColumnList_add = ColumnList_List.Where(x => x.AddShowChk == true).ToList();

            Workbook book = Workbook.Load(basePath + @"Content\Excel\" + "AnswerExport.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];

            // 填充表头
            string[] head = { "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA" };
            for (int i = 0; i < ColumnList_List.Count; i++)
            {
                sheet.GetCell(head[i] + 5).Value = ColumnList_List[i].ColumnName;
            }
            int rowIndex = 1;
            foreach (AnswerDto item in list_N)
            {
                sheet.GetCell("A" + (rowIndex + 5)).Value = rowIndex.ToString();
                ////通过与否
                //if (item.RecheckStatus == true)
                //{
                //    sheet.GetCell("B" + (rowIndex + 5)).Value = "是";
                //}
                //else {
                //    sheet.GetCell("B" + (rowIndex + 5)).Value = "否";
                //}

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
                List<ExtendColumnProjectDto> ColumnList_List_Column1 = ColumnList_List.Where(x => x.ColumnCode == "Column1").ToList();
                if (ColumnList_List_Column1 != null && ColumnList_List_Column1.Count > 0 && ColumnList_List_Column1[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column1))
                { sheet.GetCell("H" + (rowIndex + 5)).Value = DateTime.Parse(item.Column1); }
                else
                {
                    sheet.GetCell("H" + (rowIndex + 5)).Value = item.Column1;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column2 = ColumnList_List.Where(x => x.ColumnCode == "Column2").ToList();
                if (ColumnList_List_Column2 != null && ColumnList_List_Column2.Count > 0 && ColumnList_List_Column2[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column2))
                { sheet.GetCell("I" + (rowIndex + 5)).Value = DateTime.Parse(item.Column2); }
                else
                {
                    sheet.GetCell("I" + (rowIndex + 5)).Value = item.Column2;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column3 = ColumnList_List.Where(x => x.ColumnCode == "Column3").ToList();
                if (ColumnList_List_Column3 != null && ColumnList_List_Column3.Count > 0 && ColumnList_List_Column3[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column3))
                { sheet.GetCell("J" + (rowIndex + 5)).Value = DateTime.Parse(item.Column3); }
                else
                {
                    sheet.GetCell("J" + (rowIndex + 5)).Value = item.Column3;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column4 = ColumnList_List.Where(x => x.ColumnCode == "Column4").ToList();
                if (ColumnList_List_Column4 != null && ColumnList_List_Column4.Count > 0 && ColumnList_List_Column4[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column4))
                { sheet.GetCell("K" + (rowIndex + 5)).Value = DateTime.Parse(item.Column4); }
                else
                {
                    sheet.GetCell("K" + (rowIndex + 5)).Value = item.Column4;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column5 = ColumnList_List.Where(x => x.ColumnCode == "Column5").ToList();
                if (ColumnList_List_Column5 != null && ColumnList_List_Column5.Count > 0 && ColumnList_List_Column5[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column5))
                { sheet.GetCell("L" + (rowIndex + 5)).Value = DateTime.Parse(item.Column5); }
                else
                {
                    sheet.GetCell("L" + (rowIndex + 5)).Value = item.Column5;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column6 = ColumnList_List.Where(x => x.ColumnCode == "Column6").ToList();
                if (ColumnList_List_Column6 != null && ColumnList_List_Column6.Count > 0 && ColumnList_List_Column6[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column6))
                { sheet.GetCell("M" + (rowIndex + 5)).Value = DateTime.Parse(item.Column6); }
                else
                {
                    sheet.GetCell("M" + (rowIndex + 5)).Value = item.Column6;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column7 = ColumnList_List.Where(x => x.ColumnCode == "Column7").ToList();
                if (ColumnList_List_Column7 != null && ColumnList_List_Column7.Count > 0 && ColumnList_List_Column7[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column7))
                { sheet.GetCell("N" + (rowIndex + 5)).Value = DateTime.Parse(item.Column7); }
                else
                {
                    sheet.GetCell("N" + (rowIndex + 5)).Value = item.Column7;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column8 = ColumnList_List.Where(x => x.ColumnCode == "Column8").ToList();
                if (ColumnList_List_Column8 != null && ColumnList_List_Column8.Count > 0 && ColumnList_List_Column8[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column8))
                { sheet.GetCell("O" + (rowIndex + 5)).Value = DateTime.Parse(item.Column8); }
                else
                {
                    sheet.GetCell("O" + (rowIndex + 5)).Value = item.Column8;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column9 = ColumnList_List.Where(x => x.ColumnCode == "Column9").ToList();
                if (ColumnList_List_Column9 != null && ColumnList_List_Column9.Count > 0 && ColumnList_List_Column9[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column9))
                { sheet.GetCell("P" + (rowIndex + 5)).Value = DateTime.Parse(item.Column9); }
                else
                {
                    sheet.GetCell("P" + (rowIndex + 5)).Value = item.Column9;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column10 = ColumnList_List.Where(x => x.ColumnCode == "Column10").ToList();
                if (ColumnList_List_Column10 != null && ColumnList_List_Column10.Count > 0 && ColumnList_List_Column10[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column10))
                { sheet.GetCell("Q" + (rowIndex + 5)).Value = DateTime.Parse(item.Column10); }
                else
                {
                    sheet.GetCell("Q" + (rowIndex + 5)).Value = item.Column10;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column11 = ColumnList_List.Where(x => x.ColumnCode == "Column11").ToList();
                if (ColumnList_List_Column11 != null && ColumnList_List_Column11.Count > 0 && ColumnList_List_Column11[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column11))
                { sheet.GetCell("R" + (rowIndex + 5)).Value = DateTime.Parse(item.Column11); }
                else
                {
                    sheet.GetCell("R" + (rowIndex + 5)).Value = item.Column11;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column12 = ColumnList_List.Where(x => x.ColumnCode == "Column12").ToList();
                if (ColumnList_List_Column12 != null && ColumnList_List_Column12.Count > 0 && ColumnList_List_Column12[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column12))
                { sheet.GetCell("S" + (rowIndex + 5)).Value = DateTime.Parse(item.Column12); }
                else
                {
                    sheet.GetCell("S" + (rowIndex + 5)).Value = item.Column12;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column13 = ColumnList_List.Where(x => x.ColumnCode == "Column13").ToList();
                if (ColumnList_List_Column13 != null && ColumnList_List_Column13.Count > 0 && ColumnList_List_Column13[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column13))
                { sheet.GetCell("T" + (rowIndex + 5)).Value = DateTime.Parse(item.Column13); }
                else
                {
                    sheet.GetCell("T" + (rowIndex + 5)).Value = item.Column13;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column14 = ColumnList_List.Where(x => x.ColumnCode == "Column14").ToList();
                if (ColumnList_List_Column14 != null && ColumnList_List_Column14.Count > 0 && ColumnList_List_Column14[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column14))
                { sheet.GetCell("U" + (rowIndex + 5)).Value = DateTime.Parse(item.Column14); }
                else
                {
                    sheet.GetCell("U" + (rowIndex + 5)).Value = item.Column14;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column15 = ColumnList_List.Where(x => x.ColumnCode == "Column15").ToList();
                if (ColumnList_List_Column15 != null && ColumnList_List_Column15.Count > 0 && ColumnList_List_Column15[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column15))
                {
                    sheet.GetCell("V" + (rowIndex + 5)).Value = DateTime.Parse(item.Column15);
                }
                else
                {
                    sheet.GetCell("V" + (rowIndex + 5)).Value = item.Column15;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column16 = ColumnList_List.Where(x => x.ColumnCode == "Column16").ToList();
                if (ColumnList_List_Column16 != null && ColumnList_List_Column16.Count > 0 && ColumnList_List_Column16[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column16))
                {
                    sheet.GetCell("W" + (rowIndex + 5)).Value = DateTime.Parse(item.Column16);
                }
                else
                {
                    sheet.GetCell("W" + (rowIndex + 5)).Value = item.Column16;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column17 = ColumnList_List.Where(x => x.ColumnCode == "Column17").ToList();
                if (ColumnList_List_Column17 != null && ColumnList_List_Column17.Count > 0 && ColumnList_List_Column17[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column17))
                {
                    sheet.GetCell("X" + (rowIndex + 5)).Value = DateTime.Parse(item.Column17);
                }
                else
                {
                    sheet.GetCell("X" + (rowIndex + 5)).Value = item.Column17;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column18 = ColumnList_List.Where(x => x.ColumnCode == "Column18").ToList();
                if (ColumnList_List_Column18 != null && ColumnList_List_Column18.Count > 0 && ColumnList_List_Column18[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column18))
                {
                    sheet.GetCell("Y" + (rowIndex + 5)).Value = DateTime.Parse(item.Column18);
                }
                else
                {
                    sheet.GetCell("Y" + (rowIndex + 5)).Value = item.Column18;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column19 = ColumnList_List.Where(x => x.ColumnCode == "Column19").ToList();
                if (ColumnList_List_Column19 != null && ColumnList_List_Column19.Count > 0 && ColumnList_List_Column19[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column19))
                {
                    sheet.GetCell("Z" + (rowIndex + 5)).Value = DateTime.Parse(item.Column19);
                }
                else
                {
                    sheet.GetCell("Z" + (rowIndex + 5)).Value = item.Column19;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column20 = ColumnList_List.Where(x => x.ColumnCode == "Column20").ToList();
                if (ColumnList_List_Column20 != null && ColumnList_List_Column20.Count > 0 && ColumnList_List_Column20[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column20))
                {
                    sheet.GetCell("AA" + (rowIndex + 5)).Value = DateTime.Parse(item.Column20);
                }
                else
                {
                    sheet.GetCell("AA" + (rowIndex + 5)).Value = item.Column20;
                }
                rowIndex++;
            }
            //填充数据
            Worksheet sheet1 = book.Worksheets[1];
            // 填充表头
            string[] head1 = { "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            for (int i = 0; i < ColumnList_add.Count; i++)
            {
                sheet1.GetCell(head1[i] + 5).Value = ColumnList_add[i].ColumnName;
            }
            int rowIndex1 = 1;
            foreach (AnswerDto item in list_Y)
            {
                sheet1.GetCell("A" + (rowIndex1 + 5)).Value = rowIndex1.ToString();
                ////通过与否
                //if (item.RecheckStatus == true)
                //{
                //    sheet1.GetCell("B" + (rowIndex1 + 5)).Value = "是";
                //}
                //else
                //{
                //    sheet1.GetCell("B" + (rowIndex1 + 5)).Value = "否";
                //}
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
                List<ExtendColumnProjectDto> ColumnList_List_Column1 = ColumnList_List.Where(x => x.ColumnCode == "Column1").ToList();
                if (ColumnList_List_Column1 != null && ColumnList_List_Column1.Count > 0 && ColumnList_List_Column1[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column1))
                { sheet1.GetCell("G" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column1); }
                else
                {
                    sheet1.GetCell("G" + (rowIndex1 + 5)).Value = item.Column1;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column2 = ColumnList_List.Where(x => x.ColumnCode == "Column2").ToList();
                if (ColumnList_List_Column2 != null && ColumnList_List_Column2.Count > 0 && ColumnList_List_Column2[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column2))
                { sheet1.GetCell("H" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column2); }
                else
                {
                    sheet1.GetCell("H" + (rowIndex1 + 5)).Value = item.Column2;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column3 = ColumnList_List.Where(x => x.ColumnCode == "Column3").ToList();
                if (ColumnList_List_Column3 != null && ColumnList_List_Column3.Count > 0 && ColumnList_List_Column3[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column3))
                { sheet1.GetCell("I" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column3); }
                else
                {
                    sheet1.GetCell("I" + (rowIndex1 + 5)).Value = item.Column3;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column4 = ColumnList_List.Where(x => x.ColumnCode == "Column4").ToList();
                if (ColumnList_List_Column4 != null && ColumnList_List_Column4.Count > 0 && ColumnList_List_Column4[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column4))
                { sheet1.GetCell("J" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column4); }
                else
                {
                    sheet1.GetCell("J" + (rowIndex1 + 5)).Value = item.Column4;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column5 = ColumnList_List.Where(x => x.ColumnCode == "Column5").ToList();
                if (ColumnList_List_Column5 != null && ColumnList_List_Column5.Count > 0 && ColumnList_List_Column5[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column5))
                { sheet1.GetCell("K" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column5); }
                else
                {
                    sheet1.GetCell("K" + (rowIndex1 + 5)).Value = item.Column5;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column6 = ColumnList_List.Where(x => x.ColumnCode == "Column6").ToList();
                if (ColumnList_List_Column6 != null && ColumnList_List_Column6.Count > 0 && ColumnList_List_Column6[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column6))
                { sheet1.GetCell("L" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column6); }
                else
                {
                    sheet1.GetCell("L" + (rowIndex1 + 5)).Value = item.Column6;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column7 = ColumnList_List.Where(x => x.ColumnCode == "Column7").ToList();
                if (ColumnList_List_Column7 != null && ColumnList_List_Column7.Count > 0 && ColumnList_List_Column7[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column7))
                { sheet1.GetCell("M" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column7); }
                else
                {
                    sheet1.GetCell("M" + (rowIndex1 + 5)).Value = item.Column7;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column8 = ColumnList_List.Where(x => x.ColumnCode == "Column8").ToList();
                if (ColumnList_List_Column8 != null && ColumnList_List_Column8.Count > 0 && ColumnList_List_Column8[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column8))
                { sheet1.GetCell("N" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column8); }
                else
                {
                    sheet1.GetCell("N" + (rowIndex1 + 5)).Value = item.Column8;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column9 = ColumnList_List.Where(x => x.ColumnCode == "Column9").ToList();
                if (ColumnList_List_Column9 != null && ColumnList_List_Column9.Count > 0 && ColumnList_List_Column9[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column9))
                { sheet1.GetCell("O" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column9); }
                else
                {
                    sheet1.GetCell("O" + (rowIndex1 + 5)).Value = item.Column9;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column10 = ColumnList_List.Where(x => x.ColumnCode == "Column10").ToList();
                if (ColumnList_List_Column10 != null && ColumnList_List_Column10.Count > 0 && ColumnList_List_Column10[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column10))
                { sheet1.GetCell("P" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column10); }
                else
                {
                    sheet1.GetCell("P" + (rowIndex1 + 5)).Value = item.Column10;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column11 = ColumnList_List.Where(x => x.ColumnCode == "Column11").ToList();
                if (ColumnList_List_Column11 != null && ColumnList_List_Column11.Count > 0 && ColumnList_List_Column11[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column11))
                { sheet1.GetCell("Q" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column11); }
                else
                {
                    sheet1.GetCell("Q" + (rowIndex1 + 5)).Value = item.Column11;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column12 = ColumnList_List.Where(x => x.ColumnCode == "Column12").ToList();
                if (ColumnList_List_Column12 != null && ColumnList_List_Column12.Count > 0 && ColumnList_List_Column12[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column12))
                { sheet1.GetCell("R" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column12); }
                else
                {
                    sheet1.GetCell("R" + (rowIndex1 + 5)).Value = item.Column12;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column13 = ColumnList_List.Where(x => x.ColumnCode == "Column13").ToList();
                if (ColumnList_List_Column13 != null && ColumnList_List_Column13.Count > 0 && ColumnList_List_Column13[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column13))
                { sheet1.GetCell("S" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column13); }
                else
                {
                    sheet1.GetCell("S" + (rowIndex1 + 5)).Value = item.Column13;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column14 = ColumnList_List.Where(x => x.ColumnCode == "Column14").ToList();
                if (ColumnList_List_Column14 != null && ColumnList_List_Column14.Count > 0 && ColumnList_List_Column14[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column14))
                { sheet1.GetCell("T" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column14); }
                else
                {
                    sheet1.GetCell("T" + (rowIndex1 + 5)).Value = item.Column14;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column15 = ColumnList_List.Where(x => x.ColumnCode == "Column15").ToList();
                if (ColumnList_List_Column15 != null && ColumnList_List_Column15.Count > 0 && ColumnList_List_Column15[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column15))
                {
                    sheet1.GetCell("U" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column15);
                }
                else
                {
                    sheet1.GetCell("U" + (rowIndex1 + 5)).Value = item.Column15;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column16 = ColumnList_List.Where(x => x.ColumnCode == "Column16").ToList();
                if (ColumnList_List_Column16 != null && ColumnList_List_Column16.Count > 0 && ColumnList_List_Column16[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column16))
                {
                    sheet1.GetCell("V" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column16);
                }
                else
                {
                    sheet1.GetCell("V" + (rowIndex1 + 5)).Value = item.Column16;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column17 = ColumnList_List.Where(x => x.ColumnCode == "Column17").ToList();
                if (ColumnList_List_Column17 != null && ColumnList_List_Column17.Count > 0 && ColumnList_List_Column17[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column17))
                {
                    sheet1.GetCell("W" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column17);
                }
                else
                {
                    sheet1.GetCell("W" + (rowIndex1 + 5)).Value = item.Column17;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column18 = ColumnList_List.Where(x => x.ColumnCode == "Column18").ToList();
                if (ColumnList_List_Column18 != null && ColumnList_List_Column18.Count > 0 && ColumnList_List_Column18[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column18))
                {
                    sheet1.GetCell("X" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column18);
                }
                else
                {
                    sheet1.GetCell("X" + (rowIndex1 + 5)).Value = item.Column18;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column19 = ColumnList_List.Where(x => x.ColumnCode == "Column19").ToList();
                if (ColumnList_List_Column19 != null && ColumnList_List_Column19.Count > 0 && ColumnList_List_Column19[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column19))
                {
                    sheet1.GetCell("Y" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column19);
                }
                else
                {
                    sheet1.GetCell("Y" + (rowIndex1 + 5)).Value = item.Column19;
                }
                List<ExtendColumnProjectDto> ColumnList_List_Column20 = ColumnList_List.Where(x => x.ColumnCode == "Column20").ToList();
                if (ColumnList_List_Column20 != null && ColumnList_List_Column20.Count > 0 && ColumnList_List_Column20[0].ColumnType == "2" && !string.IsNullOrEmpty(item.Column20))
                {
                    sheet1.GetCell("Z" + (rowIndex1 + 5)).Value = DateTime.Parse(item.Column20);
                }
                else
                {
                    sheet1.GetCell("Z" + (rowIndex1 + 5)).Value = item.Column20;
                }
                rowIndex1++;
            }
            //填充数据
            Worksheet sheet2 = book.Worksheets[2];
            int rowIndex2 = 1;
            foreach (AnswerPhotoDto item in list_Photo)
            {
                sheet2.GetCell("A" + (rowIndex2 + 5)).Value = rowIndex2.ToString();
                //经销商代码
                sheet2.GetCell("B" + (rowIndex2 + 5)).Value = item.ShopCode;
                //经销商名称
                sheet2.GetCell("C" + (rowIndex2 + 5)).Value = item.ShopName;
                //检查代码
                sheet2.GetCell("D" + (rowIndex2 + 5)).Value = item.CheckCode;
                //检查类型
                sheet2.GetCell("E" + (rowIndex2 + 5)).Value = item.CheckTypeName;
                //是否新增
                sheet2.GetCell("F" + (rowIndex2 + 5)).Value = item.AddCheck;
                //照片名称
                sheet2.GetCell("G" + (rowIndex2 + 5)).Value = item.PhotoName;
                //是否有照片
                sheet2.GetCell("H" + (rowIndex2 + 5)).Value = item.Photo;
                rowIndex2++;
            }
            //保存excel文件
            string shopName = "经销商导出清单";
            List<UserInfo> userInfoList = masterService.GetUserInfo(projectId, shopCode, shopCode);
            if (userInfoList != null && userInfoList.Count == 1)
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

        // 导出进店状态
        public string AnswerShopInfoExport(string projectId, string key)
        {
            List<AnswerShopInfoDto> list = answerService.GetAnswerShopInfo(projectId, key);
            Workbook book = Workbook.Load(basePath + @"Content\Excel\" + "AnswerShopInfoExport.xlsx", false);
            //填充数据
            Worksheet sheet = book.Worksheets[0];
            int rowIndex = 1;

            foreach (AnswerShopInfoDto item in list)
            {
                //期号代码
                sheet.GetCell("A" + (rowIndex + 1)).Value = item.ProjectCode;
                //期号名称
                sheet.GetCell("B" + (rowIndex + 1)).Value = item.ProjectName;
                //经销商代码
                sheet.GetCell("C" + (rowIndex + 1)).Value = item.ShopCode;
                //经销商名称
                sheet.GetCell("D" + (rowIndex + 1)).Value = item.ShopName;
                //进店状态
                sheet.GetCell("E" + (rowIndex + 1)).Value = item.ShopInStatus;
                //进店日期
                if (item.ShopInDateTime == null)
                {
                    sheet.GetCell("F" + (rowIndex + 1)).Value = "";
                }
                else
                {
                    sheet.GetCell("F" + (rowIndex + 1)).Value = item.ShopInDateTime.ToString();
                }
                //离店日期
                if (item.ShopOutDateTime == null)
                {
                    sheet.GetCell("G" + (rowIndex + 1)).Value = "";
                }
                else
                {
                    sheet.GetCell("G" + (rowIndex + 1)).Value = item.ShopOutDateTime.ToString();
                }

                rowIndex++;
            }

            //保存excel文件
            string fileName = "进店状态" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
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