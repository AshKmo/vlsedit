using System.ComponentModel.Design;
using SplashKitSDK;

namespace VLSEdit
{
    public abstract class KVMControllerDragState
    {
        private Point2D _initialState;

        private Point2D _startMousePosition;

        public Point2D InitialState { get { return _initialState; } }

        public Point2D StartMousePosition { get { return _startMousePosition; } }

        protected KVMControllerDragState(Point2D initialState)
        {
            _startMousePosition = SplashKit.MousePosition();
            _initialState = initialState;
        }

        public Point2D NewState()
        {
            return new Point2D() { X = _initialState.X + SplashKit.MouseX() - _startMousePosition.X, Y = _initialState.Y + SplashKit.MouseY() - _startMousePosition.Y };
        }

        public virtual void UpdateTarget()
        {
        }
    }

    public class KVMControllerDragStateNone : KVMControllerDragState
    {
        public KVMControllerDragStateNone() : base(new Point2D())
        {
        }
    }

    public class KVMControllerDragStateNode : KVMControllerDragState
    {
        private NodeWidget _selectedNodeWidget;

        public NodeWidget SelectedNodeWidget { get { return _selectedNodeWidget; } }

        public KVMControllerDragStateNode(NodeWidget widget) : base(SplashKit.MousePosition())
        {
            _selectedNodeWidget = widget;
        }
    }

    public class KVMControllerDragStateBox : KVMControllerDragState
    {
        private BoxWidget _selectedBoxWidget;

        public BoxWidget SelectedBoxWidget { get { return _selectedBoxWidget; } }

        public KVMControllerDragStateBox(BoxWidget widget) : base(new Point2D() { X = widget.X, Y = widget.Y })
        {
            _selectedBoxWidget = widget;
        }

        public override void UpdateTarget()
        {
            Point2D newState = NewState();

            _selectedBoxWidget.X = newState.X;
            _selectedBoxWidget.Y = newState.Y;
        }
    }

    public class KVMControllerDragStateView : KVMControllerDragState
    {
        public KVMControllerDragStateView() : base(new Point2D() { X = Editor.Instance.View.OffsetX, Y = Editor.Instance.View.OffsetY })
        {
        }

        public override void UpdateTarget()
        {
            Point2D newState = NewState();

            Editor.Instance.View.OffsetX = newState.X;
            Editor.Instance.View.OffsetY = newState.Y;
        }
    }

    public class KVMController
    {
        private KVMControllerDragState _dragState = new KVMControllerDragStateNone();

        public KVMControllerDragState DragState { get { return _dragState; } }

        public KVMController()
        {
        }

        public void Tick()
        {
            if (SplashKit.MouseDown(MouseButton.LeftButton))
            {
                switch (_dragState)
                {
                    case KVMControllerDragStateNone:
                        foreach (BoxWidget boxWidget in Editor.Instance.View.BoxWidgets)
                        {
                            Editor.Instance.SelectedBoxWidget = null;

                            boxWidget.Selected = false;

                            if (MouseWithinBoxWidget(boxWidget))
                            {
                                _dragState = new KVMControllerDragStateBox(boxWidget);
                            }

                            if (boxWidget.Box.Mutable)
                            {
                                NodeWidget? nodeWidget = FindNodeWidget(boxWidget);

                                if (nodeWidget != null)
                                {
                                    _dragState = new KVMControllerDragStateNode(nodeWidget);
                                }
                            }
                        }

                        switch (_dragState)
                        {
                            case KVMControllerDragStateBox dragStateBox:
                                Editor.Instance.SelectedBoxWidget = dragStateBox.SelectedBoxWidget;
                                Editor.Instance.SelectedBoxWidget.Selected = true;

                                Editor.Instance.View.BoxWidgets.Remove(Editor.Instance.SelectedBoxWidget);
                                Editor.Instance.View.BoxWidgets.Add(Editor.Instance.SelectedBoxWidget);
                                break;

                            case KVMControllerDragStateNone:
                                _dragState = new KVMControllerDragStateView();
                                break;
                        }

                        break;
                }

                _dragState.UpdateTarget();
            }
            else
            {
                switch (_dragState)
                {
                    case KVMControllerDragStateNode dragStateNode:
                        BoxWidget? topBoxWidget = FindTopBoxWidget();

                        if (topBoxWidget == null || !topBoxWidget.Box.Mutable) break;

                        NodeWidget? nodeWidget = FindNodeWidget(topBoxWidget);

                        if (nodeWidget == null) break;

                        Node nodeA = dragStateNode.SelectedNodeWidget.Node;
                        Node nodeB = nodeWidget.Node;

                        if (nodeA.GetType() == nodeB.GetType()) break;

                        if (nodeA is ServerNode)
                        {
                            Node tmp = nodeA;
                            nodeA = nodeB;
                            nodeB = tmp;
                        }

                        ((ClientNode)nodeA).To = (ServerNode)nodeB;

                        break;
                }

                _dragState = new KVMControllerDragStateNone();
            }

            if (SplashKit.MouseClicked(MouseButton.RightButton))
            {
                BoxWidget? topBoxWidget = FindTopBoxWidget();

                if (topBoxWidget != null)
                {
                    NodeWidget? nodeWidget = FindNodeWidget(topBoxWidget);

                    if (nodeWidget != null)
                    {
                        if (nodeWidget.Node is ClientNode)
                        {
                            ((ClientNode)nodeWidget.Node).To = null;
                        }
                        else
                        {
                            Editor.Instance.BreakLinksTo(nodeWidget.Node);
                        }
                    }
                }
            }

            if (Editor.Instance.SelectedBoxWidget != null)
            {
                if (SplashKit.KeyTyped(KeyCode.CKey))
                {
                    new CloneCommand().Execute();
                }

                if (SplashKit.KeyTyped(KeyCode.DeleteKey) || SplashKit.KeyTyped(KeyCode.BackspaceKey))
                {
                    Editor.Instance.RemoveBox(Editor.Instance.SelectedBoxWidget);
                    Editor.Instance.SelectedBoxWidget = null;
                }
            }
        }

        private bool MouseWithinBoxWidget(BoxWidget widget)
        {
            return widget.PointWithin(Editor.Instance.View.OffsetX, Editor.Instance.View.OffsetY, SplashKit.MousePosition());
        }

        private bool MouseWithinNodeWidget(BoxWidget boxWidget, NodeWidget nodeWidget)
        {
            return nodeWidget.PointWithin(Editor.Instance.View.OffsetX + boxWidget.X, Editor.Instance.View.OffsetY + boxWidget.Y, SplashKit.MousePosition());
        }

        private BoxWidget? FindTopBoxWidget()
        {
            BoxWidget? topBoxWidget = null;

            foreach (BoxWidget boxWidget in Editor.Instance.View.BoxWidgets)
            {
                if (!MouseWithinBoxWidget(boxWidget)) continue;

                topBoxWidget = boxWidget;
            }

            return topBoxWidget;
        }

        private NodeWidget? FindNodeWidget(BoxWidget boxWidget)
        {
            foreach (NodeWidget nodeWidget in boxWidget.NodeWidgets)
            {
                if (MouseWithinNodeWidget(boxWidget, nodeWidget)) return nodeWidget;
            }

            return null;
        }
    }
}