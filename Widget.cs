using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Runtime.CompilerServices;
using SplashKitSDK;

namespace VLSEdit
{
    public abstract class Widget
    {
        private double _x;

        private double _y;

        public virtual double X { get { return _x; } set { _x = value; } }

        public virtual double Y { get { return _y; } set { _y = value; } }

        public abstract void Draw(double offsetX, double offsetY, double scale);

        public abstract bool PointWithin(double offsetX, double offsetY, double scale, Point2D point);
    }

    public class BoxWidget : Widget
    {
        private List<NodeWidget> _nodeWidgets = new List<NodeWidget>();

        private List<ButtonWidget> _buttonWidgets = new List<ButtonWidget>();

        private Box _box;

        private double _height;

        private bool _selected;

        public override double X { get { return _box.X; } set { _box.X = value; } }

        public override double Y { get { return _box.Y; } set { _box.Y = value; } }

        public double Height { get { return _height; } }

        public bool Selected { get { return _selected; } set { _selected = value; } }

        public Box Box { get { return _box; } }

        public List<NodeWidget> NodeWidgets { get { return _nodeWidgets; } }

        public List<ButtonWidget> ButtonWidgets { get { return _buttonWidgets; } }

        public BoxWidget(Box box)
        {
            _box = box;

            double clientNodeY = Constants.BOX_TITLE_HEIGHT + 20;

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

                _buttonWidgets.Add(new ButtonWidget(Constants.BOX_WIDTH - 20, 5, 15, 15, 0, Constants.TRANSPARENT_COLOR, Color.White, Constants.BUTTON_CLONE_BITMAP, "", new CloneBoxCommand(_box)));

                if (_box.Mutable)
                {
                    _buttonWidgets.Add(new ButtonWidget(Constants.BOX_WIDTH - 40, 5, 15, 15, 0, Constants.TRANSPARENT_COLOR, Color.White, Constants.BUTTON_DELETE_BITMAP, "", new DeleteBoxCommand(this)));
                }

                if (_box is IValueSettable && _box.Mutable)
                {
                    _buttonWidgets.Add(new ButtonWidget(Constants.BOX_WIDTH - 60, 5, 15, 15, 0, Constants.TRANSPARENT_COLOR, Color.White, Constants.BUTTON_SETVALUE_BITMAP, "", new SetBoxValueCommand(_box)));
                }
            }

            _height = Math.Max(clientNodeY, serverNodeY) - 5;
        }

        public override void Draw(double offsetX, double offsetY, double scale)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            if (_selected)
            {
                SplashKit.FillRectangle(
                    Constants.BOX_OUTLINE_COLOR,
                    screenX - Constants.BOX_OUTLINE_WIDTH * scale,
                    screenY - Constants.BOX_OUTLINE_WIDTH * scale,
                    (Constants.BOX_WIDTH + Constants.BOX_OUTLINE_WIDTH * 2) * scale,
                    (_height + Constants.BOX_OUTLINE_WIDTH * 2) * scale
                );
            }

            SplashKit.FillRectangle(Constants.BOX_BACKGROUND_COLOR, screenX, screenY, Constants.BOX_WIDTH * scale, _height * scale);

            SplashKit.FillRectangle(_box.Color, screenX, screenY, Constants.BOX_WIDTH * scale, Constants.BOX_TITLE_HEIGHT * scale);

            if (Editor.Instance.View.Scale >= Constants.HIGH_DETAIL_SCALE)
            {
                SplashKit.DrawText(_box.Name, Color.Black, Constants.FONT_PATH, (int)Math.Round(16 * scale), screenX + 5 * scale, screenY + 3 * scale);
            }

            foreach (ButtonWidget buttonWidget in _buttonWidgets)
            {
                buttonWidget.Draw(screenX, screenY, scale);
            }

