using Microsoft.Data.SqlClient;
using Polly;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class SqlRetryHelper
{
    private static readonly int[] TransientSqlErrorNumbers = new[] { 40613, 40197, 40501 };

    public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
    {
        var retryPolicy = Policy
            .Handle<SqlException>(ex => TransientSqlErrorNumbers.Contains(ex.Number))
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Log opcional
                    Console.WriteLine($"Tentativa {retryCount} falhou. Retentando em {timeSpan.TotalSeconds} segundos...");
                });

        return await retryPolicy.ExecuteAsync(action);
    }
}
