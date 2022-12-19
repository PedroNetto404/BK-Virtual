using MediatR;

namespace LojaVirtual.Core.Integration;

public class ReverterEstoquePedidoRequest : IRequest<bool>
{
    public IEnumerable<Item> Items { get; set; }
}