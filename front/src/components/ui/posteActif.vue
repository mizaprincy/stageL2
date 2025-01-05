<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import axios from 'axios'
import { eventBus } from '../events/eventBus'

const backendUrl = import.meta.env.VITE_BACKEND_URL
const activePostesCount = ref(0) // Variable pour stocker le nombre d'employés actifs

// Fonction pour récupérer le nombre d'employés actifs
const fetchActivePostesCount = async () => {
  try {
    const response = await axios.get(`${backendUrl}/PosteTravail/actifs/count`)
    activePostesCount.value = response.data // Mettre à jour la variable avec le résultat
  } catch (err) {
    console.error('Erreur lors de la récupération du nombre de postes de travail actifs', err)
  }
}
const updateActivePosteCount = async () => {
  fetchActivePostesCount()
}

onMounted(() => {
  fetchActivePostesCount()
  eventBus.on('updateActivePosteCount', updateActivePosteCount)
})

onBeforeUnmount(() => {
  eventBus.off('updateActivePosteCount', updateActivePosteCount)
})
</script>

<template>
  <div class="w-full flex flex-row justify-evenly mt-6 text-2xl font-semibold text-slate-100">
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="30"
      height="32"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
      class="lucide lucide-folder-kanban"
    >
      <path
        d="M4 20h16a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.93a2 2 0 0 1-1.66-.9l-.82-1.2A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13c0 1.1.9 2 2 2Z"
      />
      <path d="M8 10v4" />
      <path d="M12 10v2" />
      <path d="M16 10v6" />
    </svg>
    <span id="nbPostes" class="nbPostes">{{ activePostesCount }}</span>
  </div>
</template>
