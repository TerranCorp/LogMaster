// See https://aka.ms/new-console-template for more information


LogMaster.LogMaster logMaster = new LogMaster.LogMaster();

logMaster.IsConsoleLogEnabled = true;
logMaster.IsTimeStampEnabled1 = true;
logMaster.IsLineNumberEnabled = true;
logMaster.IsMethodNameEnabled = true;
logMaster.IsFileNameEnabled = true;


//logMaster.RainbowAnimateLogSmooth("Hi my name is Christopher Robert Mitchell", LogLevel.Info, 150, 1, true);

string sourceFile = Path.Combine(AppContext.BaseDirectory, "LogMaster.cs");

RainbowDashboard dashboard = new RainbowDashboard(logMaster, sourceFile);
dashboard.Run();