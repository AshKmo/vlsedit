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

        private List<BoxWidget> _boxWidgets = new List<BoxWidget>();

        public double OffsetX { get { return _offsetX; } set { _offsetX = value; } }

        public double OffsetY { get { return _offsetY; } set { _offsetY = value; } }

        public List<BoxWidget> BoxWidgets { get { return _boxWidgets; } }

        public void Draw()
        {
            SplashKit.ClearScreen(Constants.BACKGROUND_COLOR);

            foreach (BoxWidget boxWidget in _boxWidgets)
            {
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
    }
}