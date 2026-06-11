type ProblemDetails = {
  errors?: Record<string, string[]>
}

export class ApiError extends Error {
  constructor(status: number, body: unknown){
    super(`Ошибка Api: ${status}`)
    this.name = "ApiError"
  }
}

export class ValidationApiError extends ApiError {
  errors: Record<string, string[]>

  constructor(errors: Record<string, string[]>) {
    super(400, errors)
    this.name = "ValidationApiError"
    this.errors = errors
  }
}

export async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const base = import.meta.env.VITE_API_BASE_URL ?? ""
  const headers = new Headers(options?.headers)

  if (options?.body !== undefined) {
    headers.set("Content-Type", "application/json")
  }

  const response = await fetch(`${base}${path}`, {
    ...options,
    headers
  })

  if (!response.ok) {
    const body = (await response.json().catch(() => null)) as ProblemDetails | null

    if (response.status == 400 && body?.errors) {
      throw new ValidationApiError(body.errors)
    }

    throw new ApiError(response.status, body)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return await response.json() as Promise<T>
}
