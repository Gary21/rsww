
/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('@/pages/index.vue'),
    props: true,
  },
  {
    path: '/offers',
    name: 'Offers',
    component: () => import('@/pages/offers.vue'),
    props: true,
  },
  {
    path: '/offer',
    name: 'Offer',
    component: () => import('@/pages/offer.vue'),
    props: true,
  },
  {
    path: '/reservation',
    name: 'Reservation',
    component: () => import('@/pages/reservation.vue'),
    props: true,
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/pages/login.vue'),
    props: true,
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router
