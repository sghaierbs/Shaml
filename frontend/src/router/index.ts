import {
    createRouter,
    createWebHistory,
} from 'vue-router'

import CentersView from '../views/centers/CentersView.vue'
import CasesView from '../views/cases/CasesView.vue'
import PublicHomeView from '../views/PublicHomeView.vue'

import {
    isAuthenticated,
} from '../auth/auth'

import {
    getCurrentUser,
    getMyRoles,
} from '../api/roles'

import {
    getLandingRoute,
} from '../auth/navigation'

const router = createRouter({
    history: createWebHistory(),

    routes: [
        {
            path: '/',
            name: 'home',
            meta: {
                requiresAuth: true,
            },
            component: {
                template: '<div></div>',
            },
        },

        {
            path: '/centers',
            name: 'centers',
            meta: {
                requiresAuth: true,
                portal: 1,
            },
            component: CentersView,
        },

        {
            path: '/cases',
            name: 'cases',
            meta: {
                requiresAuth: true,
                portal: 1,
            },
            component: CasesView,
        },

        {
            path: '/public',
            name: 'public-home',
            meta: {
                requiresAuth: true,
                portal: 2,
            },
            component: PublicHomeView,
        },

        {
            path: '/login',
            name: 'login',
            component: () =>
                import('../views/auth/LoginView.vue'),

            meta: {
                public: true,
            },
        },
    ],
})

async function resolveLandingRoute(): Promise<string> {
    const [currentUser, roles] =
        await Promise.all([
            getCurrentUser(),
            getMyRoles(),
        ])

    const activeRole = roles.find(
        role =>
            role.userRoleId ===
            currentUser.activeUserRoleId
    )

    return getLandingRoute(
        currentUser,
        activeRole
    )
}

router.beforeEach(async (to) => {

    /*
     * Not authenticated and trying to access
     * a protected route.
     */
    if (
        to.meta.requiresAuth &&
        !isAuthenticated()
    ) {
        return {
            name: 'login',
        }
    }

    /*
     * Nothing else to check for an
     * unauthenticated user.
     */
    if (!isAuthenticated()) {
        return true
    }

    try {
        const currentUser =
            await getCurrentUser()

        /*
         * Authenticated user visiting login.
         *
         * Send them to the correct portal instead.
         */
        if (to.name === 'login') {
            return await resolveLandingRoute()
        }

        /*
         * "/" means:
         *
         * "Take me to my appropriate landing page."
         */
        if (to.name === 'home') {
            return await resolveLandingRoute()
        }

        /*
         * Prevent Public users from manually
         * navigating to Internal portal pages
         * and vice versa.
         */
        const requiredPortal =
            to.meta.portal as number | undefined

        if (
            requiredPortal !== undefined &&
            currentUser.portal !== requiredPortal
        ) {
            return await resolveLandingRoute()
        }

        return true
    } catch (error) {
        console.error(
            'Failed to resolve user navigation:',
            error
        )

        return {
            name: 'login',
        }
    }
})

export default router