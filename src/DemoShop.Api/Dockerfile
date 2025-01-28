FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do \
    mkdir -p src/${file%.*}/ \
    && mv $file src/${file%.*}/; \
done
RUN dotnet restore src/DemoShop.Api/DemoShop.Api.csproj
