using SplashKitSDK;

namespace VLSEdit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please use a sub-command, e.g. 'vlsedit run x.vls' or 'vlsedit edit x.vls'");
                return;
            }

            switch (args[0])
            {
                case "edit":
                    {
                        string scriptPath = args[1];

                        Editor.Instance.OpenScript(scriptPath);

                        Window window = new Window("VLSEdit: " + scriptPath, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);

                        SplashKit.WindowSetIcon(window, SplashKit.LoadBitmap("icon", "icons/icon.svg"));

                        while (!window.CloseRequested)
                        {
                            SplashKit.ProcessEvents();

                            Editor.Instance.Tick();

                            SplashKit.RefreshScreen(Constants.FRAMERATE);
                        }

                        window.Close();
                    }
                    break;

                case "run":
                    {
                        if (Runner.Instance.OpenScript(args[1]))
                        {
                            Runner.Instance.Run();
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Sub-command must be either 'run' or 'edit'");
                    return;
            }
        }
    }
}
