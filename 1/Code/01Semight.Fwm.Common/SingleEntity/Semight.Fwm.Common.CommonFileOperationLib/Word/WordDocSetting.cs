using NPOI.XWPF.UserModel;
using System.Collections.Generic;

namespace Semight.Fwm.Common.CommonFileOperationLib.Word
{
    /// <summary>
    /// 段落设定
    /// </summary>
    public class ContentSetting
    {
        /// <summary>
        /// 使用字体
        /// </summary>
        public string FontFamily { get; set; } = "宋体";

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// 是否加粗，默认不加粗
        /// </summary>
        public bool HasBold { get; set; } = false;

        /// <summary>
        /// 是否倾斜，默认不倾斜
        /// </summary>
        public bool HasItalic { get; set; } = false;

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string FontColor { get; set; } = "000000";

        /// <summary>
        /// 对齐方式
        /// </summary>
        public ParagraphAlignment ParagraphAlignment { get; set; } = ParagraphAlignment.LEFT;

        /// <summary>
        /// 下划线
        /// </summary>
        public UnderlinePatterns UnderlinePatterns { get; set; } = UnderlinePatterns.None;
    }

    public class TableSetting
    {
        public int Width { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public List<ulong> ColWidthes { get; set; }

        public List<TableBorder> Borders { get; set; }

        public int TopCellMargin { get; set; }

        public int BotCellMargin { get; set; }

        public int LeftCellMargin { get; set; }

        public int RightCellMargin { get; set; }
    }

    public class TableBorder
    {
        public BorderPosition Position { get; set; }

        public XWPFTable.XWPFBorderType BorderType { get; set; } = XWPFTable.XWPFBorderType.NIL;

        public int Size { get; set; }

        public int Space { get; set; }

        public string Rgb { get; set; }
    }

    public class TableContent
    {
        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public string TextValue { get; set; }

        public ContentSetting TextSetting { get; set; } = new ContentSetting();
    }

    public enum BorderPosition
    {
        Top,
        Bot,
        Left,
        Right,
        InsideH,
        InsideV
    }

    /// <summary>
    /// 纸张类型
    /// </summary>
    public enum PaperType
    {
        /// <summary>
        /// A4纵向
        /// </summary>
        A4_V,

        /// <summary>
        /// A4横向
        /// </summary>
        A4_H,

        /// <summary>
        /// A5纵向
        /// </summary>
        A5_V,

        /// <summary>
        /// A5横向
        /// </summary>
        A5_H,

        /// <summary>
        /// A6纵向
        /// </summary>
        A6_V,

        /// <summary>
        /// A6横向
        /// </summary>
        A6_H
    }
}