var builder = DistributedApplication.CreateBuilder(args);

var mssql = builder.AddSqlServer("mssql", port: 1433);
var messaging = builder.AddRabbitMQ("messaging")
    .WithDataVolume("DevStoreRabbit");

var databaseBilling = mssql.AddDatabase("DSBilling", "DSBilling");
var billing = builder.AddProject<Projects.DevStore_Billing_API>("billing")
    .WithReference(databaseBilling)
    .WithReference(messaging);

var databaseCatalog = mssql.AddDatabase("DSCatalog", "DSCatalog");
var catalog = builder.AddProject<Projects.DevStore_Catalog_API>("catalog")
    .WithReference(databaseCatalog)
    .WithReference(messaging);

var databaseCustomers = mssql.AddDatabase("DSCustomers", "DSCustomers");
var customers = builder.AddProject<Projects.DevStore_Customers_API>("customers")
    .WithReference(databaseCustomers)
    .WithReference(messaging);

var databaseIdentity = mssql.AddDatabase("DSUsers", "DSUsers");
var identity = builder.AddProject<Projects.DevStore_Identity_API>("identity")
    .WithReference(databaseIdentity)
    .WithReference(messaging);

var databaseOrders = mssql.AddDatabase("DSOrders", "DSOrders");
var orders = builder.AddProject<Projects.DevStore_Orders_API>("orders")
    .WithReference(databaseOrders)
    .WithReference(messaging);

var databaseShoppingcart = mssql.AddDatabase("DSShoppingCart", "DSShoppingCart");
var shoppingCart = builder.AddProject<Projects.DevStore_ShoppingCart_API>("shoppingCart")
    .WithReference(databaseShoppingcart)
    .WithReference(messaging);

var checkout = builder.AddProject<Projects.DevStore_Bff_Checkout>("bff-checkout")
    .WithReference(catalog)
    .WithReference(shoppingCart)
    .WithReference(orders)
    .WithReference(billing)
    .WithReference(customers);

//builder.AddProject<Projects.DevStore_WebApp_MVC>("webapp-mvc");

builder.Build().Run();
