using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace GraphPaper
{
    class GraphPaperToolbox : Toolbox<IViewModel>
    {
        public static readonly IToolbox<IViewModel> Instance = new GraphPaperToolbox();

        private GraphPaperToolbox()
        {
            SetMissingClickTool(UnsupportedTool.Instance);
            SetMissingDragTool(UnsupportedTool.Instance);

            SetClickTool(Keys.None, MouseButtons.Left, new SelectSingleObjectTool());
            SetClickTool(Keys.Control, MouseButtons.Left, new ToggleSingleObjectSelectionTool());
            SetClickTool(Keys.Alt, MouseButtons.Left, new CenterViewTool());
            SetClickTool(Keys.None, MouseButtons.Right, new AddVertexTool());

            SetDragTool(Keys.Alt, MouseButtons.Left, new AdjustViewTool());
            SetDragTool(Keys.None, MouseButtons.Right, new AddEdgeTool());
        }
    }

    class UnsupportedTool : IClickTool<IViewModel>, IDragTool<IViewModel>
    {
        public static readonly UnsupportedTool Instance = new UnsupportedTool();

        private UnsupportedTool() { }

        public void Gestured(IViewModel context, Point p)
        {
            SystemSounds.Beep.Play();
        }

        public void Gestured(IViewModel context, Point start, Point end)
        {
            SystemSounds.Beep.Play();
        }

        public void Gesturing(IViewModel context, Point p)
        {
            context.ShowNoOperationFeedback();
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.ShowNoOperationFeedback();
        }
    }

    class SelectSingleObjectTool : IClickTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point p)
        {
            context.SelectSingleObject(p);
        }

        public void Gesturing(IViewModel context, Point p)
        {
            context.PreviewSelectSingleObject(p);
        }
    }

    class ToggleSingleObjectSelectionTool : IClickTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point p)
        {
            context.ToggleSingleObjectSelection(p);
        }

        public void Gesturing(IViewModel context, Point p)
        {
            context.PreviewToggleSingleObjectSelection(p);
        }
    }

    class CenterViewTool : IClickTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point p)
        {
            context.CenterView(p);
        }

        public void Gesturing(IViewModel context, Point p)
        {
            context.PreviewCenterView(p);
        }
    }

    class AddVertexTool : IClickTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point p)
        {
            context.AddVertex(p);
        }

        public void Gesturing(IViewModel context, Point p)
        {
            context.PreviewAddVertex(p);
        }
    }

    class AdjustViewTool : IDragTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point start, Point end)
        {
            context.AdjustViewExtents(
                ComputeBoundingRectangle(start, end));
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewAdjustViewExtents(
                ComputeBoundingRectangle(start, current));
        }

        static Rectangle ComputeBoundingRectangle(Point a, Point b)
        {
            return Rectangle.FromLTRB(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y));
        }
    }

    class AddEdgeTool : IDragTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point start, Point end)
        {
            context.AddEdge(start, end);
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewAddEdge(start, current);
        }
    }
}
