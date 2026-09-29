import {
  clearSession,
  getAccessToken,
} from '../auth/auth'

export class ApiError extends Error {
  public readonly status: number

  constructor(
      status: number,
      message: string
  ) {
    super(message)

    this.name = 'ApiError'
    this.status = status
  }
}

export async function apiFetch<T>(
    path: string,
    options: RequestInit = {}
): Promise<T> {
  const token = getAccessToken()

  const headers = new Headers(options.headers)

  headers.set('Accept', 'application/json')

  if (options.body) {
    headers.set(
        'Content-Type',
        'application/json'
    )
  }

  if (token) {
    headers.set(
        'Authorization',
        `Bearer ${token}`
    )
  }

  const response = await fetch(
      path,
      {
        ...options,
        headers,
      }
  )

  if (!response.ok) {
    if (response.status === 401) {
      clearSession()
    }

    let message =
        `Request failed with status ${response.status}`

    try {
      const body = await response.json()

      message =
          body.detail ??
          body.title ??
          message
    } catch {
      // Ignore non-JSON responses.
    }

    throw new ApiError(
        response.status,
        message
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return await response.json() as T
}