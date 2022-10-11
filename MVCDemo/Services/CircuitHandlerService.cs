using Demo.Shared.Interfaces;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Threading;
using System.Threading.Tasks;

namespace MVCDemo.Services
{
    /// <summary>
    /// Initial code for CircuitHandlerService from sample code for
    /// Episode 72 of BlazorTrain.  See note in readme.
    /// </summary>
    public class CircuitHandlerService : CircuitHandler
    {
        public string CircuitId { get; private set; } = string.Empty;

        IBlazorUserService _userService;

        public CircuitHandlerService(IBlazorUserService userService)
        {
            _userService = userService;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            CircuitId = circuit.Id;
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _userService.Disconnect(circuit.Id);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
