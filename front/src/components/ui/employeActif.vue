<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import axiosInstance from '@/config/axios'
import { eventBus } from '../events/eventBus'

const activeEmployesCount = ref(0) // Variable pour stocker le nombre d'employés actifs

// Fonction pour récupérer le nombre d'employés actifs
const fetchActiveEmployesCount = async () => {
  try {
    const response = await axiosInstance.get(`/Employe/actifs/count`)
    activeEmployesCount.value = response.data // Mettre à jour la variable avec le résultat
  } catch (err) {
    console.error("Erreur lors de la récupération du nombre d'employés actifs", err)
  }
}
const updateActiveEmployeCount = async () => {
  fetchActiveEmployesCount()
}

onMounted(() => {
  fetchActiveEmployesCount()
  eventBus.on('updateActiveEmployeCount', updateActiveEmployeCount)
})

onBeforeUnmount(() => {
  eventBus.off('updateActiveEmployeCount', updateActiveEmployeCount)
})
</script>

<template>
  <div class="w-full flex flex-row justify-center mt-6 text-2xl font-semibold text-slate-100">
    <svg
      style="margin-right: 5px"
      xmlns="http://www.w3.org/2000/svg"
      color="#f1f5f9"
      width="32"
      height="32"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-width="1"
      stroke-linecap="round"
      stroke-linejoin="round"
      class="lucide lucide-circle-user"
    >
      <circle cx="12" cy="12" r="10" />
      <circle cx="12" cy="10" r="3" />
      <path d="M7 20.662V19a2 2 0 0 1 2-2h6a2 2 0 0 1 2 2v1.662" />
    </svg>
    <span id="nbEmployes" class="nbEmployes">{{ activeEmployesCount }}</span>
  </div>
</template>
