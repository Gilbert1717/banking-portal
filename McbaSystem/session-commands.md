https://docs.microsoft.com/en-au/aspnet/core/performance/caching/distributed?view=aspnetcore-7.0

# Installing tools

dotnet tool install --global dotnet-sql-cache
OR
dotnet tool update --global dotnet-sql-cache

# Distributed SQL Server Cache

Note the schema used below is dotnet not the default schema dbo.
The dotnet schema will need to be created on the database in advance with this SQL:

    create schema dotnet;

Run the following command to create the session cache table:

    dotnet sql-cache create "Server=rmit.australiaeast.cloudapp.azure.com;Encrypt=False;Uid=demo_McbaExampleWithLogin;Pwd=abc123;MultipleActiveResultSets=True" dotnet SessionCache
