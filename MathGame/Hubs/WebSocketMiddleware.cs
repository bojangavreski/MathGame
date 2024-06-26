﻿using Microsoft.IdentityModel.Tokens;

namespace MathGame.API.Hubs;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (request.Query.TryGetValue("access_token", out var accessToken) && request.Headers.Authorization.IsNullOrEmpty())
        {
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
        }

        await _next(httpContext);
    }
}
