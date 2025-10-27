using SplashKitSDK;

namespace VLSEdit
{
    public static class Extensions
    {
        public static bool IsEqualTo(this Point2D a, Point2D b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
    }
}