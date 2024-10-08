using DevStore.Core.Messages;
using FluentValidation.Results;
using MediatR;
using System.Threading.Tasks;

namespace DevStore.Core.Mediator
{
    public class MediatorHandler(IMediator mediator) : IMediatorHandler
    {
        public async Task<ValidationResult> SendCommand<T>(T comando) where T : Command
        {
            return await mediator.Send(comando);
        }

        public async Task PublishEvent<T>(T evento) where T : Event
        {
            await mediator.Publish(evento);
        }
    }
}