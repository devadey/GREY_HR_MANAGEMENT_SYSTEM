global using System;
global using WebApi;
global using System.Linq;
global using Application;
global using Infrastructure;
global using Common.Authorization;
global using Infrastructure.Models;
global using Infrastructure.Context;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.DependencyInjection;


global using Application.Features.Identity.Users.Commands;
global using Application.Features.Identity.Users.Queries;
global using Application.Features.Identity.Token.Queries;
global using Application.Services.Identity;
global using Common.Requests.Identity;
global using MediatR;


global using Application.Features.Employees.Commands;
global using Application.Features.Employees.Queries;
global using Common.Requests.Employee;
global using WebApi.Attributes;


global using Application.AppConfigs;
global using Common.Responses.Wrappers;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc.Authorization;
global using Microsoft.Extensions.Configuration;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Newtonsoft.Json;
global using System.Collections.Generic;
global using System.Net;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;
global using WebApi.Permissions;