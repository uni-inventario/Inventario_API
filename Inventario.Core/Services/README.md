Inventario.Core.Services

Propósito:
Implementar funcionalidades transversais, por exemplo AuthService para autenticação, geração e validação de tokens JWT.

Como usar:
    Injetar os serviços via DI no Program.cs e nos controllers/handlers que precisarem.

Exemplo (AuthService):
    Recebe credenciais, valida o usuário (via UsuarioRepository) e gera token JWT com dados do JwtConfiguration.