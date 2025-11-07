using SplashKitSDK;

namespace VLSEdit
{
    public abstract class KVMControllerDragState
    {
        private Point2D _initialState;

        private Point2D _startMousePosition;

        public Point2D InitialState { get { return _initialState; } set { _initialState = value; } }

        public Point2D StartMousePosition { get { return _startMousePosition; } set { _initialState = value; } }

        protected KVMControllerDragState(Point2D initialState)
        {
            _startMousePosition = SplashKit.MousePosition();
            _initialState = initialState;
        }

        public Point2D NewState(bool skipScaling = false)
        {
            double localScale = 1;

            if (!skipScaling)
            {
                localScale = Editor.Instance.View.Scale;
            }

            return new Point2D() { X = _initialState.X + (SplashKit.MouseX() - _startMousePosition.X) / localScale, Y = _initialState.Y + (SplashKit.MouseY() - _startMousePosition.Y) / localScale };
        }

        public virtual void Initiate()
        {
        }

        public virtual void UpdateTarget()
        {
        }

        public virtual void Finalise()
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

        public override void Finalise()
        {
            BoxWidget? topBoxWidget = ObjectFinder.FindTopBoxWidget();

            if (topBoxWidget == null || !topBoxWidget.Box.Mutable) return;

            NodeWidget? nodeWidget = ObjectFinder.FindNodeWidget(topBoxWidget);

            if (nodeWidget == null) return;

            Node nodeA = _selectedNodeWidget.Node;
            Node nodeB = nodeWidget.Node;

            if (nodeA.GetType() == nodeB.GetType()) return;

            if (nodeA is ServerNode)
            {
                Node tmp = nodeA;
                nodeA = nodeB;
                nodeB = tmp;
            }

            ((ClientNode)nodeA).To = (ServerNode)nodeB;

            Editor.Instance.RegisterChange();
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

        public override void Initiate()
        {
            Editor.Instance.SelectedBoxWidget = _selectedBoxWidget;
            Editor.Instance.SelectedBoxWidget.Selected = true;

            Editor.Instance.View.BoxWidgets.Remove(Editor.Instance.SelectedBoxWidget);
            Editor.Instance.View.BoxWidgets.Add(Editor.Instance.SelectedBoxWidget);
        }

        public override void UpdateTarget()
        {
            Point2D newState = NewState();

            _selectedBoxWidget.X = newState.X;
            _selectedBoxWidget.Y = newState.Y;
        }

        public override void Finalise()
        {
            if (!NewState().IsEqualTo(InitialState))
            {
                Editor.Instance.RegisterChange();
            }
        }
    }

    public class KVMControllerDragStateView : KVMControllerDragState
    {
        public KVMControllerDragStateView() : base(new Point2D() { X = Editor.Instance.View.OffsetX, Y = Editor.Instance.View.OffsetY })
        {
        }

        public override void UpdateTarget()
        {
            Point2D newState = NewState(true);

            Editor.Instance.View.OffsetX = newState.X;
            Editor.Instance.View.OffsetY = newState.Y;
        }
    }

    public class KVMControllerDragStateButton : KVMControllerDragState
    {
        private ButtonWidget _selectedWidget;

        public ButtonWidget SelectedWidget { get { return _selectedWidget; } }

        public KVMControllerDragStateButton(ButtonWidget widget) : base(new Point2D())
        {
            _selectedWidget = widget;
        }

        public override void Initiate()
        {
            _selectedWidget.Clicking = true;
        }

