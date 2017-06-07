using System;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Options
{
    private static XmlSerializer serializer = new XmlSerializer(typeof(Options));

    public string LastDirectory { get; set; }
    public string LastOpenedFile { get; set; }

    Options()
    {
        LastDirectory = Environment.CurrentDirectory;
    }

    public static Options LoadSettings(string path)
    {
        Options loadedOpts = new Options();

        try
        {
            // Read if existing
            if (File.Exists(path))
                using (FileStream reader = File.OpenRead(path))
                    loadedOpts = (Options)(serializer.Deserialize(reader));

            // Save
            loadedOpts.SaveSettings(path);
        }
        catch { }

        return loadedOpts;
    }

    public void SaveSettings(string path)
    {
        try
        {
            File.WriteAllText(path, string.Empty);

            using (FileStream writer = File.OpenWrite(path))
                serializer.Serialize(writer, this);
        }
        catch { }
    }
}