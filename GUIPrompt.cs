using SplashKitSDK;

namespace VLSEdit
{
    static class GUIPrompt
    {
        public static string Ask(string question)
        {
            Font font = SplashKit.LoadFont(Constants.FONT_NAME, Constants.FONT_PATH);

            float startX = SplashKit.CurrentWindow().Width / 2 - 200;
            float startY = SplashKit.CurrentWindow().Height / 2 - 50;

            Rectangle rect = SplashKit.RectangleFrom(startX + 20, startY + 50, 380, 100);

            SplashKit.StartReadingText(rect);

            while (SplashKit.CurrentWindow().ReadingText())
            {
                SplashKit.ProcessEvents();

                SplashKit.FillRectangle(Color.White, startX, startY, 400, 100);

                SplashKit.DrawText(question, Color.Black, "roboto.ttf", 20, startX + 20, startY + 20);

                if (SplashKit.ReadingText())
                {
                    SplashKit.DrawCollectedText(Color.Black, font, 18, SplashKit.OptionDefaults());
                }

                SplashKit.CurrentWindow().Refresh(Constants.FRAMERATE);
            }

            return SplashKit.TextInput();
        }
    }
}