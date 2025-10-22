using System.Diagnostics;

namespace VLSEdit
{
    public class Runner
    {
        private static Runner? _instance = null;

        private Script _script = new Script();

        public static Runner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Runner();
                }

                return _instance;
            }
        }

        public Script Script { get { return _script; } }

        private Runner()
        {
        }

        public bool OpenScript(string scriptPath)
        {
            try
            {
                _script = ScriptLoader.LoadScript(scriptPath);
            }
            catch
            {
                Console.WriteLine($"Script {scriptPath} could not be loaded");

                return false;
            }

            return true;
        }

        public void Run()
        {
            foreach (Box box in _script.Boxes)
            {
                if (box is not StartBox) continue;

                ((StartBox)box).Trigger(new NullValue());
            }
        }
    }
}