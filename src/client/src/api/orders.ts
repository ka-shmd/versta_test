import type {CreateOrderRequest, OrderDetails, PagedOrdersResponse} from "@/api/types.ts";
import {apiFetch} from "@/api/client.ts";

export function getOrders(page = 1, pageSize = 20) {
  const params = new URLSearchParams({
    page: page.toString(),
    pageSize: pageSize.toString()
  })

  return apiFetch<PagedOrdersResponse>(`/api/orders?${params}`)
}

export function getOrderByNumber(orderNumber: string) {
  return apiFetch<OrderDetails>(`/api/orders/${encodeURIComponent(orderNumber)}`)
}

export function createOrder(data: CreateOrderRequest) {
  return apiFetch<OrderDetails>("/api/orders", {
    method: "POST",
    body: JSON.stringify(data)
  })
}
