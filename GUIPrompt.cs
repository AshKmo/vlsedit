using System.Reflection.Metadata;
using SplashKitSDK;

namespace VLSEdit
{
    static class GUIPrompt
    {
        public static string? Ask(string question)
        {
            float startX = SplashKit.CurrentWindow().Width / 2 - 300;
            float startY = SplashKit.CurrentWindow().Height / 2 - 50;

            Rectangle rect = SplashKit.RectangleFrom(startX + 20, startY + 50, 380, 100);

            SplashKit.StartReadingText(rect);

            while (SplashKit.CurrentWindow().ReadingText() && !SplashKit.CurrentWindow().CloseRequested)
            {
                SplashKit.ProcessEvents();

                SplashKit.FillRectangle(Color.White, startX, startY, 600, 100);

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

        public static void ShowHelp(string title, string content)
        {
            if (!content.EndsWith("\n"))
            {
                content += "\n";
            }

            content = content + "\nPress any button to continue";

            float startX = SplashKit.CurrentWindow().Width / 2 - 500;
            float startY = SplashKit.CurrentWindow().Height / 2 - 300;

            while (!SplashKit.CurrentWindow().CloseRequested)
            {
                SplashKit.ProcessEvents();

                if (SplashKit.AnyKeyPressed() || SplashKit.MouseClicked(MouseButton.LeftButton) || SplashKit.MouseClicked(MouseButton.RightButton) || SplashKit.MouseClicked(MouseButton.MiddleButton)) break;

                SplashKit.FillRectangle(Color.White, startX, startY, 1000, 600);

                SplashKit.DrawText(title, Color.Black, Constants.FONT_ROBOTO, 30, startX + 20, startY + 20);

                float contentY = startY + 65;
                foreach (string line in content.Split("\n"))
                {
                    SplashKit.DrawText(line, Color.Black, Constants.FONT_ROBOTO, 20, startX + 20, contentY);
                    contentY += 30;
                }

                SplashKit.CurrentWindow().Refresh(Constants.FRAMERATE);
            }
        }
    }
}