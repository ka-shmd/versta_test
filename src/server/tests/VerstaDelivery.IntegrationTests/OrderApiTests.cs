using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VerstaDelivery.Api.Data;
using VerstaDelivery.Api.DTOs;

namespace VerstaDelivery.IntegrationTests;

[Collection("Api")]
public class OrderApiTests : IAsyncLifetime
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public OrderApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.MigrateDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private static CreateOrderRequest ValidRequest(string senderCity = "City") =>
        new(senderCity, "Address", "City", "Address", 10,
            DateOnly.FromDateTime(DateTime.UtcNow));

    private async Task PostOrderAsync(CreateOrderRequest? request = null)
    {
        request ??= ValidRequest();

        var response = await _client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<OrderDetails>();
        Assert.NotNull(created);
    }

    private async Task ClearOrdersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Orders.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task PostOrder_RequestIsValid_Returns201WithValidOrderDetails()
    {
        var request = new CreateOrderRequest(
            "Khabarovsk",
            "Lenina 2",
            "Vladivostok",
            "Tigrovaya 7",
            10,
            DateOnly.FromDateTime(DateTime.UtcNow));

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<OrderDetails>();
        Assert.NotNull(created);

        Assert.Matches(@"^ORD-\d{8}-[23456789ABCDEFGHJKMNPQRSTUVWXYZ]{6}$", created.OrderNumber);

        Assert.Equal(request.SenderCity, created.SenderCity);
        Assert.Equal(request.SenderAddress, created.SenderAddress);
        Assert.Equal(request.RecipientCity, created.RecipientCity);
        Assert.Equal(request.RecipientAddress, created.RecipientAddress);
        Assert.Equal(request.Weight, created.Weight);
        Assert.Equal(request.PickupDate, created.PickupDate);

        Assert.True(created.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.True(created.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task PostOrder_RequestWithEmptyFields_Returns400AndValidationProblemDetails()
    {
        var request = new CreateOrderRequest(string.Empty, string.Empty, string.Empty,
            string.Empty, 10, DateOnly.FromDateTime(DateTime.UtcNow));

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.NotEmpty(problem.Errors);

        Assert.True(problem.Errors.ContainsKey(nameof(CreateOrderRequest.SenderCity)));
        Assert.True(problem.Errors.ContainsKey(nameof(CreateOrderRequest.SenderAddress)));
        Assert.True(problem.Errors.ContainsKey(nameof(CreateOrderRequest.RecipientCity)));
        Assert.True(problem.Errors.ContainsKey(nameof(CreateOrderRequest.RecipientAddress)));
    }

    [Fact]
    public async Task GetListOrders_ValidRequest_Returns200AndPageWithOrder()
    {
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        var request = new CreateOrderRequest(
            "City", "Address", "City", "Address", 10, pickupDate);

        var postResponse = await _client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<OrderDetails>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync("/api/orders");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var page = await getResponse.Content.ReadFromJsonAsync<PagedOrdersResponse>();
        Assert.NotNull(page);
        Assert.Contains(page.Items, o => o.OrderNumber == created.OrderNumber);
    }

    [Fact]
    public async Task GetOrder_ValidOrderNumber_Returns200AndOrderFromDb()
    {
        var pickupDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        var request = new CreateOrderRequest(
            "SomeUniqueCity", "Address", "City", "Address", 10, pickupDate);

        var postResponse = await _client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<OrderDetails>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/orders/{created.OrderNumber}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var content = await getResponse.Content.ReadFromJsonAsync<OrderDetails>();
        Assert.NotNull(content);
        Assert.Equal(request.SenderCity, content.SenderCity);
    }

    [Fact]
    public async Task GetOrder_UnknownOrderNumber_Returns404()
    {
        var response = await _client.GetAsync("/api/orders/ORD-00000000-000000");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOrders_SecondPage_Returns200AndRemainingItems()
    {
        await ClearOrdersAsync();

        for (var i = 0; i < 21; i++)
        {
            await PostOrderAsync(ValidRequest($"City {i}"));
        }

        var page1Response = await _client.GetAsync("/api/orders?page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);

        var page1 = await page1Response.Content.ReadFromJsonAsync<PagedOrdersResponse>();
        Assert.NotNull(page1);
        Assert.Equal(20, page1.Items.Length);
        Assert.Equal(1, page1.Page);
        Assert.Equal(20, page1.PageSize);
        Assert.Equal(2, page1.TotalPages);

        var page2Response = await _client.GetAsync("/api/orders?page=2&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, page2Response.StatusCode);

        var page2 = await page2Response.Content.ReadFromJsonAsync<PagedOrdersResponse>();
        Assert.NotNull(page2);
        Assert.Equal(2, page2.Page);
        Assert.Single(page2.Items);
    }
}
