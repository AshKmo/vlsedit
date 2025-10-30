using System.Diagnostics;
using System.Timers;
using SplashKitSDK;

namespace VLSEdit
{
    public class KVMView
    {
        private double _offsetX;

        private double _offsetY;

        private double _zoom = 0;

        private double _scale = 1;

        private System.Timers.Timer? _alertTimer = null;

        private string? _alertText = "";

        private Stopwatch frameRateStopwatch = new Stopwatch();

        private List<BoxWidget> _boxWidgets = new List<BoxWidget>();

        private ToolbarWidget _toolbarWidget = new ToolbarWidget();

        private long _lastFrameTime = 0;

        public double OffsetX { get { return _offsetX; } set { _offsetX = value; } }

        public double OffsetY { get { return _offsetY; } set { _offsetY = value; } }

        public List<BoxWidget> BoxWidgets { get { return _boxWidgets; } set { _boxWidgets = value; } }

        public ToolbarWidget Toolbar { get { return _toolbarWidget; } }

        public double Scale { get { return _scale; } }

        public KVMView()
        {
            double height = Constants.TOOLBAR_HEIGHT - 10;

            _toolbarWidget.AddButton(new ButtonWidget(5, 5, 65, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("save", "icons/save.svg"), "Save", new SaveCommand()));

            _toolbarWidget.AddButton(new ButtonWidget(100, 5, 65, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("undo", "icons/undo.svg"), "Undo", new ChangeStateCommand(false)));
            _toolbarWidget.AddButton(new ButtonWidget(170, 5, 65, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("redo", "icons/redo.svg"), "Redo", new ChangeStateCommand(true)));

            _toolbarWidget.AddButton(new ButtonWidget(265, 5, 100, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("autoclean", "icons/autoclean.svg"), "Auto Clean", new AutoCleanCommand()));

            _toolbarWidget.AddButton(new ButtonWidget(395, 5, 95, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("zoomout", "icons/zoomout.svg"), "Zoom Out", new ScaleCommand(-1, true)));
            _toolbarWidget.AddButton(new ButtonWidget(495, 5, 85, height, 7, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("zoomin", "icons/zoomin.svg"), "Zoom In", new ScaleCommand(1, true)));
        }

        public void Draw()
        {
            SplashKit.ClearScreen(Constants.BACKGROUND_COLOR);

            foreach (BoxWidget boxWidget in _boxWidgets)
            {
                if (!BoxWidgetOnScreen(boxWidget)) continue;

                boxWidget.Draw(_offsetX, _offsetY, Scale);
            }

            foreach (BoxWidget boxWidgetA in _boxWidgets)
            {
                foreach (NodeWidget nodeWidgetA in boxWidgetA.NodeWidgets)
                {
                    if (nodeWidgetA.Node is not ClientNode) continue;

                    ClientNode clientNode = (ClientNode)nodeWidgetA.Node;

                    if (clientNode.To == null) continue;

                    foreach (BoxWidget boxWidgetB in _boxWidgets)
                    {
                        if (!(BoxWidgetOnScreen(boxWidgetA) || BoxWidgetOnScreen(boxWidgetB))) continue;

                        foreach (NodeWidget nodeWidgetB in boxWidgetB.NodeWidgets)
                        {
                            if (nodeWidgetB.Node is not ServerNode) continue;

                            if ((ServerNode)nodeWidgetB.Node != clientNode.To) continue;

                            SplashKit.DrawLine(
                                Constants.LINK_LINE_COLOR,
                                (nodeWidgetA.X + boxWidgetA.X) * Editor.Instance.View.Scale + Editor.Instance.View.OffsetX,
                                (nodeWidgetA.Y + boxWidgetA.Y) * Editor.Instance.View.Scale + Editor.Instance.View.OffsetY,
                                (nodeWidgetB.X + boxWidgetB.X) * Editor.Instance.View.Scale + Editor.Instance.View.OffsetX,
                                (nodeWidgetB.Y + boxWidgetB.Y) * Editor.Instance.View.Scale + Editor.Instance.View.OffsetY
                            );

                            goto FoundServerNode;
                        }
                    }

                FoundServerNode:;
                }

                if (_alertText != null)
                {
                    SplashKit.DrawText(_alertText, Color.White, Constants.FONT_PATH, 20, 10, Constants.WINDOW_HEIGHT - 35);
                }
            }

            if (Editor.Instance.Controller.DragState is KVMControllerDragStateNode)
            {
                KVMControllerDragStateNode nodeDragState = (KVMControllerDragStateNode)Editor.Instance.Controller.DragState;

                SplashKit.DrawLine(Constants.DRAG_LINE_COLOR, SplashKit.LineFrom(nodeDragState.StartMousePosition, nodeDragState.NewState(true)));
            }

            _toolbarWidget.Draw(0, 0, 1);

            if (_lastFrameTime != 0)
            {
                SplashKit.DrawText("FPS: " + (1000 / _lastFrameTime).ToString(), Color.Black, Constants.WINDOW_WIDTH - 65, 10);
            }

            _lastFrameTime = frameRateStopwatch.ElapsedMilliseconds;

            frameRateStopwatch.Restart();
        }

        public void Alert(string text, int duration)
        {
            if (_alertTimer != null)
            {
                _alertTimer.Close();
            }

            _alertTimer = new System.Timers.Timer(duration);

            _alertTimer.Elapsed += (Object? _, ElapsedEventArgs _) =>
            {
                _alertText = null;
            };

            _alertTimer.Enabled = true;

            _alertText = text;
        }

        private bool BoxWidgetOnScreen(BoxWidget boxWidget)
        {
            double screenX = OffsetX + boxWidget.X * Scale;
            double screenY = OffsetY + boxWidget.Y * Scale;

            return
                (screenX + Constants.BOX_WIDTH * Scale >= 0 && screenX < Constants.WINDOW_WIDTH) &&
                (screenY + boxWidget.Height * Scale >= 0 && screenY < Constants.WINDOW_HEIGHT);
        }

        public void Zoom(double amount)
        {
            _zoom = Math.Min(_zoom + amount, 0);

            _scale = Math.Pow(2, _zoom);
        }
    }
}