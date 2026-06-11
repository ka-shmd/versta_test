export interface CreateOrderRequest {
  senderCity: string
  senderAddress: string
  recipientCity: string
  recipientAddress: string
  weight: number
  pickupDate: string
}

export interface OrderSummary {
  orderNumber: string
  senderCity: string
  senderAddress: string
  recipientCity: string
  recipientAddress: string
  weight: number
  pickupDate: string
}

export interface OrderDetails extends OrderSummary {
  createdAt: string
}

export interface PagedOrdersResponse {
  items: OrderSummary[]
  page: number
  pageSize: number
  totalPages: number
}
