FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Sample.Elasticsearch.WebApi/Sample.Elasticsearch.WebApi.csproj", "src/Sample.Elasticsearch.WebApi/"]
COPY ["src/Sample.Elasticsearch.WebApi.Core/Sample.Elasticsearch.WebApi.Core.csproj", "src/Sample.Elasticsearch.WebApi.Core/"]
COPY ["src/Sample.Elasticsearch.Domain/Sample.Elasticsearch.Domain.csproj", "src/Sample.Elasticsearch.Domain/"]
RUN dotnet restore "src/Sample.Elasticsearch.WebApi/Sample.Elasticsearch.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Sample.Elasticsearch.WebApi"
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