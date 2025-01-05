import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/admin/Home.vue'
import Employe from '@/views/admin/employe.vue'
import Profile from '@/views/admin/profile.vue'
import Modification from '@/views/admin/modification.vue'
import Login from '@/views/login.vue'
import Paiement from '@/views/admin/paiement.vue'
import WelcomeEmploye from '@/views/employe/welcomeEmploye.vue'
import Apercu from '@/views/employe/apercu.vue'
import Service from '@/views/employe/service.vue'
import { useGlobalStore } from '@/stores/globalStore'

const routes = [
  {
    path: '/',
    name: 'login',
    component: Login
  },
  {
    path: '/employe',
    name: 'employe',
    component: WelcomeEmploye,
    children: [
      {
        path: 'apercu',
        component: Apercu
      },
      {
        path: 'service',
        component: Service
      }
    ]
  },
  {
    path: '/admin',
    name: 'admin',
    component: Home,
    children: [
      {
        path: 'profile',
        component: Profile
      },
      {
        path: 'employe',
        component: Employe
      },
      {
        path: 'modification',
        component: Modification
      },
      {
        path: 'paiement',
        component: Paiement
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  const globalStore = useGlobalStore()

  // Gestion de la navigation vers la page de login
  if (to.name === 'login') {
    globalStore.logout()
    next() // Autorise l'accès à la page de login
    return
  }

  // Gestion de l'accès à la route /admin
  if (to.path.startsWith('/admin')) {
    if (!globalStore.isLoggedIn) {
      alert('Aucun utilisateur trouvé. Veuillez vous connecter.')
      return next({ name: 'login' })
    }

    if (!globalStore.isAdmin) {
      alert('Accès refusé. Vous devez vous connecter avec un compte Admin.')
      return next({ name: 'login' })
    }
  }
  // Gestion de l'accès à la route /employe
  if (to.path.startsWith('/employe')) {
    if (!globalStore.isLoggedIn) {
      alert('Aucun utilisateur trouvé. Veuillez vous connecter.')
      return next({ name: 'login' })
    }

    if (globalStore.isAdmin) {
      alert('Accès refusé. Vous devez vous connecter avec un compte Employe.')
      return next({ name: 'login' })
    }
  }
  next()
})

export default router
