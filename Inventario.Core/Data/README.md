Inventario.Core.Data

Propósito:
    Contém a implementação do DbContext e classes auxiliares para acesso a dados.

Arquivos importantes:
    ContextRepository.cs - Implementação do DbContext com DbSet<T> para Produtos, Estoques, Usuarios.
    ContextFactory.cs - Helper para criação do contexto (útil para migrações e testes).

Notas sobre migrações:
    Migrations geradas estão em Migrations/ na raiz do projeto Inventario.Core.
    Use os comandos dotnet ef especificando project conforme necessário.