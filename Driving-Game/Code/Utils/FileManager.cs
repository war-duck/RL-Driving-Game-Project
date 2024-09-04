using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Persist;

public class FileManager
{
    public static void SaveObject(Object model, string path = "Saves/", string name = null)
    {
        name ??= model.GetType().Name;
        EncogDirectoryPersistence.SaveObject(new FileInfo(path + name + ".eg"), model);
    }
    public static Object LoadObject(string name, string path = "Saves/")
    {
        return EncogDirectoryPersistence.LoadObject(new FileInfo(path + name));
    }
    public static void SaveLine(string line, string path = "Saves/Logs/", string name = null)
    {
        name ??= "log";
        using (StreamWriter writer = new StreamWriter(path + name + ".txt", true))
        {
            writer.WriteLine(line);
        }
    }
    
}