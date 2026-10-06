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

interface ShamlJwtPayload {
    elsa_access?: string | boolean
}

function getJwtPayload(): ShamlJwtPayload | null {
    const token = getAccessToken()

    if (!token) {
        return null
    }

    try {
        const parts = token.split('.')

        if (parts.length !== 3) {
            return null
        }

        const base64Url = parts[1]

        const base64 = base64Url
            .replace(/-/g, '+')
            .replace(/_/g, '/')

        const json = decodeURIComponent(
            atob(base64)
                .split('')
                .map(
                    character =>
                        '%' +
                        character
                            .charCodeAt(0)
                            .toString(16)
                            .padStart(2, '0')
                )
                .join('')
        )

        return JSON.parse(json) as ShamlJwtPayload
    } catch {
        return null
    }
}

export function canAccessElsa(): boolean {
    const payload = getJwtPayload()

    if (!payload) {
        return false
    }

    return (
        payload.elsa_access === true ||
        payload.elsa_access === 'true'
    )
}