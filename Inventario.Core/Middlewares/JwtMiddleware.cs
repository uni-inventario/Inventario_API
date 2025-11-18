using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Inventario.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Inventario.Core.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUsuarioRepository UsuarioRepository)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sub = context.User.FindFirst("id")?.Value;

                if (long.TryParse(sub, out var UsuarioId))
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    var incomingToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                                        ? authHeader[7..].Trim()
                                        : authHeader.Trim();

                    var responseValidation = await UsuarioRepository.CheckTokenAsync(UsuarioId, incomingToken);
                    if (!responseValidation)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token revogado ou inválido");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
