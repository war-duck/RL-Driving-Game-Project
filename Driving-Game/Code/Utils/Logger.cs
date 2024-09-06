using Encog.Neural.Networks;
using Godot;

public interface ILogger
{
    public void LogError(double actorError, double criticError);
    public void LogDistance(double distance);
    public void LogNetwork(BasicNetwork actor, BasicNetwork critic);
    public void Log(string message);
}
public static class Logger
{
    private static ILogger instance = new NullLogger();
    public static void SetLogger(ILogger logger)
    {
        instance = logger;
    }
    public static void LogError(double actorError, double criticError)
    {
        instance.LogError(actorError, criticError);
    }
    public static void LogDistance(double distance)
    {
        instance.LogDistance(distance);
    }
    public static void LogNetwork(BasicNetwork actor, BasicNetwork critic)
    {
        instance.LogNetwork(actor, critic);
    }
    public static void Log(string message)
    {
        instance.Log(message);
    }
}
public class ConsoleLogger : ILogger
{
    public void LogError(double actorError, double criticError)
    {
        Console.WriteLine("Actor Error: " + actorError + " Critic Error: " + criticError);
    }
    public void LogDistance(double distance)
    {
        Console.WriteLine("Distance: " + distance);
    }
    public void LogNetwork(BasicNetwork actor, BasicNetwork critic)
    {
    }
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
public class StandardLogger : ILogger
{
    public void LogError(double actorError, double criticError)
    {
        FileManager.SaveLine(string.Join(",", Time.GetDatetimeStringFromSystem(), actorError, criticError), name: DataLoader.Instance.GetAgentParamString() + "_error");
    }
    public void LogDistance(double distance)
    {
        FileManager.SaveLine(string.Join(",", Time.GetDatetimeStringFromSystem(), distance), name: DataLoader.Instance.GetAgentParamString() + "_distance");
    }
    public void LogNetwork(BasicNetwork actor, BasicNetwork critic)
    {
        var timestamp = Time.GetDatetimeStringFromSystem();
        FileManager.SaveObject(actor, "Saves/Models/", timestamp + "_" + DataLoader.Instance.GetAgentParamString() + "_actor");
        FileManager.SaveObject(critic, "Saves/Models/", timestamp + "_" + DataLoader.Instance.GetAgentParamString() + "_critic");
    }
    public void Log(string message)
    {
        Console.WriteLine(message);
        FileManager.SaveLine(message, name: DataLoader.Instance.GetAgentParamString() + "_log");
    }
}
public class NullLogger : ILogger
{
    public void LogError(double actorError, double criticError) { }
    public void LogDistance(double distance) { }
    public void LogNetwork(BasicNetwork actor, BasicNetwork critic) { }
    public void Log(string message) { }
}