using System.Diagnostics;
using System.Timers;
using SplashKitSDK;

namespace VLSEdit
{
    public class KVMView
    {
        private double _offsetX;

        private double _offsetY;

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

        public KVMView()
        {
            double height = Constants.TOOLBAR_HEIGHT - 10;

            _toolbarWidget.AddButton(new ButtonWidget(5, 5, 65, height, 5, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("save", "icons/save.png"), "Save", new SaveCommand()));

            _toolbarWidget.AddButton(new ButtonWidget(75, 5, 65, height, 5, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("undo", "icons/undo.png"), "Undo", new ChangeStateCommand(false)));
            _toolbarWidget.AddButton(new ButtonWidget(145, 5, 65, height, 5, Constants.TOOLBAR_BUTTON_COLOR, Constants.TOOLBAR_BUTTON_COLOR_CLICKING, SplashKit.LoadBitmap("redo", "icons/redo.png"), "Redo", new ChangeStateCommand(true)));
        }

        public void Draw()
        {
            SplashKit.ClearScreen(Constants.BACKGROUND_COLOR);

            foreach (BoxWidget boxWidget in _boxWidgets)
            {
                if (!BoxWidgetOnScreen(boxWidget)) continue;

                boxWidget.Draw(_offsetX, _offsetY);
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
                                nodeWidgetA.X + boxWidgetA.X + Editor.Instance.View.OffsetX,
                                nodeWidgetA.Y + boxWidgetA.Y + Editor.Instance.View.OffsetY,
                                nodeWidgetB.X + boxWidgetB.X + Editor.Instance.View.OffsetX,
                                nodeWidgetB.Y + boxWidgetB.Y + Editor.Instance.View.OffsetY
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

                SplashKit.DrawLine(Constants.DRAG_LINE_COLOR, SplashKit.LineFrom(nodeDragState.StartMousePosition, nodeDragState.NewState()));
            }

            _toolbarWidget.Draw(0, 0);

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
            double screenX = boxWidget.X + OffsetX;
            double screenY = boxWidget.Y + OffsetY;

            return
                (screenX + Constants.BOX_WIDTH >= 0 && screenX < Constants.WINDOW_WIDTH) &&
                (screenY + boxWidget.Height >= 0 && screenY < Constants.WINDOW_HEIGHT);
        }
    }
}