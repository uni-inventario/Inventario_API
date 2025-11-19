Inventario.Core

Essa biblioteca concentra toda a lógica de negócio do sistema de inventário.
Aqui ficam as entidades, DTOs, validações, handlers, repositórios, serviços, configurações e middlewares usados pela API.

O que tem no projeto;

    Models

    Ficam em Models/ e representam as entidades do domínio, como:

        Produto
        Estoque
        Usuario

----------------------------------------------------------------------------------------------------------

    DTOs

    Localizados em DTOs/, são usados para entrada e saída de dados da API.
    Incluem:

        ValidationResultDto
        Subpastas Requests/ e Responses/

----------------------------------------------------------------------------------------------------------

    Handlers

    Estão em Handlers/ e são responsáveis por orquestrar regras de negócio e chamar repositórios e serviços.
    Exemplos:

        ProdutoHandler
        EstoqueHandler
        UsuarioHandler

----------------------------------------------------------------------------------------------------------

    Repositories

    Em Repositories/, fazem o acesso aos dados de forma organizada e isolada.
    Exemplos:

        BaseRepository
        ProdutoRepository
        UsuarioRepository
        EstoqueRepository

----------------------------------------------------------------------------------------------------------

    Services

    Serviços auxiliares localizados em Services/.
    Exemplo:

        AuthService para autenticação e geração de tokens JWT

----------------------------------------------------------------------------------------------------------

    Validators

    Em Validators/, usam FluentValidation para validar DTOs e modelos:

        ProdutoValidator
        UsuarioValidator

----------------------------------------------------------------------------------------------------------

    Configurations

    Em Configurations/, ficam:

        Configuração do AutoMapper (AutoMapperConfiguration.cs)
        Configuração do JWT (JwtConfiguration.cs)

----------------------------------------------------------------------------------------------------------

    Middlewares

    Ficam em Middlewares/.
    Exemplo:

        JwtMiddleware para validar tokens recebidos no header

----------------------------------------------------------------------------------------------------------

    EF Core

        O DbContext está em Data/ContextRepository.cs.
        Migrations ficam na pasta Migrations/.

    
    Como rodar migrações

    No terminal:

        dotnet ef migrations add NomeDaMigracao 
        dotnet ef database update

----------------------------------------------------------------------------------------------------------

    AutoMapper

    Os perfis ficam em Configurations/AutoMapperConfiguration.cs.
    A configuração deve ser carregada no Program.cs da API.

----------------------------------------------------------------------------------------------------------


    JWT

    As configurações e geração de tokens estão centralizadas em:

        Configurations/JwtConfiguration.cs
        Services/AuthService.cs

----------------------------------------------------------------------------------------------------------

    Criando uma nova entidade

    Para adicionar algo novo ao sistema, o fluxo sugerido é:

        1. Criar o Model.
        2. Criar os DTOs (Request/Response).
        3. Criar o Validator.
        4. Criar o Repository.
        5. Criar o Handler.
        6. Registrar o AutoMapper para mapear DTO <-> Model.

----------------------------------------------------------------------------------------------------------

    Exemplo de fluxo 

        1. O controller recebe um ProdutoRequest.
        2. O ProdutoValidator valida a entrada.
        3. O ProdutoHandler aplica regras e chama o repositório.
        4. O ProdutoRepository persiste no banco.
        5. É retornado um ProdutoResponse com os dados gravados.