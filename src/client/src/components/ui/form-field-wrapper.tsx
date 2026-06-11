import type {CreateOrderFormInput, CreateOrderFormOutput, CreateOrderFormValues} from "@/order-schema.ts";
import {type Control, Controller} from "react-hook-form";
import * as React from "react";
import {Field, FieldError, FieldLabel} from "@/components/ui/field.tsx";
import {Input} from "@/components/ui/input.tsx";

type FormFieldProps = {
  name: keyof CreateOrderFormValues
  label: string
  control: Control<CreateOrderFormInput, unknown, CreateOrderFormOutput>
  type?: "text" | "date" | "number"
  inputProps?: React.ComponentProps<"input">
}

export function FormFieldWrapper({name, label, control, type = "text", inputProps}: FormFieldProps) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <Field data-invalid={fieldState.invalid}>
          <FieldLabel htmlFor={name}>{label}</FieldLabel>
          <Input {...field} {...inputProps} value={field.value as string | string[] | number | undefined} id={name} type={type} aria-invalid={fieldState.invalid}/>
          {fieldState.invalid && (
            <FieldError errors={[fieldState.error]} />
          )}
        </Field>
      )}
    />
  )
}
