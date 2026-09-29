const ACCESS_TOKEN_KEY = 'access_token'
const EXPIRES_AT_KEY = 'access_token_expires_at'

export function setSession(
    accessToken: string,
    expiresAtUtc: string
) {
    localStorage.setItem(
        ACCESS_TOKEN_KEY,
        accessToken
    )

    localStorage.setItem(
        EXPIRES_AT_KEY,
        expiresAtUtc
    )
}

export function getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY)
}

export function clearSession() {
    localStorage.removeItem(ACCESS_TOKEN_KEY)
    localStorage.removeItem(EXPIRES_AT_KEY)
}

export function isAuthenticated(): boolean {
    const token = getAccessToken()

    if (!token) {
        return false
    }

    const expiresAt =
        localStorage.getItem(EXPIRES_AT_KEY)

    if (!expiresAt) {
        return true
    }

    const expirationTime =
        new Date(expiresAt).getTime()

    if (Number.isNaN(expirationTime)) {
        return false
    }

    return expirationTime > Date.now()
}