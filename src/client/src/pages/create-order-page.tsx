import {useNavigate} from "react-router-dom";
import {useMutation, useQueryClient} from "@tanstack/react-query";
import {useForm} from "react-hook-form";
import {
  type CreateOrderFormInput,
  type CreateOrderFormOutput,
  type CreateOrderFormValues,
  createOrderSchema
} from "@/schemas/order-schema.ts";
import {zodResolver} from "@hookform/resolvers/zod";
import {createOrder} from "@/api/orders.ts";
import {ValidationApiError} from "@/api/client.ts";
import {Alert} from "@/components/ui/alert.tsx";
import {Button} from "@/components/ui/button.tsx";
import {FieldGroup} from "@/components/ui/field.tsx";
import {FormFieldWrapper} from "@/components/ui/form-field-wrapper.tsx";

export function CreateOrderPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const form = useForm<CreateOrderFormInput, unknown, CreateOrderFormOutput>({
    resolver: zodResolver(createOrderSchema),
    defaultValues: {
      senderCity: "",
      senderAddress: "",
      recipientCity: "",
      recipientAddress: "",
      weight: 0,
      pickupDate: ""
    }
  })

  const mutation = useMutation({
    mutationFn: createOrder,
    onSuccess: (order) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] })
      navigate(`/orders/${order.orderNumber}`)
    },
    onError: (error) => {
      if (error instanceof ValidationApiError) {
        for (const [key, messages] of Object.entries(error.errors)) {
          const field = key.charAt(0).toLowerCase() + key.slice(1)

          if (messages[0]) {
            form.setError(field as keyof CreateOrderFormValues, {
              message: messages[0]
            })
          }
        }

        return
      }
    }
  })

  const showServerError = mutation.isError && !(mutation.error instanceof ValidationApiError)

  const onSubmit = form.handleSubmit((values) => {
    mutation.mutate(values)
  })

  return (
    <div className="space-y-6">
      <h1 className="text-lg font-medium">Создание заказа</h1>


      <form onSubmit={onSubmit} noValidate className="space-y-6">
        <FieldGroup>
          <FormFieldWrapper name="senderCity" label="Город отправителя" control={form.control} />
          <FormFieldWrapper name="senderAddress" label="Адрес отправителя" control={form.control} />
          <FormFieldWrapper name="recipientCity" label="Город получателя" control={form.control} />
          <FormFieldWrapper name="recipientAddress" label="Адрес получателя" control={form.control} />
          <FormFieldWrapper name="weight" label="Вес" control={form.control} type="number" inputProps={{step: 0.1}} />
          <FormFieldWrapper name="pickupDate" label="Дата забора" control={form.control} type="date"/>
        </FieldGroup>

        {showServerError &&
          <Alert className="border-red-500 border-2 text-red-500">Не удалось создать заказ</Alert>
        }

        <Button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? "Заказ создается" : "Создать заказ"}
        </Button>
      </form>
    </div>
  )
}
