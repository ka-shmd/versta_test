using System.Net;
using System.Net.Http.Json;
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

    [Fact]
    public async Task PostOrder_RequestIsValid_Returns201()
    {
        var request = new CreateOrderRequest("City", "Address", "City",
            "Address", 10, DateOnly.FromDateTime(DateTime.UtcNow));

        var response = await _client.PostAsJsonAsync("api/orders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task PostOrder_RequestWithEmptyFields_Returns400()
    {
        var request = new CreateOrderRequest(string.Empty, string.Empty, string.Empty,
            string.Empty, 10, DateOnly.FromDateTime(DateTime.UtcNow));

        var response = await _client.PostAsJsonAsync("api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
}
