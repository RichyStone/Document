using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Semight.Fwm.Common.CommonFileOperationLib.Word
{
    public static class WordHelper
    {
        /// <summary>
        /// 段落设定字典
        /// </summary>
        private static Dictionary<string, ContentSetting> ContentSettings { get; set; }

        /// <summary>
        /// 添加段落设定
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddContentSetting(string key, ContentSetting value)
        {
            if (ContentSettings == null)
                ContentSettings = new Dictionary<string, ContentSetting>();

            if (ContentSettings.ContainsKey(key))
                ContentSettings.Remove(key);

            ContentSettings.Add(key, value);
        }

        /// <summary>
        /// 获取段落设定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ContentSetting GetContentSetting(string key)
        {
            if (ContentSettings == null)
                throw new Exception("ContentSetting Dictionary is Null");

            if (ContentSettings.ContainsKey(key))
                return ContentSettings[key];
            else
                throw new Exception($"ContentSetting Dictionary doesn't Have Specified Key：{key}");
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="document"></param>
        /// <param name="imgPath"></param>
        /// <param name="pictureType"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static XWPFParagraph InsertImg(XWPFDocument document, string imgPath, int width, int height, PictureType pictureType = PictureType.JPEG, ParagraphAlignment alignment = ParagraphAlignment.CENTER)
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = alignment;

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
                                                    //标题图片
            FileStream gfs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            xwpfRun.AddPicture(gfs, (int)pictureType, Path.GetFileName(imgPath), width * 9525, height * 9525);
            gfs.Close();

            return paragraph;
        }

        /// <summary>
        /// 创建word文档中的段落对象并设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="document">document文档对象</param>
        /// <param name="fillContent">段落第一个文本对象填充的内容</param>
        /// <param name="contentItemSetting">内容设定</param>
        /// <returns></returns>
        public static XWPFParagraph InsertParagraph(XWPFDocument document, string fillContent, ContentSetting contentItemSetting)
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = contentItemSetting.ParagraphAlignment;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.IsBold = contentItemSetting.HasBold;//文字加粗
            xwpfRun.SetText(fillContent);//填充内容
            xwpfRun.FontSize = contentItemSetting.FontSize;//设置文字大小
            xwpfRun.IsItalic = contentItemSetting.HasItalic;//是否设置斜体（字体倾斜）
            xwpfRun.SetColor(contentItemSetting.FontColor);//设置字体颜色--十六进制
            xwpfRun.SetFontFamily(contentItemSetting.FontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定
            xwpfRun.Underline = contentItemSetting.UnderlinePatterns;

            return paragraph;
        }

        /// <summary>
        /// 获取一个表格基础模板
        /// </summary>
        /// <param name="document"></param>
        /// <param name="tableSetting"></param>
        /// <param name="tableContents"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static XWPFTable InsertTable(XWPFDocument document, TableSetting tableSetting, List<TableContent> tableContents)
        {
            if (tableSetting.Row == 0 || tableSetting.Column == 0)
                throw new Exception("表格的行或列不可设定为0！");
            XWPFTable table = document.CreateTable(tableSetting.Row, tableSetting.Column);

            if (tableSetting.Width > 0)
                table.Width = tableSetting.Width;//总宽度

            if (tableSetting.ColWidthes.Count > 0)
                for (int i = 0; i < tableSetting.Column; i++)
                {
                    if (i < tableSetting.ColWidthes.Count)
                        table.SetColumnWidth(i, tableSetting.ColWidthes[i]); /* 设置列宽 */
                    else
                        table.SetColumnWidth(i, 1000); /* 设置列宽 */
                }

            if (tableSetting.Borders != null && tableSetting.Borders.Count > 0)
                foreach (var item in tableSetting.Borders)
                {
                    switch (item.Position)
                    {
                        case BorderPosition.Top:
                            table.SetTopBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;

                        case BorderPosition.Bot:
                            table.SetBottomBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;

                        case BorderPosition.Left:
                            table.SetLeftBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;

                        case BorderPosition.Right:
                            table.SetRightBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;

                        case BorderPosition.InsideH:
                            table.SetInsideHBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;

                        case BorderPosition.InsideV:
                            table.SetInsideVBorder(item.BorderType, item.Size, item.Space, item.Rgb);
                            break;
                    }
                }

            table.SetCellMargins(tableSetting.TopCellMargin, tableSetting.LeftCellMargin, tableSetting.BotCellMargin, tableSetting.RightCellMargin);

            for (int i = 0; i < tableSetting.Row; i++)
            {
                for (int j = 0; j < tableSetting.Column; j++)
                {
                    if (tableContents.Any(con => con.RowIndex == i && con.ColumnIndex == j))
                    {
                        var content = tableContents.First(con => con.RowIndex == i && con.ColumnIndex == j);
                        table.GetRow(i).GetCell(j).SetParagraph(SetTableSetting(table, content.TextValue, content.TextSetting));
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="fillContent">要填充的文字</param>
        /// <param name="contentItemSetting">内容设定</param>
        /// <returns></returns>
        public static XWPFParagraph SetTableSetting(XWPFTable table, string fillContent, ContentSetting contentItemSetting)
        {
            var para = new CT_P();
            //设置单元格文本对齐
            para.AddNewPPr().AddNewTextAlignment();

            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = contentItemSetting.ParagraphAlignment;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(fillContent);
            xwpfRun.FontSize = contentItemSetting.FontSize;//字体大小
            xwpfRun.SetColor(contentItemSetting.FontColor);//设置字体颜色--十六进制
            xwpfRun.IsItalic = contentItemSetting.HasItalic;//是否设置斜体（字体倾斜）
            xwpfRun.IsBold = contentItemSetting.HasBold;//是否加粗
            xwpfRun.SetFontFamily(contentItemSetting.FontFamily, FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）

            return paragraph;
        }

        /// <summary>
        /// 创建文档
        /// </summary>
        /// <param name="setting"></param>
        public static void ExportDocument(XWPFDocument docx, string savePath, out string errorMsg, bool substitute = false)
        {
            try
            {
                errorMsg = string.Empty;

                ////CT_SectPr setPr = docx.Document.body.sectPr = new CT_SectPr();
                //////获取页面大小
                ////Tuple<int, int> size = GetPaperSize(paperType);
                ////setPr.pgSz.w = (ulong)size.Item1;
                ////setPr.pgSz.h = (ulong)size.Item2;

                if (File.Exists(savePath) && !substitute)
                {
                    errorMsg = "给定路径已存在文件！";
                    return;
                }
                else if (File.Exists(savePath) && substitute)
                    File.Delete(savePath);

                using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    //开始写入
                    docx.Write(fs);
                    fs.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取纸张大小，单位：Twip
        /// <para>换算关系：1英寸=1440缇  1厘米=567缇  1磅=20缇  1像素=15缇</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Tuple<int, int> GetPaperSize(PaperType type)
        {
            Tuple<int, int> res = null;
            switch (type)
            {
                case PaperType.A4_V:
                    res = new Tuple<int, int>(11906, 16838);
                    break;

                case PaperType.A4_H:
                    res = new Tuple<int, int>(16838, 11906);
                    break;

                case PaperType.A5_V:
                    res = new Tuple<int, int>(8390, 11906);
                    break;

                case PaperType.A5_H:
                    res = new Tuple<int, int>(11906, 8390);
                    break;

                case PaperType.A6_V:
                    res = new Tuple<int, int>(5953, 8390);
                    break;

                case PaperType.A6_H:
                    res = new Tuple<int, int>(8390, 5953);
                    break;
            }
            return res;
        }
    }
}