using ImanSoftware.Framework.Abstractions.Jwt;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Framework.AspNetCore.Middlewares;

/// <summary>
/// Middleware to extract JWT token from Authorization header and set the user in HttpContext.
/// </summary>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtTokenService _tokenService;

    public JwtMiddleware(RequestDelegate next, IJwtTokenService tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromHeader(context);
        if (!string.IsNullOrEmpty(token))
        {
            var validationResult = _tokenService.ValidateToken(token);
            if (validationResult.IsValid && validationResult.Principal is not null)
            {
                context.User = validationResult.Principal;
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var parts = authHeader.ToString().Split(' ');
            if (parts.Length == 2 && parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                return parts[1];
        }
        return null;
    }
}