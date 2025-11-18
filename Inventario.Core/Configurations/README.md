Inventario.Core.Configurations

Propósito:
    Centralizar configurações reutilizáveis: AutoMapper, JWT e outras configurações de infraestrutura.

Arquivos importantes:
    AutoMapperConfiguration.cs - Define mapeamentos entre Models e DTOs.
    JwtConfiguration.c - Define parâmetros do JWT (chave secreta, emissor, validade).

Como usar:
    Registrar AutoMapper no Program.cs da API chamando as configurações desta pasta.
    Configurar IConfiguration para ler valores sensíveis (ex.: chave JWT) de variáveis de ambiente ou appsettings.{Environment}.json.