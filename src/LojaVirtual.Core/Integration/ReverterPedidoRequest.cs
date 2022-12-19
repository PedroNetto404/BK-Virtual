using LojaVirtual.Core.DTOs;
using MediatR;

namespace LojaVirtual.Core.Integration;

public class ReverterPedidoRequest : IRequest<BaseResponse>
{
    public Guid PedidoId { get; set; }
}