            foreach (NodeWidget nodeWidget in _nodeWidgets)
            {
                nodeWidget.Draw(screenX, screenY, scale);
            }
        }

        public override bool PointWithin(double offsetX, double offsetY, double scale, Point2D point)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            return point.X >= screenX && point.X <= screenX + Constants.BOX_WIDTH * scale && point.Y >= screenY && point.Y <= screenY + _height * scale;
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

        public override void Draw(double offsetX, double offsetY, double scale)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            if (_node is ServerNode)
            {
                SplashKit.FillCircle(Color.White, screenX, screenY, 7 * scale);

                if (Editor.Instance.View.Scale >= Constants.HIGH_DETAIL_SCALE)
                {
                    SplashKit.DrawText(_node.Name, Color.White, Constants.FONT_PATH, (int)Math.Round(14 * scale), screenX + 15 * scale, screenY - 8 * scale);
                }
            }
            else
            {
                SplashKit.FillRectangle(Color.White, screenX - 7 * scale, screenY - 7 * scale, 14 * scale, 14 * scale);

                if (Editor.Instance.View.Scale >= Constants.HIGH_DETAIL_SCALE)
                {
                    SplashKit.DrawText(_node.Name, Color.White, Constants.FONT_PATH, (int)Math.Round(14 * scale), screenX + (15 - Constants.NODE_WIDTH) * scale, screenY - 8 * scale);
                }
            }
        }

        public override bool PointWithin(double offsetX, double offsetY, double scale, Point2D point)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            if (_node is ServerNode)
            {
                return SplashKit.PointInCircle(point.X, point.Y, screenX, screenY, 7 * scale);
            }
            else
            {
                return SplashKit.PointInRectangle(point.X, point.Y, screenX - 7 * scale, screenY - 7 * scale, 14 * scale, 14 * scale);
            }
        }
    }

    public class ToolbarWidget : Widget
    {
        private List<ButtonWidget> _buttons = new List<ButtonWidget>();

        public List<ButtonWidget> Buttons { get { return _buttons; } }

        public override double X { get { return 0; } }

        public override double Y { get { return 0; } }

        public ToolbarWidget()
        {
        }

        public override void Draw(double offsetX, double offsetY, double scale)
        {
            SplashKit.FillRectangle(Constants.TOOLBAR_COLOR, 0, 0, Constants.WINDOW_WIDTH, Constants.TOOLBAR_HEIGHT);

            foreach (ButtonWidget buttonWidget in _buttons)
            {
                buttonWidget.Draw(offsetX, offsetY, scale);
            }
        }

        public override bool PointWithin(double offsetX, double offsetY, double scale, Point2D point)
        {
            return point.Y <= Constants.TOOLBAR_HEIGHT;
        }

        public void AddButton(ButtonWidget button)
        {
            _buttons.Add(button);
        }
    }

    public class ButtonWidget : Widget
    {
        private Command _clickAction;

        private double _width;

        private double _height;

        private string _text;

        private bool _clicking;

        private Bitmap _icon;

        private Color _color;

        private Color _clickingColor;

        private double _leftPad;

        public bool Clicking { set { _clicking = value; } }

        public Command ClickAction { get { return _clickAction; } }

        public ButtonWidget(double x, double y, double width, double height, double leftPad, Color color, Color clickingColor, Bitmap icon, string text, Command clickAction)
        {
            _icon = icon;

            X = x;
            Y = y;

            _width = width;
            _height = height;

            _text = text;

            _clickAction = clickAction;

            _color = color;
            _clickingColor = clickingColor;

            _leftPad = leftPad;
        }

        public override void Draw(double offsetX, double offsetY, double scale)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            Color color = _color;

            if (_clicking)
            {
                color = _clickingColor;
            }

            SplashKit.FillRectangle(color, screenX, screenY, _width * scale, _height * scale);

            DrawingOptions bitmapOptions = SplashKit.OptionDefaults();
            bitmapOptions.ScaleX = (float)scale;
            bitmapOptions.ScaleY = (float)scale;

            SplashKit.DrawBitmap(_icon, screenX + _leftPad * scale - 1.5 / scale, screenY + (_height * scale - _icon.Height) / 2, bitmapOptions);

            SplashKit.DrawText(_text, Color.Black, Constants.FONT_PATH, (int)Math.Round(14 * scale), screenX + (_icon.Width + 10) * scale, screenY + 5 * scale);
        }

        public override bool PointWithin(double offsetX, double offsetY, double scale, Point2D point)
        {
            double screenX = offsetX + X * scale;
            double screenY = offsetY + Y * scale;

            return point.X >= screenX && point.X <= screenX + _width * scale && point.Y >= screenY && point.Y <= screenY + _height * scale;
        }
    }
}