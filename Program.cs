using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);


app.MapGet("/", () => "Hello World! Teste");
app.MapGet("/user", () => "Marcos Valério filho de José Valério");
app.MapPost("/user", () => new {name="Marcos Valério", age=25});
app.MapGet("/AddHeader", (HttpResponse response) => response.Headers.Append("Teste","Stephany Batista"));


//parametros com QueryParans
//api.app.com/users?datastart=sa&cod=124
// app.MapGet("/getproduct", ([FromQuery]string dateStart, [FromQuery]string dateEnd) =>
// {
//     return dateStart + " - " + dateEnd;
// });


//CRUD
app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created("/products/"+ product.Cod, product.Cod);
});

//parametros na rota
//api.app.com/users/code
app.MapGet("/products/{cod}", ([FromRoute]string cod) =>
{
    var product = ProductRepository.GetBy(cod);
    if (product!=null) 
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapPut("/products", (Product product) =>
{
    var productSaved = ProductRepository.GetBy(product.Cod);
    productSaved.Name = product.Name;
});

app.MapDelete("/products/{cod}", ([FromRoute] string cod) =>
{
    var productSaved = ProductRepository.GetBy(cod);
    ProductRepository.Remove(productSaved);   
});


app.MapDelete("/configuration/dattabase", (IConfiguration configuration) =>
{
    return Results.Ok($"{configuration["TesteConfig:url"]}/{configuration["TesteConfig:url"]}");
    //ou return Results.Ok(configuration["TesteConfig:url"]);
    
    
});


//parametros no header do solicitante
//api.app.com/users/code
app.MapGet("/getproductheader/", (HttpRequest request) =>
{
    return request.Headers["product-code"].ToString();
});

app.Run();




public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        if(Products == null) 
            Products = new List<Product>();
    
        Products.Add(product);
    }

    public static Product GetBy(string cod)
    {
        return Products.FirstOrDefault(p => p.Cod == cod);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{

    public string? Cod {get; set;}
    public string? Name {get; set;}
}