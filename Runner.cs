namespace VLSEdit
{
    public class Runner
    {
        private static Runner? _instance = null;

        private Script? _script = null;

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

        private Runner()
        {
        }

        public void OpenScript(string scriptPath)
        {
            _script = ScriptLoader.LoadScript(scriptPath);
        }

        public void Run()
        {
            if (_script == null)
            {
                throw new Exception("no script has been loaded");
            }

            foreach (Box box in _script.Boxes)
            {
                if (box is not StartBox) continue;

                ((StartBox)box).Trigger(new NullValue());
            }
        }
    }
}