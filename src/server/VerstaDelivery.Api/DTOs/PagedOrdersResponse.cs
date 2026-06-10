namespace VerstaDelivery.Api.DTOs;

public record PagedOrdersResponse(OrderSummary[] Items, int Page, int PageSize, int TotalPages);
