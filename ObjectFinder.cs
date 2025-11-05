using SplashKitSDK;

namespace VLSEdit
{
    public static class ObjectFinder
    {
        public static bool MouseWithinBoxWidget(BoxWidget widget)
        {
            return widget.PointWithin(Editor.Instance.View.OffsetX, Editor.Instance.View.OffsetY, Editor.Instance.View.Scale, SplashKit.MousePosition());
        }

        public static bool MouseWithinNodeWidget(BoxWidget boxWidget, NodeWidget nodeWidget)
        {
            return nodeWidget.PointWithin(Editor.Instance.View.OffsetX + boxWidget.X * Editor.Instance.View.Scale, Editor.Instance.View.OffsetY + boxWidget.Y * Editor.Instance.View.Scale, Editor.Instance.View.Scale, SplashKit.MousePosition());
        }

        public static BoxWidget? FindTopBoxWidget()
        {
            BoxWidget? topBoxWidget = null;

            foreach (BoxWidget boxWidget in Editor.Instance.View.BoxWidgets)
            {
                if (!MouseWithinBoxWidget(boxWidget)) continue;

                topBoxWidget = boxWidget;
            }

            return topBoxWidget;
        }

        public static NodeWidget? FindNodeWidget(BoxWidget boxWidget)
        {
            foreach (NodeWidget nodeWidget in boxWidget.NodeWidgets)
            {
                if (MouseWithinNodeWidget(boxWidget, nodeWidget)) return nodeWidget;
            }

            return null;
        }

        public static ButtonWidget? FindToolbarButtonWidget()
        {
            foreach (ButtonWidget buttonWidget in Editor.Instance.View.Toolbar.Buttons)
            {
                if (buttonWidget.PointWithin(0, 0, 1, SplashKit.MousePosition()))
                {
                    return buttonWidget;
                }
            }

            return null;
        }
    }
}