        public override void Finalise()
        {
            _selectedWidget.Clicking = false;

            _selectedWidget.ClickAction.Execute();
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
                if (_dragState is KVMControllerDragStateNone)
                {
                    ButtonWidget? toolbarButtonWidget = ObjectFinder.FindToolbarButtonWidget();

                    if (toolbarButtonWidget != null)
                    {
                        _dragState = new KVMControllerDragStateButton(toolbarButtonWidget);

                        toolbarButtonWidget.Clicking = true;
                    }

                    if (_dragState is KVMControllerDragStateNone)
                    {
                        foreach (BoxWidget boxWidget in Editor.Instance.View.BoxWidgets)
                        {
                            Editor.Instance.SelectedBoxWidget = null;

                            boxWidget.Selected = false;

                            if (ObjectFinder.MouseWithinBoxWidget(boxWidget))
                            {
                                _dragState = new KVMControllerDragStateBox(boxWidget);
                            }

                            if (boxWidget.Box.Mutable)
                            {
                                NodeWidget? nodeWidget = ObjectFinder.FindNodeWidget(boxWidget);

                                if (nodeWidget != null)
                                {
                                    _dragState = new KVMControllerDragStateNode(nodeWidget);
                                }
                            }

                            foreach (ButtonWidget buttonWidget in boxWidget.ButtonWidgets)
                            {
                                if (buttonWidget.PointWithin(Editor.Instance.View.OffsetX + boxWidget.X * Editor.Instance.View.Scale, Editor.Instance.View.OffsetY + boxWidget.Y * Editor.Instance.View.Scale, Editor.Instance.View.Scale, SplashKit.MousePosition()))
                                {
                                    _dragState = new KVMControllerDragStateButton(buttonWidget);
                                }
                            }
                        }
                    }

                    if (_dragState is KVMControllerDragStateNone)
                    {
                        _dragState = new KVMControllerDragStateView();
                    }

                    _dragState.Initiate();
                }

                _dragState.UpdateTarget();
            }
            else
            {
                _dragState.Finalise();

                _dragState = new KVMControllerDragStateNone();
            }

            if (SplashKit.MouseClicked(MouseButton.RightButton))
            {
                BoxWidget? topBoxWidget = ObjectFinder.FindTopBoxWidget();

                if (topBoxWidget != null)
                {
                    NodeWidget? nodeWidget = ObjectFinder.FindNodeWidget(topBoxWidget);

                    if (nodeWidget == null)
                    {
                        new SetBoxValueCommand(topBoxWidget.Box).Execute();
                    }
                    else
                    {
                        if (nodeWidget.Node is ClientNode)
                        {
                            ((ClientNode)nodeWidget.Node).To = null;
                        }
                        else
                        {
                            Editor.Instance.BreakLinksTo(nodeWidget.Node);
                        }

                        Editor.Instance.RegisterChange();
                    }
                }
                else
                {
                    if (Editor.Instance.SelectedBoxWidget != null)
                    {
                        new CloneBoxCommand().Execute();
                    }
                }
            }

            if (Editor.Instance.SelectedBoxWidget != null)
            {
                if (SplashKit.KeyTyped(KeyCode.DeleteKey) || SplashKit.KeyTyped(KeyCode.BackspaceKey))
                {
                    new DeleteBoxCommand(Editor.Instance.SelectedBoxWidget).Execute();
                }
            }

            if (SplashKit.KeyDown(KeyCode.LeftCtrlKey) || SplashKit.KeyDown(KeyCode.RightCtrlKey))
            {
                if (SplashKit.KeyTyped(KeyCode.SKey))
                {
                    new SaveCommand().Execute();
                }

                if (SplashKit.KeyTyped(KeyCode.DKey))
                {
                    new AutoCleanCommand().Execute();
                }

                if (SplashKit.KeyTyped(KeyCode.ZKey) || SplashKit.KeyTyped(KeyCode.YKey))
                {
                    new ChangeStateCommand(SplashKit.KeyTyped(KeyCode.YKey)).Execute();
                }

                if (SplashKit.KeyTyped(KeyCode.EqualsKey) || SplashKit.KeyTyped(KeyCode.MinusKey))
                {
                    double amount = -1;

                    if (SplashKit.KeyTyped(KeyCode.EqualsKey))
                    {
                        amount = 1;
                    }

                    new ScaleCommand(amount, true).Execute();
                }
            }

            if (SplashKit.MouseWheelScroll().Y != 0)
            {
                new ScaleCommand(SplashKit.MouseWheelScroll().Y).Execute();
            }
        }
    }
}