namespace VLSEdit
{
    public static class ScriptLoader
    {
        public static Script LoadScript(string scriptPath)
        {
            StreamReader? streamReader;

            try
            {
                streamReader = new StreamReader(scriptPath);
            }
            catch
            {
                return new Script();
            }

            Script result = ParseScript(streamReader.ReadToEnd());

            streamReader.Close();

            return result;
        }
        
        public static Script ParseScript(string scriptText)
        {
            Script script = new Script();

            StringReader reader = new StringReader(scriptText);

            // TODO: parsing stuff

            reader.Close();

            return script;
        }
    }
}