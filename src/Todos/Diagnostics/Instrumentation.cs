using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Todos.Diagnostics;

public static class Instrumentation
{
    public const string ServiceName = "todos-app";

    private static readonly Meter Meter = new(ServiceName);
    
    public static class Todos
    {
        public static Counter<long> Created = Meter.CreateCounter<long>("todos.created");
        public static Counter<long> Completed= Meter.CreateCounter<long>("todos.completed");
    }
    
    public static ActivitySource Source = new ActivitySource(ServiceName);
}
