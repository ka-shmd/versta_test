import {useQuery} from "@tanstack/react-query";
import {getOrders} from "@/api/orders.ts";

export function OrdersListPage() {
  const {data, isLoading, isError, error} = useQuery({
    queryKey: ["orders"],
    queryFn: () => getOrders()
  })

  if (isLoading) return <p>Загрузка...</p>
  if (isError) return <p>Ошибка: {String(error)}</p>

  if (!data) return <p>Заказов еще нет.</p>

  return (
    <div>
      <h1 className="text-lg font-medium">Список заказов</h1>
      <pre className="mt-4 text-sm">{JSON.stringify(data.items, null, 2)}</pre>
    </div>
    )
}
