import { apiFetch } from './http'

export interface UserRole {
    userRoleId: string
    roleId: string
    roleCode: string
    roleName: string
    portal: number
    scopeType: number
    centerId: string | null
    centerName: string | null
    isDefault: boolean
}

export interface CurrentUser {
    userId: string
    externalId: string
    activeUserRoleId: string
    roleId: string
    portal: number
    scopeType: number
    centerId: string | null
}

export interface SwitchRoleResponse {
    accessToken: string
    expiresAtUtc: string
}

export function getCurrentUser(): Promise<CurrentUser> {
    return apiFetch<CurrentUser>(
        '/api/me'
    )
}

export function getMyRoles(): Promise<UserRole[]> {
    return apiFetch<UserRole[]>(
        '/api/me/roles'
    )
}

export function switchRole(
    userRoleId: string
): Promise<SwitchRoleResponse> {
    return apiFetch<SwitchRoleResponse>(
        '/api/me/switch-role',
        {
            method: 'POST',
            body: JSON.stringify({
                userRoleId,
            }),
        }
    )
}