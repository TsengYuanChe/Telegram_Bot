# 使用官方 .NET SDK 構建應用
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# 複製 CSPROJ 文件並還原依賴項
COPY *.csproj ./
RUN dotnet restore

# 複製源代碼並編譯
COPY . ./
RUN dotnet publish -c Release -o out

# 使用 .NET 6.0 運行時鏡像運行應用
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# 暴露應用執行的端口
EXPOSE 8081

# 啟動應用程序
ENTRYPOINT ["dotnet", "TelegramBotApp.dll", "--urls=http://0.0.0.0:8081"]