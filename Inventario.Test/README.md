Inventario.Test

Descrição
    Projeto com os testes unitários e de integração do sistema de inventário.
    Cobertura típica: Handlers e Repositories (ex.: ProdutoHandlerTest, UsuarioHandlerTest, EstoqueHandlerTest, BaseRepositoryTests).

Requisitos
    .NET SDK 8.0+ instalado.
    Recomenda-se usar o Visual Studio.


Executando todos os testes

    cd Inventario.Test
    dotnet test


Executando a solução inteira (a partir da raiz)

    dotnet test


Executando um teste específico (filtro por nome)

    dotnet test --filter FullyQualifiedName~Inventario.Test.Repositories.BaseRepositoryTests.AddAsync_DeveAdicionarEntidade


Sobre os testes de repositório:
    Os testes de repositório usam o provedor InMemory do EF Core para isolar a camada de persistência. Padrão usado:

    var options = new DbContextOptionsBuilder<ContextRepository>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

    using var context = new ContextRepository(options);


    Use sempre um nome de banco diferente para garantir isolamento entre testes.

    Boas práticas ao escrever novos testes:
    - Mantenha testes pequenos e determinísticos (arrange-act-assert).
    - Use `async Task` em vez de `void` para testes assíncronos e `await` as chamadas assíncronas.
    - Cada teste deve montar seu próprio contexto de dados.
    - Para dependências externas, use mocks (Moq ou similares) e injete as dependências no handler/service.

    Como adicionar um novo teste
        1. Criar uma nova classe em Inventario.Test/Handlers ou Inventario.Test/Repositories.
        2. Nomeie o método de teste com padrão claro Metodo_EstadoEsperado_ResultadoEsperado (ex.: AddAsync_DeveAdicionarEntidade).
        3. Use [Fact] para testes unitários.

    Depurando testes
        - No Visual Studio: clique com o botão direito no teste e escolha "Debug Test".
        - No VS Code: configure um launch.json para anexar ao processo de teste ou use a extensão .NET Test Explorer para executar e depurar testes.

    Integração contínua (CI)
        - Em pipelines , adicione passos para executar `dotnet test`.


    Lidando com falhas comuns
        - Falha por DbUpdateConcurrencyException ou estado inesperado: verifique se cada teste cria seu próprio contexto InMemory.
        - Falha por dependência externa: substitua por mock e valide apenas a lógica.
