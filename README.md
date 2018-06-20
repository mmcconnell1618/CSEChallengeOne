# CSE Challenge One
CSE Code Challenge One - Marcmc

#### Platform
Dotnet Core 2.1, ASP.NET MVC Core 2.1, EntityFramework Core 2.1

#### Requirements
This is a web app designed to be deployed on Azure App Service. You will also need an Azure Storage Account and an Azure SQL Database.

# Setup Instructions

## Create Azure Resources
Open the [Azure portal](https://portal.azure.com, "Azure Portal") and create:
1. A new Azure SQL Database
2. A new Storage Account
3. A new Azure Web App (with App Service Plan if needed)

## Add a login to the Azure SQL Server
Connect to your Azure SQL database using a management tool or go to the "Query Editor" in the Azure Portal and run this command:

```
CREATE USER [a_user_name] WITH PASSWORD = 'a_secure_password';
ALTER ROLE [db_owner] ADD MEMBER [a_user_name];
```
Make sure to replace "a_user_name" and "a_secure_password" with your own values.

## Storage Account Setup
1. In your newly created storage account, add a blob container which will be used to store the employee images
2. Save storage account settings in the appsettings.json file or user-secrets
```
dotnet user-secrets set "StorageUrl" "https://<storageaccountname>.blob.core.windows.net"
dotnet user-secrets set "StorageContainerName" "<containername>"
dotnet user-secrets set "StorageConnectionString" "<connection string to your storage account>"
```

## Application Setup
1. In the Azure Portal, navigate to your web app and go to Application Settings->Connection Strings and add a connection string to your SQL database using the login information you created above.

```
data source=<your_db_server_name>.database.windows.net,1433;initial catalog=<your_databse_name>;persist security info=True;user id=<your_sql_username>;password=<your_sql_password>;MultipleActiveResultSets=True;App=EntityFramework
```

2. Download or clone the Github repository for the application to your local machine
3. Open a command prompt to the project and build/restore
```
dotnet restore
dotnet build
```
4. Set the connection string secret for local development
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your connection string here>"
```
5. Run database migrations to create the schema in Azure
```
dotnet ef database update
```
6. Run the web app locally to verify it is working
```
dotnet run
```
7. Deploy from Visual Studio by opening the solution, then right-click on the ProfileManager web app and choose "publish." Follow the prompts and publish to your Azure Web App created above.
