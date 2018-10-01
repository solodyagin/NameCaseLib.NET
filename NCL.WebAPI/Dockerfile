FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["NCL.WebAPI.csproj", "WebAPI/"]
RUN dotnet restore "WebAPI/NCL.WebAPI.csproj"
COPY . WebAPI/
WORKDIR "/src/WebAPI"
RUN dotnet build "NCL.WebAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "NCL.WebAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NCL.WebAPI.dll"]