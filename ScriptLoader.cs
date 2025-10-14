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
                throw new Exception($"script file {scriptPath} not found");
            }

            Script result = ParseScript(streamReader.ReadToEnd());

            streamReader.Close();

            return result;
        }

        public static Script ParseScript(string scriptText)
        {
            Script script = new Script();

            StringReader reader = new StringReader(scriptText);

            script.Deserialise(reader);

            reader.Close();

            return script;
        }

        public static void SaveScript(string path, Script script)
        {
            StreamWriter writer = new StreamWriter(path);

            writer.Write(UnparseScript(script));

            writer.Close();
        }
        
        public static string UnparseScript(Script script)
        {
            StringWriter writer = new StringWriter();

            script.Serialise(writer);

            return writer.ToString();
        }
    }
}