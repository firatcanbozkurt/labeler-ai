FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY . ./
RUN dotnet build -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose OPC UA port
EXPOSE 4840

# Create directory for CSV files
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:4840
ENV CSV_FILE_PATH=/app/data/data.csv

ENTRYPOINT ["dotnet", "OpcUaServer.dll"]