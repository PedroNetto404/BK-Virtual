using AutoMapper;
using LojaVirtual.Core.DTOs;
using LojaVirtual.Core.Handlers;
using LojaVirtual.Core.Integration;
using LojaVirtual.Core.NotificationError;
using LojaVirtual.Domain.Entities;
using LojaVirtual.Domain.Enums;
using LojaVirtual.Domain.Interfaces.Repositories;
using LojaVirtual.Infrastructure.Pagamentos.Facade;
using MediatR;

namespace LojaVirtual.Infrastructure.Pagamentos.Handlers;

public class PagamentoHandler :
    BaseHandler,
    IRequestHandler<RealizarPagamentoRequest, BaseResponse>
{
    private readonly IPagamentoFacade _pagamentoFacade;
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IPedidoRepository _pedidoRepository; 

    public PagamentoHandler(
        IMediator mediator,
        IMapper mapper,
        IPagamentoFacade pagamentoFacade,
        ITransacaoRepository transacaoRepository, 
        IPagamentoRepository pagamentoRepository,
        IPedidoRepository pedidoRepository) : base(mediator, mapper)
    {
        _pagamentoFacade = pagamentoFacade;
        _transacaoRepository = transacaoRepository;
        _pagamentoRepository = pagamentoRepository;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<BaseResponse> Handle(RealizarPagamentoRequest request, CancellationToken cancellationToken)
    {
        var pagamento = new Pagamento(request.PedidoId, request.Total, request.NomeCartao, request.NumeroCartao,
            request.ExpiracaoCartao,
            request.CodigoVerificadoCartao);

        var transacao = await _pagamentoFacade.RealizarPagamentoAsync(request.PedidoId, request.Total, pagamento);

        if (transacao.Status == StatusTransacao.Pago)
        {
            await _pagamentoRepository.AdicionarAsync(pagamento);
            await _transacaoRepository.AdicionarAsync(transacao);
            await _pagamentoRepository.UnityOfWork.SalvarAlteracoesAsync();
            return BaseResponse.Sucesso();
        }

        // enviar evento de pagamento recusado (voltar pedido carrinho e produtos no estoque)
        

        return BaseResponse.Erro();
    }
}