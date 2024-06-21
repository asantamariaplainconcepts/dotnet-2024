using BuildingBlocks.Common;

namespace Workers.Domain;

public class Worker:  BaseEntity
{
    public Worker(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }
}