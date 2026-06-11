import {Link, useParams} from "react-router-dom";
import {getOrderByNumber} from "@/api/orders.ts";
import {useQuery} from "@tanstack/react-query";
import {Skeleton} from "@/components/ui/skeleton.tsx";
import {ApiError} from "@/api/client.ts";
import {Button} from "@/components/ui/button.tsx";
import {Card, CardContent, CardHeader, CardTitle} from "@/components/ui/card.tsx";

export function OrderDetailPage() {
  const {orderNumber} = useParams<{ orderNumber: string }>()

  const {data, isLoading, isError, error, refetch} = useQuery({
    queryKey: ["order", orderNumber],
    queryFn: () => getOrderByNumber(orderNumber!),
    enabled: Boolean(orderNumber)
  })

  if (!orderNumber) {
    return (
      <div>
        <h1>Некорректный номер заказа</h1>
        <Link to="/orders">К списку заказов</Link>
      </div>
    )
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-full"/>
        <Skeleton className="h-8 w-full"/>
        <Skeleton className="h-8 w-full"/>
      </div>
    )
  }

  if (isError || !data) {
    const isNotFound = error instanceof ApiError && error.status === 404

    return (
      <div className="space-y-4">
        <h1 className="text-lg font-medium">Заказ</h1>
        <div>
          {isNotFound ?
            `Заказ "${orderNumber}" не найден` :
            "Не удалось загрузить заказ"
          }
        </div>
        {!isNotFound && (
          <Button onClick={() => refetch()}>Повторить</Button>
        )}
        <Link to="/orders" className="text-muted-foreground underline">К списку заказов</Link>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-lg font-medium">Заказ {data.orderNumber}</h1>
        <Link to="/orders" className="text-sm text-muted-foreground underline">Назад к списку</Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Информация о заказе</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex justify-between">
            <div>Город отправителя</div>
            <div>{data.senderCity}</div>
          </div>
          <div className="flex justify-between">
            <div>Адрес отправителя</div>
            <div>{data.senderAddress}</div>
          </div>
          <div className="flex justify-between">
            <div>Город получателя</div>
            <div>{data.recipientCity}</div>
          </div>
          <div className="flex justify-between">
            <div>Адрес получателя</div>
            <div>{data.recipientAddress}</div>
          </div>
          <div className="flex justify-between">
            <div>Вес</div>
            <div>{data.weight}</div>
          </div>
          <div className="flex justify-between">
            <div>Дата забора</div>
            <div>{data.pickupDate}</div>
          </div>
          <div className="flex justify-between">
            <div>Дата создания заказа</div>
            <div>{data.createdAt.slice(0, 10)}</div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
