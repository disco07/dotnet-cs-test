#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80/tcp

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

COPY ["*.sln", "."]
COPY ["./dotnet-cs-test/**.csproj", "./dotnet-cs-test/"]
COPY ["./TestProject/**.csproj", "./TestProject/"]
RUN dotnet restore 
COPY . .

RUN dotnet build -c release --no-restore

RUN dotnet test -c release --no-build

FROM build AS publish
RUN dotnet publish "./dotnet-cs-test/dotnet-cs-test.csproj" -c release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet-cs-test.dll"]