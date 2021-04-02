using System.Threading.Tasks;

namespace AzureFunctions.Infrastructure.Commands
{
    public interface ICommand<TResult>
    {
    }

    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        Task<TResult> Execute(TCommand command);
    }
}
