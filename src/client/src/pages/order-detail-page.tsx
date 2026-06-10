import {useParams} from "react-router-dom";

export function OrderDetailPage() {
  const { orderNumber } = useParams<{ orderNumber: string }>()
  return <h1 className="text-lg font-medium">Заказ {orderNumber}</h1>
}
