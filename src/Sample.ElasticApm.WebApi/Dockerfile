FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sample.Elasticsearch.WebApi/Sample.Elasticsearch.WebApi.csproj", "Sample.Elasticsearch.WebApi/"]
COPY ["Sample.Elasticsearch.WebApi.Core/Sample.Elasticsearch.WebApi.Core.csproj", "Sample.Elasticsearch.WebApi.Core/"]
COPY ["Sample.Elasticsearch.Domain/Sample.Elasticsearch.Domain.csproj", "Sample.Elasticsearch.Domain/"]
RUN dotnet restore "Sample.Elasticsearch.WebApi/Sample.Elasticsearch.WebApi.csproj"
COPY . .
WORKDIR "/src/Sample.Elasticsearch.WebApi"
RUN dotnet build "Sample.Elasticsearch.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Elasticsearch.WebApi.csproj" -c Release -o /app/publish

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Elasticsearch.WebApi.dll"]