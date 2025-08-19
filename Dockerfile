# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Solution və csproj-ları əvvəlcə kopyala (restore üçün)
COPY *.sln ./
COPY Core/Legno.Application/Legno.Application.csproj Core/Legno.Application/
COPY Core/Legno.Domain/Legno.Domain.csproj       Core/Legno.Domain/
COPY Infrastructure/Legno.Infrastructure/Legno.Infrastructure.csproj Infrastructure/Legno.Infrastructure/
COPY Infrastructure/Legno.Persistence/Legno.Persistence.csproj     Infrastructure/Legno.Persistence/
COPY Presentation/Legno.WebApi/Legno.WebApi.csproj                 Presentation/Legno.WebApi/

# NuGet bərpası
RUN dotnet restore

# Bütün mənbə faylları
COPY . .

# Publish
WORKDIR /src/Presentation/Legno.WebApi
RUN dotnet publish -c Release -o /app/out /p:UseAppHost=false


# ===== Runtime stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# NReco.VideoConverter üçün ffmpeg (Linux) lazımdır
RUN apt-get update \
 && apt-get install -y --no-install-recommends ffmpeg \
 && rm -rf /var/lib/apt/lists/*

# Publish olunmuş çıxışı kopyala
COPY --from=build /app/out .

# Kestrel 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Legno.WebApi.dll"]
