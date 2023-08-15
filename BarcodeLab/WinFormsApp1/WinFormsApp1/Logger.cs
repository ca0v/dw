using System;
using System.Diagnostics;

public class Logger
{
    private readonly string AppName = "Barcode_lab";

    public void Log(string message)
    {
        EventLog.WriteEntry("Application", $"{AppName}: {message}", EventLogEntryType.Information, 1001);
    }

    public void Error(Exception ex)
    {
        EventLog.WriteEntry("Application", $"{AppName}: {ex.Message}", EventLogEntryType.Error, 1001);
    }

    public void AppStarted()
    {
        EventLog.WriteEntry("Application", $"{AppName} started", EventLogEntryType.Information, 1001);
    }

    public void AppStopped()
    {
        EventLog.WriteEntry("Application", $"{AppName} stopped", EventLogEntryType.Information, 1001);
    }

    internal void Verbose(string message)
    {
        EventLog.WriteEntry("Application", $"{AppName}: {message}", EventLogEntryType.Information, 1001);
    }
}