Inventario.Core.Handlers

Propósito:
    Encapsular cenários e regras de negócio. Ex.: ProdutoHandler, UsuarioHandler, EstoqueHandler.

Responsabilidades:
    Validar entradas (depois do validador), aplicar regras, orquestrar chamadas a repositórios e serviços, e preparar DTOs de resposta.

Como testar:
    Criar mocks para repositórios e serviços, instanciar o handler e chamar métodos diretamente para validar retornos e efeitos colaterais.
