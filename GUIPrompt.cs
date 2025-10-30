using System.Reflection.Metadata;
using SplashKitSDK;

namespace VLSEdit
{
    static class GUIPrompt
    {
        public static string? Ask(string question)
        {
            float startX = SplashKit.CurrentWindow().Width / 2 - 200;
            float startY = SplashKit.CurrentWindow().Height / 2 - 50;

            Rectangle rect = SplashKit.RectangleFrom(startX + 20, startY + 50, 380, 100);

            SplashKit.StartReadingText(rect);

            while (SplashKit.CurrentWindow().ReadingText())
            {
                SplashKit.ProcessEvents();

                SplashKit.FillRectangle(Color.White, startX, startY, 400, 100);

                SplashKit.DrawText(question, Color.Black, Constants.FONT_ROBOTO, 20, startX + 20, startY + 20);

                if (SplashKit.ReadingText())
                {
                    SplashKit.DrawCollectedText(Color.Black, Constants.FONT_ROBOTO, 18, SplashKit.OptionDefaults());
                }

                SplashKit.CurrentWindow().Refresh(Constants.FRAMERATE);
            }

            if (SplashKit.TextEntryCancelled()) return null;

            return SplashKit.TextInput();
        }
    }
}