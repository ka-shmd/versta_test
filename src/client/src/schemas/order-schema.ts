import {z} from "zod"

const citySchema = z.string().trim().min(1, "Поле не должно быть пустым").max(100, "Не более 100 символов")
const addressSchema = z.string().trim().min(1, "Поле не должно быть пустым").max(255, "Не более 255 символов")

const weightSchema = z.coerce
  .number({ error: "Введите число" })
  .gt(0, "Вес должен быть больше нуля")

const pickupDateSchema = z.string()
  .min(1, "Укажите дату")
  .refine((value) => value >= new Date().toISOString().slice(0, 10), "Забор груза нельзя оформить задним числом")

export const createOrderSchema = z.object({
  senderCity: citySchema,
  senderAddress: addressSchema,
  recipientCity: citySchema,
  recipientAddress: addressSchema,
  weight: weightSchema,
  pickupDate: pickupDateSchema
})

export type CreateOrderFormInput = z.input<typeof createOrderSchema>
export type CreateOrderFormOutput = z.output<typeof createOrderSchema>

export type CreateOrderFormValues = CreateOrderFormOutput
