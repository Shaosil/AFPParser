using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

[Serializable]
public class Options
{
    public string LastDirectory { get; set; }

    public static Options LoadSettings(string path)
    {
        Options loadedOpts = new Options();

        try
        {
            // Read if existing
            if (File.Exists(path))
                using (FileStream reader = File.OpenRead(path))
                    loadedOpts = (Options)new XmlSerializer(typeof(Options)).Deserialize(reader);

            // Save
            loadedOpts.SaveSettings(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not load settings. {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return loadedOpts;
    }

    public void SaveSettings(string path)
    {
        try
        {
            using (FileStream writer = File.OpenWrite(path))
                new XmlSerializer(typeof(Options)).Serialize(writer, this);
        }
        catch { }
    }
}