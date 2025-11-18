Inventario.Core.Repositories

Propósito:
    Abstrair acesso a dados. O BaseRepository<T> contém operações comuns (Add, Update, GetById, AddRange, UpdateRange).

Como usar:
    Injetar IRepository<T> (ou a implementação concreta) nos handlers ou services.
    Para adicionar um repositório customizado, criar classe que herde de BaseRepository<T> e implementar métodos específicos quando necessário.

Testes:
    Os testes de repositório usam InMemoryDatabase para validar comportamento (ver Inventario.Test/Repositories).

Observações de implementação:
    BaseRepository deve filtrar entidades com DeletedAt != null quando apropriado; confira testes que verificam comportamento quando DeletedAt está setado.