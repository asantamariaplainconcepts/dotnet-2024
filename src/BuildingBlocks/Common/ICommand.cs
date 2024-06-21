namespace BuildingBlocks.Common;

public interface IBaseCommand 
{
}

public interface ICommand : IRequest, IBaseCommand
{
}

public interface ICommand<T> : IRequest<T>, IBaseCommand where T : IAppResult
{
}