using System.ComponentModel.DataAnnotations;
using SplashKitSDK;

namespace VLSEdit
{
    public abstract class Widget
    {
        private double _x;

        private double _y;

        public virtual double X { get { return _x; } set { _x = value; } }

        public virtual double Y { get { return _y; } set { _y = value; } }

        public abstract void Draw(double offsetX, double offsetY);

        public abstract bool PointWithin(double offsetX, double offsetY, Point2D point);
    }

    public class BoxWidget : Widget
    {
        private List<NodeWidget> _nodeWidgets = new List<NodeWidget>();

        private Box _box;

        private double _height;

        private bool _selected;

        public override double X { get { return _box.X; } set { _box.X = value; } }

        public override double Y { get { return _box.Y; } set { _box.Y = value; } }

        public double Height { get { return _height; } }

        public bool Selected { get { return _selected; } set { _selected = value; } }

        public Box Box { get { return _box; } }

        public List<NodeWidget> NodeWidgets { get { return _nodeWidgets; } }

        public BoxWidget(Box box)
        {
            _box = box;

            double clientNodeY = Constants.BOX_TITLE_HEIGHT + 15;

            double serverNodeY = clientNodeY;

            foreach (Node node in box.Nodes)
            {
                NodeWidget newWidget = new NodeWidget(node);

                _nodeWidgets.Add(newWidget);

                if (node is ServerNode)
                {
                    newWidget.X = 18;
                    newWidget.Y = clientNodeY;
                    clientNodeY += Constants.NODE_HEIGHT;
                }
                else
                {
                    newWidget.X = Constants.BOX_WIDTH - 18;
                    newWidget.Y = serverNodeY;
                    serverNodeY += Constants.NODE_HEIGHT;
                }
            }

            _height = Math.Max(clientNodeY, serverNodeY);
        }

        public override void Draw(double offsetX, double offsetY)
        {
            double screenX = offsetX + X;
            double screenY = offsetY + Y;

            if (_selected)
            {
                SplashKit.FillRectangle(
                    Constants.BOX_OUTLINE_COLOR,
                    screenX - Constants.BOX_OUTLINE_WIDTH,
                    screenY - Constants.BOX_OUTLINE_WIDTH,
                    Constants.BOX_WIDTH + Constants.BOX_OUTLINE_WIDTH * 2,
                    _height + Constants.BOX_OUTLINE_WIDTH * 2
                );
            }

            SplashKit.FillRectangle(Constants.BOX_BACKGROUND_COLOR, screenX, screenY, Constants.BOX_WIDTH, _height);

            SplashKit.FillRectangle(_box.Color, screenX, screenY, Constants.BOX_WIDTH, Constants.BOX_TITLE_HEIGHT);

            SplashKit.DrawText(_box.Name, Color.Black, Constants.FONT_PATH, 16, screenX + 5, screenY + 3);

            foreach (NodeWidget nodeWidget in _nodeWidgets)
            {
                nodeWidget.Draw(screenX, screenY);
            }
        }

        public override bool PointWithin(double offsetX, double offsetY, Point2D point)
        {
            double screenX = offsetX + X;
            double screenY = offsetY + Y;

            return point.X >= screenX && point.X <= screenX + Constants.BOX_WIDTH && point.Y >= screenY && point.Y <= screenY + _height;
        }
    }

    public class NodeWidget : Widget
    {
        private Node _node;

        public Node Node { get { return _node; } }

        public NodeWidget(Node node)
        {
            _node = node;
        }

        public override void Draw(double offsetX, double offsetY)
        {
            double screenX = offsetX + X;
            double screenY = offsetY + Y;

            if (_node is ServerNode)
            {
                SplashKit.FillCircle(Color.White, screenX, screenY, 7);

                SplashKit.DrawText(_node.Name, Color.White, Constants.FONT_PATH, 14, screenX + 15, screenY - 8);
            }
            else
            {
                SplashKit.FillRectangle(Color.White, screenX - 7, screenY - 7, 14, 14);

                SplashKit.DrawText(_node.Name, Color.White, Constants.FONT_PATH, 14, screenX + 15 - Constants.NODE_WIDTH, screenY - 8);
            }
        }

        public override bool PointWithin(double offsetX, double offsetY, Point2D point)
        {
            double screenX = offsetX + X;
            double screenY = offsetY + Y;

            if (_node is ServerNode)
            {
                return SplashKit.PointInCircle(point.X, point.Y, screenX, screenY, 7);
            }
            else
            {
                return SplashKit.PointInRectangle(point.X, point.Y, screenX - 7, screenY - 7, 14, 14);
            }
        }
    }
}