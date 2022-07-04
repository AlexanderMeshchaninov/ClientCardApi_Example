#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Lessons.ClientCardApi.Runner/Lessons.ClientCardApi.Runner.csproj", "Lessons.ClientCardApi.Runner/"]
COPY ["Lessons.ClientCardApi.NuGet.Serilog/Lessons.ClientCardApi.NuGet.Serilog.csproj", "Lessons.ClientCardApi.NuGet.Serilog/"]
COPY ["Lessons.ClientCardApi.UnitOfWork/Lessons.ClientCardApi.UnitOfWork.csproj", "Lessons.ClientCardApi.UnitOfWork/"]
COPY ["Lessons.ClientCardApi.Data.Repository/Lessons.ClientCardApi.Data.Repository.csproj", "Lessons.ClientCardApi.Data.Repository/"]
COPY ["Lessons.ClientCardApi.Data.Context/Lessons.ClientCardApi.Data.Context.csproj", "Lessons.ClientCardApi.Data.Context/"]
COPY ["Lessons.ClientCardApi.Core/Lessons.ClientCardApi.Core.csproj", "Lessons.ClientCardApi.Core/"]
COPY ["Lessons.ClientCardApi.NuGet.Identity/Lessons.ClientCardApi.NuGet.Identity.csproj", "Lessons.ClientCardApi.NuGet.Identity/"]
COPY ["Lessons.ClientCardApi.NuGet.AutoMapper/Lessons.ClientCardApi.NuGet.AutoMapper.csproj", "Lessons.ClientCardApi.NuGet.AutoMapper/"]
COPY ["Lessons.ClientCardApi.NuGet.JwtBearer/Lessons.ClientCardApi.NuGet.JwtBearer.csproj", "Lessons.ClientCardApi.NuGet.JwtBearer/"]
COPY ["Lessons.ClientCardApi.Data.AuthContext/Lessons.ClientCardApi.Data.AuthContext.csproj", "Lessons.ClientCardApi.Data.AuthContext/"]
COPY ["Lessons.ClientCardApi.NuGet.Nest/Lessons.ClientCardApi.NuGet.Nest.csproj", "Lessons.ClientCardApi.NuGet.Nest/"]
COPY ["Lessons.ClientCardApi.NuGet.Consul/Lessons.ClientCardApi.NuGet.Consul.csproj", "Lessons.ClientCardApi.NuGet.Consul/"]
COPY ["Migrations/Migrations.csproj", "Migrations/"]
COPY ["Lessons.ClientCardApi.Nuget.Metrics/Lessons.ClientCardApi.Nuget.Metrics.csproj", "Lessons.ClientCardApi.Nuget.Metrics/"]
COPY ["AuthMigrations/AuthMigrations.csproj", "AuthMigrations/"]
COPY ["Lessons.ClientCardApi.CommonLogic.Facades/Lessons.ClientCardApi.CommonLogic.Facades.csproj", "Lessons.ClientCardApi.CommonLogic.Facades/"]
COPY ["Lessons.ClientCardApi.NuGet.FluentValidator/Lessons.ClientCardApi.NuGet.FluentValidator.csproj", "Lessons.ClientCardApi.NuGet.FluentValidator/"]
RUN dotnet restore "Lessons.ClientCardApi.Runner/Lessons.ClientCardApi.Runner.csproj"
COPY . .
WORKDIR "/src/Lessons.ClientCardApi.Runner"
RUN dotnet build "Lessons.ClientCardApi.Runner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lessons.ClientCardApi.Runner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lessons.ClientCardApi.Runner.dll"]