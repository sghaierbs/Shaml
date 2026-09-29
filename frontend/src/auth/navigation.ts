import type {
    CurrentUser,
    UserRole,
} from '../api/roles'

export const PortalType = {
    Internal: 1,
    Public: 2,
} as const

export type PortalType =
    typeof PortalType[keyof typeof PortalType]

export function getLandingRoute(
    currentUser: CurrentUser,
    activeRole?: UserRole
): string {

    if (currentUser.portal === PortalType.Public) {
        return '/public'
    }

    if (currentUser.portal === PortalType.Internal) {
        switch (activeRole?.roleCode) {
            case 'organization-admin':
                return '/centers'

            case 'specialist':
            case 'center-director':
            case 'operations-supervisor':
                return '/cases'

            default:
                return '/'
        }
    }

    return '/'
}