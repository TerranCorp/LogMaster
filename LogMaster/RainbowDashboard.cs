using System.Diagnostics;

public class RainbowDashboard
{
    private readonly LogMaster.LogMaster _logger;
    private readonly string _sourceFilePath;
    private readonly int _updateDelayMs;
    private readonly int _sourceInterval;
    private readonly bool _waitForKey;

    public RainbowDashboard(LogMaster.LogMaster logger, string sourceFilePath, int updateDelayMs = 100, int sourceInterval = 50, bool waitForKey = true)
    {
        _logger = logger;
        _sourceFilePath = sourceFilePath;
        _updateDelayMs = updateDelayMs;
        _sourceInterval = sourceInterval;
        _waitForKey = waitForKey;
    }

    public void Run()
    {
        int counter = 0;
        int elapsedMs = 0;
        var currentProcess = Process.GetCurrentProcess();
        var rand = new Random();

        Console.CursorVisible = false;

        bool stop = false;

        while (!stop)
        {
            // Dynamic system info
            double cpuUsage = GetCpuUsage(currentProcess, 50);
            long memoryMB = currentProcess.WorkingSet64 / 1024 / 1024;
            int threads = currentProcess.Threads.Count;
            int eventNum = rand.Next(1000);

            string dynamicInfo = $"[{DateTime.Now:HH:mm:ss}] CPU: {cpuUsage:F1}% | Mem: {memoryMB}MB | Threads: {threads} | Event: {eventNum}";

            _logger.RainbowAnimateLogFrame(dynamicInfo);

            counter++;
            elapsedMs += _updateDelayMs;

            // Print ASCII rainbow header every 20 seconds
            if (elapsedMs >= 20000)
            {
                for (int i = 0; i < 20; i++)
                {
                    _logger.PrintRainbowAsciiHeader();
                    Thread.Sleep(50);
                }
                elapsedMs = 0;
            }

            // Periodically print source code
            if (counter % _sourceInterval == 0 && File.Exists(_sourceFilePath))
            {
                string[] lines = File.ReadAllLines(_sourceFilePath);
                foreach (var line in lines)
                {
                    _logger.RainbowAnimateLogFrame(line);
                    Thread.Sleep(5); // small delay for visible rainbow effect
                }
            }

            // Check for key press to stop
            if (_waitForKey && Console.KeyAvailable)
            {
                Console.ReadKey(true);
                stop = true;
            }

            // Delay for smooth updates
            Thread.Sleep(_updateDelayMs);
        }

        Console.CursorVisible = true;
    }

    /// <summary>
    /// Estimates CPU usage of the current process (cross-platform)
    /// </summary>
    private static double GetCpuUsage(Process process, int sampleDelayMs = 100)
    {
        TimeSpan startCpu = process.TotalProcessorTime;
        DateTime startTime = DateTime.UtcNow;

        Thread.Sleep(sampleDelayMs);

        TimeSpan endCpu = process.TotalProcessorTime;
        DateTime endTime = DateTime.UtcNow;

        double cpuUsedMs = (endCpu - startCpu).TotalMilliseconds;
        double totalMs = Environment.ProcessorCount * (endTime - startTime).TotalMilliseconds;

        return (cpuUsedMs / totalMs) * 100;
    }


}
