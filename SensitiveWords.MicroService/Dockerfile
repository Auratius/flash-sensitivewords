FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SensitiveWords.API/Flash.SanitizeWords.API.csproj", "SensitiveWords.API/"]
COPY ["SensitiveWords.Application/Flash.SanitizeWords.Application.csproj", "SensitiveWords.Application/"]
COPY ["SensitiveWords.Infrastructure/Flash.SanitizeWords.Infrastructure.csproj", "SensitiveWords.Infrastructure/"]
COPY ["SensitiveWords.Domain/Flash.SanitizeWords.Domain.csproj", "SensitiveWords.Domain/"]

RUN dotnet restore "SensitiveWords.API/Flash.SanitizeWords.API.csproj"

COPY . .

WORKDIR "/src/SensitiveWords.API"
RUN dotnet build "Flash.SanitizeWords.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flash.SanitizeWords.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flash.SanitizeWords.API.dll"]
