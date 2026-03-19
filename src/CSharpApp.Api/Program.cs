var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration(builder.Configuration);
builder.Services.AddHttpConfiguration(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

var basePath = "api/v{version:apiVersion}";

var productPath = $"{basePath}/products";

var basePathGroup = versionedEndpointRouteBuilder
    .MapGroup(basePath);

var productsGroup = versionedEndpointRouteBuilder
    .MapGroup(productPath);

productsGroup.MapGet("",
    async (IProductsService productsService) =>
    {
        var products = await productsService.GetProducts();
        return Results.Ok(products);
    })
    .WithName("GetProducts")
    .HasApiVersion(1.0);

productsGroup.MapGet("{id:int}",
    async (int id, IProductsService productsService) =>
    {
        var product = await productsService.GetProductByID(id);
        return product is null ? Results.NotFound() : Results.Ok(product);
    })
    .WithName("GetProductByID")
    .HasApiVersion(1.0);

productsGroup.MapPost("",
    async (CreateProductRequest request, IProductsService productsService) =>
    {
        var createdProduct = await productsService.CreateNewProduct(request);
        return createdProduct is null
            ? Results.Problem()
            : Results.CreatedAtRoute(
            "GetProductByID",
            new { id = createdProduct.Id },
            createdProduct);
    }
    ).WithName("CreateNewProduct")
    .HasApiVersion(1.0);

app.Run();