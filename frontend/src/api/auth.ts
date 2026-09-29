import { apiFetch } from './http'

export interface LoginRequest {
    externalId: string
}

export interface LoginResponse {
    accessToken: string
    expiresAtUtc: string
}

export async function login(
    externalId: string
): Promise<LoginResponse> {
    return apiFetch<LoginResponse>(
        '/api/dev/token',
        {
            method: 'POST',
            body: JSON.stringify({
                externalId,
            }),
        }
    )
}