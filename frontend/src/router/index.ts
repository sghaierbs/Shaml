import { createRouter, createWebHistory } from 'vue-router'
import CentersView from '../views/centers/CentersView.vue'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            redirect: '/centers',
        },
        {
            path: '/centers',
            name: 'centers',
            component: CentersView,
        },
    ],
})

export default router