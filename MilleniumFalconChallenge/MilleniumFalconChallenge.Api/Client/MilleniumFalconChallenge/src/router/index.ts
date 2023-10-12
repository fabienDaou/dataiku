import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'welcome',
      component: () => import('../views/WelcomeView.vue')
    },
    {
      path: '/scenarios',
      name: 'scenarios',
      component: () => import('../views/ScenariosView.vue')
    }
  ]
})

export default router
