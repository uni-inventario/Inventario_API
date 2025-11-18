Inventario_API


    Visão geral:
        - Projeto de exemplo para gerenciamento de inventário em ASP.NET Core com arquitetura em camadas.
        - Contém 3 projetos: Inventario.Api (API Web), Inventario.Core (lógica de negócio, repositórios, handlers, DTOs) e Inventario.Test (testes unitários).


    Requisitos:
        - .NET SDK 8.0+ .
        - Docker e Docker Compose para rodar em contêineres (opcional).


    Executando localmente:
    
        - Restaurar dependências e compilar:
            dotnet restore
            dotnet build

        - Rodar a API:
            dotnet run --project Inventario.Api\Inventario.Api.csproj


    Executando com Docker Compose (se preferir):

        docker-compose up --build


    Testes:

        dotnet test Inventario.Test\Inventario.Test.csproj


    Estrutura de pastas:

        - Inventario.Api/ - Entrypoint web, controllers e configurações.
        - Inventario.Core/ - Lógica de negócio, DTOs, Repositories, Services, Handlers, Validators.
        - Inventario.Test/ - Testes unitários.