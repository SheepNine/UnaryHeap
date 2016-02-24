using System.Drawing;

namespace GraphPaper
{
    class GraphPaperColors
    {
        public static Color SelectingPen {  get { return Color.FromArgb(128, 255, 128); } }
        public static Color HotTrackingPen { get { return Color.FromArgb(100, 220, 40); } }
        public static Color Paper { get { return Color.FromArgb(255, 250, 240); } }
        public static Color MinorGridLines { get { return Color.FromArgb(100, 200, 255); } }
        public static Color MajorGridLines { get { return Color.FromArgb(50, 100, 128); } }
        public static Color BluePen { get { return Color.FromArgb(30, 30, 160); } }
        public static Color SelectedBluePen { get { return Color.FromArgb(128, 128, 255); } }
        public static Color RedPen { get { return Color.FromArgb(170, 0, 0); } }
        public static Color SelectedRedPen { get { return Color.FromArgb(255, 40, 40); } }
    }
}
