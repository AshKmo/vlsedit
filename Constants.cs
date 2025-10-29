using SplashKitSDK;

namespace VLSEdit
{
    public static class Constants
    {
        public static int WINDOW_WIDTH = 1800;

        public static int WINDOW_HEIGHT = 900;

        public static double BOX_WIDTH = 200;

        public static double BOX_TITLE_HEIGHT = 25;

        public static double BOX_OUTLINE_WIDTH = 3;

        public static double NODE_HEIGHT = 25;

        public static double NODE_WIDTH = 90;

        public static double TOOLBAR_HEIGHT = 35;

        public static int INITIAL_BOX_COLUMN_COUNT = 4;

        public static double HIGH_DETAIL_SCALE = 0.5;

        public static Color BACKGROUND_COLOR = SplashKit.RGBColor(0.2, 0.2, 0.2);

        public static Color TOOLBAR_COLOR = Color.LightGray;

        public static Color TOOLBAR_BUTTON_COLOR = SplashKit.RGBColor(0.6, 0.6, 0.6);

        public static Color TOOLBAR_BUTTON_COLOR_CLICKING = Color.Plum;

        public static Color DRAG_LINE_COLOR = Color.Red;

        public static Color BOX_BACKGROUND_COLOR = Color.Gray;

        public static Color BOX_OUTLINE_COLOR = Color.White;

        public static Color LINK_LINE_COLOR = Color.White;

        public static Color TRANSPARENT_COLOR = SplashKit.RGBAColor(0, 0, 0, 0);

        public static string FONT_NAME = "Roboto";

        public static string FONT_PATH = "roboto.ttf";

        public static uint FRAMERATE = 60;

        public static Bitmap BUTTON_DELETE_BITMAP = SplashKit.LoadBitmap("delete", "icons/delete.png");

        public static Bitmap BUTTON_CLONE_BITMAP = SplashKit.LoadBitmap("clone", "icons/clone.png");

        public static Bitmap BUTTON_SETVALUE_BITMAP = SplashKit.LoadBitmap("setvalue", "icons/setvalue.png");
    }
}