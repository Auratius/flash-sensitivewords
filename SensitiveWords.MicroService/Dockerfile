FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SensitiveWords.API/Flash.SensitiveWords.API.csproj", "SensitiveWords.API/"]
COPY ["SensitiveWords.Application/Flash.SensitiveWords.Application.csproj", "SensitiveWords.Application/"]
COPY ["SensitiveWords.Infrastructure/Flash.SensitiveWords.Infrastructure.csproj", "SensitiveWords.Infrastructure/"]
COPY ["SensitiveWords.Domain/Flash.SensitiveWords.Domain.csproj", "SensitiveWords.Domain/"]

RUN dotnet restore "SensitiveWords.API/Flash.SensitiveWords.API.csproj"

COPY . .

WORKDIR "/src/SensitiveWords.API"
RUN dotnet build "Flash.SensitiveWords.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flash.SensitiveWords.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flash.SensitiveWords.API.dll"]
