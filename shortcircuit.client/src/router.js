import { createWebHistory, createRouter } from 'vue-router'

import MainPage from './components/MainPage.vue'
import NotFound from './components/NotFound.vue'

const routes = [
    { path: "/", component: MainPage },
    { path: "/wrong-turn", component: NotFound },
    { path: "/:pathMatch(.*)*", redirect: '/wrong-turn' }
]
const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router