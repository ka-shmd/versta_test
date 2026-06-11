import {useQuery} from "@tanstack/react-query";
import {getOrders} from "@/api/orders.ts";
import {Skeleton} from "@/components/ui/skeleton.tsx";
import {Button} from "@/components/ui/button.tsx";
import {Link, useNavigate, useSearchParams} from "react-router-dom";
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow} from "@/components/ui/table.tsx";
import {useEffect} from "react";

export function OrdersListPage() {
  const navigate = useNavigate()

  const [searchParams, setSearchParams] = useSearchParams()

  const rawPage = Number(searchParams.get("page") ?? "1")
  const page = Number.isFinite(rawPage) ? rawPage : 1

  const {data, isLoading, isError, error, refetch} = useQuery({
    queryKey: ["orders", page],
    queryFn: () => getOrders(page)
  })

  const isPageValid = () => {
    if (!data) return false

    if (page < 1) return false
    if (data.totalPages > 0 && page > data.totalPages) return false
    if (data.totalPages === 0) return false

    return true;
  }

  //редиректим на первую страницу, если в url невалидный параметр page
  useEffect(() => {
    const safePage = isPageValid() ? page : 1

    if (safePage != page) {
      setSearchParams({page: String(safePage)})
    }
  }, [data, page, setSearchParams])

  if (isLoading) {
    return (
      <div className="space-y-4">
        <h1>Загрузка...</h1>
        <Skeleton className="h-8 w-full" />
        <Skeleton className="h-8 w-full" />
        <Skeleton className="h-8 w-full" />
      </div>
    )
  }

  if (isError) {
    return (
      <div className="space-y-4">
        <h1 className="text-lg font-medium">Список заказов</h1>
        <p>Не удалось загрузить заказы.</p>
        <Button onClick={() => refetch()}>Повторить</Button>
      </div>
    )
  }

  if (!data || data.items.length === 0) {
    return (
      <div className="space-y-4">
        <h1 className="text-lg font-medium">Список заказов</h1>
        <p>Заказов еще нет</p>
        <Link to="/">Создать заказ</Link>
      </div>
    )
  }

  return (
    <div>
      <h1 className="text-lg font-medium">Список заказов</h1>
      <Table>
        <TableHeader>
          <TableRow>
           <TableHead>Номер заказа</TableHead>
           <TableHead>Город отправителя</TableHead>
           <TableHead>Адрес отправителя</TableHead>
           <TableHead>Город получателя</TableHead>
           <TableHead>Адрес получателя</TableHead>
           <TableHead>Вес</TableHead>
           <TableHead>Дата забора</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {data.items.map((order) => (
            <TableRow key={order.orderNumber} className="cursor-pointer"
                      onClick={() => navigate(`/orders/${order.orderNumber}`)}>
              <TableCell>{order.orderNumber}</TableCell>
              <TableCell>{order.senderCity}</TableCell>
              <TableCell>{order.senderAddress}</TableCell>
              <TableCell>{order.recipientCity}</TableCell>
              <TableCell>{order.recipientAddress}</TableCell>
              <TableCell>{order.weight}</TableCell>
              <TableCell>{order.pickupDate}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      <div className="flex justify-center gap-4 mt-4">
        <Button
          variant="outline"
          disabled={page <= 1}
          onClick={() => setSearchParams({page: String(page - 1)})}
        >
          Назад
        </Button>

        <span className="text-sm text-muted-foreground flex flex-col justify-center">
          Страница {data.page} из {data.totalPages}
        </span>

        <Button
          variant="outline"
          disabled={page >= data.totalPages}
          onClick={() => setSearchParams({page: String(page + 1)})}
        >
          Вперед
        </Button>
      </div>
    </div>
    )
}
