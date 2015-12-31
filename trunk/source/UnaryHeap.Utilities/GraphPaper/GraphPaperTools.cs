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

            SetClickTool(Keys.Shift, MouseButtons.Left, new SelectSingleObjectTool());
            SetClickTool(Keys.Shift, MouseButtons.Right, new ToggleSingleObjectSelectionTool());
            SetClickTool(Keys.Alt, MouseButtons.Left, new CenterViewTool());
            SetClickTool(Keys.None, MouseButtons.Right, new AddVertexTool());


            SetDragTool(Keys.Shift, MouseButtons.Left, new SelectObjectsInAreaTool());
            SetDragTool(Keys.Shift, MouseButtons.Right, new AppendObjectsInAreaToSelectionTool());
            SetDragTool(Keys.Alt, MouseButtons.Left, new AdjustViewTool());
            SetDragTool(Keys.None, MouseButtons.Left, new MoveSelectedTool());
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
            context.AdjustViewExtents(start.RectangleTo(end));
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewAdjustViewExtents(start.RectangleTo(current));
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

    class SelectObjectsInAreaTool : IDragTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point start, Point end)
        {
            context.SelectObjectsInArea(start.RectangleTo(end));
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewSelectObjectsInArea(start.RectangleTo(current));
        }
    }

    class AppendObjectsInAreaToSelectionTool : IDragTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point start, Point end)
        {
            context.AppendObjectsInAreaToSelection(start.RectangleTo(end));
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewAppendObjectsInAreaToSelection(start.RectangleTo(current));
        }
    }

    class MoveSelectedTool : IDragTool<IViewModel>
    {
        public void Gestured(IViewModel context, Point start, Point end)
        {
            context.MoveSelected(start, end);
        }

        public void Gesturing(IViewModel context, Point start, Point current)
        {
            context.PreviewMoveSelected(start, current);
        }
    }
}
