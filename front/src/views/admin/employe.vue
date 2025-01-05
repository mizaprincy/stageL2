<script setup lang="ts">
import EmployeList from '@/components/employe-list.vue'
import { ref } from 'vue'
import axios from 'axios'
import AjoutEmploye from '@/components/ajoutEmploye.vue'
import EmployeActif from '@/components/ui/employeActif.vue'
import type { Inbox } from '@/components/interface/Employe'
import InboxAdmin from '@/components/inbox-Admin.vue'
import { useGlobalStore } from '@/stores/globalStore'

const backendUrl = import.meta.env.VITE_BACKEND_URL
const inboxes = ref<Inbox[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

//Get all inbox admin
const fetchInbox = async () => {
  loading.value = true
  error.value = null
  const globalStore = useGlobalStore()
  if (!(globalStore && globalStore.isLoggedIn)) {
    console.error('User non trouve')
    return
  }
  try {
    const response = await axios.get<Inbox[]>(`${backendUrl}/Inbox/admin?email=${globalStore.user?.email}`)
    inboxes.value = response.data
  } catch (err) {
    error.value = 'Erreur lors du chargement des employés'
  } finally {
    loading.value = false
  }
}

fetchInbox()
function displayFormAjoutEmploye() {
  const formAjoutEmployeContainer = document.getElementById('formAjoutEmployeContainer')!
  formAjoutEmployeContainer.style.display = 'block'
}
</script>

<template>
  <div class="w-full h-full p-6 relative">
    <h1 class="font-sans text-slate-100 text-2xl mt-5">Gestion des employés</h1>
    <h2 class="font-sans text-slate-300 text-sm mt-4">
      Ceci a été conçu pour la gestion des employés au sein de l'entreprise
    </h2>
    <section
      class="employeContenu w-full mt-6 rounded-lg flex flex-row justify-between p-4 bg-slate-500"
    >
      <div class="employe-list w-[55%] h-[26rem] bg-slate-800 rounded-lg shadow-md shadow-slate-800">
        <header class="flex flex-row justify-between border-b-[1px] border-slate-400">
          <h2 class="font-semibold text-green-100 p-4">Liste des Employés</h2>
          <div class="ajout p-4">
            <button
              class="ajoutEmploye w-20 p-1 text-sm font-medium text-slate-200 hover:bg-slate-400 bg-slate-600 rounded-lg flex flex-row justify-evenly border-[1px] border-slate-400"
              @click="displayFormAjoutEmploye()"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="13"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-user-round-plus"
              >
                <path d="M2 21a8 8 0 0 1 13.292-6" />
                <circle cx="10" cy="8" r="5" />
                <path d="M19 16v6" />
                <path d="M22 19h-6" />
              </svg>
              Ajout
            </button>
          </div>
        </header>
        <section class="w-full p-4">
          <EmployeList />
        </section>
      </div>

      <div class="w-[40%] flex flex-col justify-between">
        <div class="w-full inline-flex justify-between h-36">
          <div class="nombreEmployes w-48 bg-slate-700 rounded-lg p-3 shadow-sm shadow-slate-800">
            <p class="font-sans font-medium text-green-100 mt-1">Employés actifs</p>
            <EmployeActif />
          </div>
          <div
            class="w-48 bg-[url('../components/image/fond-employe.jpg')] bg-cover rounded-lg shadow-sm shadow-slate-800"
          ></div>
        </div>
        <div class="inbox w-full h-64 bg-slate-800 rounded-lg shadow-md shadow-slate-800 overflow-hidden">
          <header class="w-full border-b-[1px] border-slate-400">
            <h2 class="font-semibold text-green-100 p-3">Notifications</h2>
          </header>
          <section class="inbox-admin w-full h-52 px-2 pb-2 mt-2 overflow-x-auto">
            <InboxAdmin />
          </section>
        </div>
      </div>
    </section>
    <AjoutEmploye />
  </div>
</template>

<style scoped>
.inbox-admin::-webkit-scrollbar {
  display: none;
}
</style>
