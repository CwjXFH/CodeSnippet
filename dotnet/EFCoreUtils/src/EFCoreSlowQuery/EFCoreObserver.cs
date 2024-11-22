using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EFCoreSlowQuery;

internal class EFCoreObserver(SlowQueryObserver slowQueryObserver) : IObserver<DiagnosticListener>
{
    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == DbLoggerCategory.Name)
        {
            value.Subscribe(slowQueryObserver);
        }
    }

    #region ignores

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    #endregion
}

internal class SlowQueryObserver(ILogger logger, EFCoreSlowQueryOptions options) : IObserver<KeyValuePair<string, object?>>
{
    public void OnError(Exception error)
    {
        logger.LogError(error, "An exception occurred.");
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key == RelationalEventId.CommandExecuted.Name
            && value.Value is CommandExecutedEventData eventData
            && eventData.Duration.Milliseconds > options.SlowQueryThresholdMilliseconds)
        {
            if (options.RecordSlowQueryLog)
            {
                RecordSlowQueryLog(eventData);
            }
        }
        else if (value.Key == RelationalEventId.CommandError.Name)
        {
            RecordErrorCommand(value.Value);
        }
    }

    #region private

    private void RecordSlowQueryLog(CommandExecutedEventData eventData)
    {
        const string msg = "[EFCoreSlowQuery] duration: {Duration}ms, service: {Service}, SQL: {SQL}";
        logger.Log(options.LogLevel, msg, eventData.Duration.Milliseconds, options.ServiceName, eventData.Command.CommandText);
    }

    private void RecordErrorCommand(object? value)
    {
        if (value is CommandErrorEventData errorEventData)
        {
            logger.LogError(errorEventData.Exception, "Exec SQL error, SQL: {SQL}", errorEventData.Command.CommandText);
        }
        else
        {
            logger.LogError("Exec SQL error, and no SQL is captured.");
        }
    }

    #endregion

    #region ignores

    public void OnCompleted()
    {
    }

    #endregion
}
