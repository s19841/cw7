﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Cw7.Models;
using Cw7.Services;

namespace Cw7.Middlewares
{
    public class LogMiddleware
    {
        private readonly ILogService _logger;
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next, ILogService logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request != null)
            {
                context.Request.EnableBuffering();
                var method = context.Request.Method;
                var path = context.Request.Path;
                var queryString = context.Request.QueryString.ToString();
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                _logger.Log(new LogData { Method = method, Path = path, Body = body, QueryString = queryString });
            }

            if (_next != null) await _next(context);
        }
    }
}