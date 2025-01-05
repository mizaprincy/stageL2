<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import logo from '@/components/image/logo.png'
import { useRoute } from 'vue-router'
import Welcome from './welcome.vue'

const route = useRoute()
</script>

<template>
  <div class="w-full flex flex-row">
    <div class="wrapper w-60 flex flex-col items-center bg-slate-800 h-screen">
      <RouterLink to="/admin" class="logo w-[70%] mt-12">
        <img :src="logo" alt="logo" />
      </RouterLink>

      <div
        class="routeContainer w-40 flex flex-col items-center h-[28rem] p-6 mt-14 rounded-lg bg-slate-700 text-slate-300 font-semibold"
      >
        <h1 class="mb-8 font-semibold text-xl text-slate-200 font-sans">Menu</h1>
        <nav class="w-full flex flex-row flex-wrap items-center">
          <div class="container w-full flex flex-row justify-between">
            <RouterLink
              to="/admin/profile"
              class="child-link w-12 flex flex-col justify-center items-center py-1 px-2 bg-gray-800 hover:bg-green-300 hover:opacity-70 hover:text-slate-900 rounded"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-user-round"
              >
                <circle cx="12" cy="8" r="5" />
                <path d="M20 21a8 8 0 0 0-16 0" />
              </svg>
              <label class="font-sans text-[0.5rem] cursor-pointer">Profile</label>
            </RouterLink>
            <RouterLink
              to="/admin/employe"
              class="child-link w-12 flex flex-col justify-center items-center py-1 px-2 bg-gray-800 hover:bg-green-300 hover:opacity-70 hover:text-slate-900 rounded"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-house"
              >
                <path d="M15 21v-8a1 1 0 0 0-1-1h-4a1 1 0 0 0-1 1v8" />
                <path
                  d="M3 10a2 2 0 0 1 .709-1.528l7-5.999a2 2 0 0 1 2.582 0l7 5.999A2 2 0 0 1 21 10v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"
                />
              </svg>
              <label class="font-sans text-[0.5rem] cursor-pointer">Employe</label>
            </RouterLink>
          </div>
          <div class="container w-full flex flex-row justify-between mt-6">
            <RouterLink
              to="/admin/modification"
              class="child-link w-12 flex flex-col justify-center items-center py-1 px-2 bg-gray-800 hover:bg-green-300 hover:opacity-70 hover:text-slate-900 font-semibold rounded text-slate-300"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-settings"
              >
                <path
                  d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z"
                />
                <circle cx="12" cy="12" r="3" />
              </svg>
              <label class="font-sans text-[0.5rem] cursor-pointer">Reglages</label>
            </RouterLink>
            <RouterLink
              to="/admin/paiement"
              class="child-link w-12 flex flex-col justify-center items-center py-1 px-2 bg-gray-800 hover:bg-green-300 hover:opacity-70 hover:text-slate-900 font-semibold rounded text-slate-300"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-chart-line"
              >
                <path d="M3 3v16a2 2 0 0 0 2 2h16" />
                <path d="m19 9-5 5-4-4-3 3" />
              </svg>
              <label class="font-sans text-[0.5rem] cursor-pointer">Paiement</label>
            </RouterLink>
          </div>
        </nav>
      </div>
    </div>
    <div v-if="route.path === '/admin'" class="w-full h-screen">
      <Welcome />
    </div>
    <RouterView class="w-full h-screen bg-slate-900" />
  </div>
</template>

<style scoped>
.router-link-active.child-link:not(.logo) {
  background-color: #86efac;
  color: #0f172a;
}
</style>
