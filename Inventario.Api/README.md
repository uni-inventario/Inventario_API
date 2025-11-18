Inventario.Api

Este projeto é a API REST do sistema de inventário, criada em ASP.NET Core.
É ela que expõe os endpoints para autenticação, produtos, usuários e estoque.

O que tem no projeto

Controllers: ficam na pasta Controllers/ e incluem:

    AuthController

    ProdutoController

    EstoqueController

    UsuarioController

    BaseController

Configurações:

    Todas as configurações principais ficam no appsettings.json.

    As definições do JWT estão em Inventario.Core/Configurations/JwtConfiguration.cs.

Como rodar o projeto:

    Pelo terminal

    Entre na pasta da API e rode:

    cd Inventario.Api
    dotnet run


Ou, a partir da raiz da solução:

    dotnet run --project Inventario.Api/Inventario.Api.csproj

Com Docker:

    Se seu docker-compose.yml estiver configurado, execute:

    docker-compose up --build

Endpoints principais:

    /api/auth → login e logout

    /api/produtos → CRUD de produtos

    /api/estoques → CRUD de estoques

    /api/usuarios → operações relacionadas a usuários

As rotas exatas e métodos HTTP estão definidas diretamente nos controllers via atributos como [Route] e [HttpGet/Post/Put/Delete].

Middlewares:

    A API utiliza middlewares localizados em Inventario.Core/Middlewares, como o JwtMiddleware, responsável por validar o token de autenticação.

Configurações do ambiente:

    Use o appsettings.json conforme o ambiente (Development, Production etc.).

Criando um novo controller

    Adicione uma classe na pasta Controllers/.

    Se fizer sentido, herde de BaseController.

    Injete os serviços necessários pelo construtor.
