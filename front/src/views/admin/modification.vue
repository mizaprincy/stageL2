<script setup lang="ts">
import axios from 'axios'
import { type PosteTravail } from '@/components/interface/Employe'
import { eventBus } from '@/components/events/eventBus'
import BarChart from '@/components/ui/BarChart.vue'
import { onMounted, ref } from 'vue'
import PosteActif from '@/components/ui/posteActif.vue'
import PosteList from '@/components/poste-list.vue'
import AjoutPoste from '@/components/ajoutPoste.vue'

//fetchEmployes apres un ajout
function handlePosteAdded() {
  fetchPostes()
}
// Données du graphique de productivité
const productivityData = ref({
  labels: ['-', '-', '-'], // Labels basés sur les designs des postes
  datasets: [
    {
      label: 'Heures travaillées',
      borderColor: 'rgba(54, 162, 235, 1)',
      backgroundColor: 'rgba(54, 162, 235, 0.2)',
      borderWidth: 2,
      pointRadius: 3,
      pointHoverBackgroundColor: 'rgba(54, 162, 235, 0.8)',
      data: [0, 0, 0],
      fill: false,
      tension: 0.4
    },
    {
      label: 'Salaire horaire',
      borderColor: 'rgba(255, 99, 132, 1)',
      backgroundColor: 'rgba(255, 99, 132, 0.2)',
      borderWidth: 2,
      pointRadius: 3,
      pointHoverBackgroundColor: 'rgba(255, 99, 132, 0.8)',
      data: [0, 0, 0],
      fill: false,
      tension: 0.4
    },
    {
      label: 'Salaire',
      borderColor: 'rgba(75, 192, 192, 1)',
      backgroundColor: 'rgba(75, 192, 192, 0.2)',
      borderWidth: 2,
      pointRadius: 3,
      pointHoverBackgroundColor: 'rgba(75, 192, 192, 0.8)',
      data: [0, 0, 0],
      fill: false,
      tension: 0.4
    }
  ]
})

const chartOptions = ref({
  responsive: true,
  plugins: {
    legend: {
      display: true,
      position: 'top', // Position des légendes (peut être 'top', 'bottom', 'left', ou 'right')
      labels: {
        boxWidth: 30, // Largeur des icônes des légendes
        boxHeight: 10, // Hauteur des icônes des légendes
        font: {
          family: 'Arial', // Famille de police
          size: 12, // Taille de la police
          weight: 'semibold' // Épaisseur de la police
        },
        color: 'rgba(248, 250, 262, 1)', // Couleur du texte
        padding: 20 // Espace autour des labels
      }
    },
    title: {
      display: true,
      text: 'Productivité par poste de travail',
      color: '#94a3b8',
      font: {
        family: 'Arial',
        size: 16,
        weight: 'bold'
      }
    }
  },
  scales: {
    x: {
      ticks: {
        color: 'rgba(147, 197, 225, .6)',
        font: {
          weight: 'bold',
          size: 10
        }
      }
    },
    y: {
      grid: {
        display: false
      },
      ticks: {
        color: 'rgba(248, 250, 262, 1)',
        font: {
          weight: 'bold',
          size: 10
        }
      },
      beginAtZero: true
    }
  }
})

const backendUrl = import.meta.env.VITE_BACKEND_URL
// Liste des postes de travail
const postes = ref<PosteTravail[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Fonction pour récupérer les postes
const fetchPostes = async () => {
  loading.value = true
  error.value = null

  try {
    const response = await axios.get<PosteTravail[]>(`${backendUrl}/PosteTravail`)
    postes.value = response.data
    const labels = postes.value.map((poste) => poste.design)
    const chartDataHeuresTravaillees = postes.value.map((poste) => poste.heures_travaillees)
    const chartDataSalaireHoraire = postes.value.map((poste) => poste.salaire_horaire)
    const chartDataSalaire = postes.value.map(
      (poste) => poste.salaire_horaire * poste.heures_travaillees
    )
    //mise a jour des donnees du chart
    productivityData.value = {
      labels: [...labels],
      datasets: [
        {
          ...productivityData.value.datasets[0],
          data: [...chartDataHeuresTravaillees]
        },
        {
          ...productivityData.value.datasets[1],
          data: [...chartDataSalaireHoraire]
        },
        {
          ...productivityData.value.datasets[2],
          data: [...chartDataSalaire]
        }
      ]
    }
  } catch (err) {
    error.value = 'Erreur lors du chargement des postes de travail'
  } finally {
    loading.value = false
  }
}
fetchPostes()

onMounted(() => {
  eventBus.on('PosteAdded', handlePosteAdded)
})

function displayFormAjoutPoste() {
  const formAjoutPosteContainer = document.getElementById('formAjoutPosteContainer')!
  formAjoutPosteContainer.style.display = 'block'
}
</script>

<template>
  <div class="w-full h-full p-6 relative">
    <h1 class="font-sans text-slate-100 text-2xl mt-5">Modifications</h1>
    <h3 class="font-sans text-slate-300 text-sm mt-4">
      Cette section est faite pour gerer les postes de travail
    </h3>
    <section
      class="posteContenu w-full mt-6 rounded-lg flex flex-row justify-between p-4 bg-slate-500"
    >
      <div class="w-[40%] flex flex-col justify-between">
        <div class="w-full inline-flex justify-between h-36">
          <div
            class="w-56 bg-[url('../components/image/fond-employe.jpg')] bg-cover rounded-lg shadow-md shadow-slate-800"
          ></div>
          <div class="nombrePostes w-36 bg-gray-800 rounded-lg shadow-md shadow-slate-800 p-3">
            <h2 class="font-sans font-medium mt-1">Poste de travail</h2>
            <PosteActif />
          </div>
        </div>
        <div class="w-full h-64 bg-gray-800 rounded-lg shadow-md shadow-slate-800 p-5">
          <header class="mb-3 flex flex-row justify-between">
            <h2 class="font-semibold">Liste des postes de travail</h2>
            <button
              class="ajoutPoste text-slate-200 p-1 bg-slate-500 rounded"
              @click="displayFormAjoutPoste()"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="18"
                height="13"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-plus"
              >
                <path d="M5 12h14" />
                <path d="M12 5v14" />
              </svg>
            </button>
          </header>
          <PosteList />
        </div>
      </div>
      <div
        class="productivite w-[55%] h-[26rem] flex flex-col justify-between bg-gray-800 rounded-lg shadow-md shadow-slate-800 p-6"
      >
        <h2 class="font-sans font-semibold text-xl">Productivité</h2>
        <BarChart
          :chartData="productivityData"
          :chart-options="chartOptions"
          class="w-full mb-6"
        />
        <h3 class="font-sans text-xs text-slate-200 text-center">
          Courbe des heures travaillées, salaire horaire et salaire du mois en fonction du poste de
          travail
        </h3>
      </div>
    </section>
    <AjoutPoste />
  </div>
</template>

<style scoped>
h2 {
  color: rgb(211, 247, 247);
}
</style>
