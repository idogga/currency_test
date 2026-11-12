FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FinanceService.csproj", "./"]
RUN dotnet restore "FinanceService.csproj"

COPY . .
RUN dotnet publish "FinanceService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl ca-certificates

RUN rm -rf /var/lib/apt/lists/* \
 && groupadd --system --gid 1001 user \
 && useradd  --system --uid 1001 --gid user user

COPY --from=build --chown=user:user /app/publish .

USER user

ENTRYPOINT ["dotnet", "FinanceService.dll"]