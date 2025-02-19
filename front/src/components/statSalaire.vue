<script setup lang="ts">
import axiosInstance from '@/config/axios'
import LineChart from '@/components/ui/LineChart.vue'
import { ref, watch } from 'vue'
import { useToast } from 'vue-toast-notification'
import { useGlobalStore } from '@/stores/globalStore'
import router from '@/router'

const toast = useToast()
const AnneeFilter = ref<number>(new Date().getFullYear())

const moisEnLettres = [
  'Jan',
  'Fév',
  'Mar',
  'Avr',
  'Mai',
  'Jun',
  'Jul',
  'Aoû',
  'Sep',
  'Oct',
  'Nov',
  'Déc'
]

const salaireData = ref<{
  employeId: string
  employeNom: string
  employePrenom: string
  annee: number
  salaires: Array<{
    mois: number
    montant: number
    datePaiement: Date
    avanceDeduite: number
  }>
}>({
  employeId: '',
  employeNom: '',
  employePrenom: '',
  annee: new Date().getFullYear(),
  salaires: []
})

const loading = ref(false)

const fetchSalaire = async () => {
  loading.value = true
  const globalStore = useGlobalStore()
  if(!(globalStore && globalStore.isLoggedIn)){
    alert("Aucun utilisateur trouvé. Veuillez vous connecter.")
    router.push('/')
  }
  try {
    const response = await axiosInstance.get<{
      employeId: string
      employeNom: string
      employePrenom: string
      annee: number
      salaires: Array<{
        mois: number
        montant: number
        datePaiement: Date
        avanceDeduite: number
      }>
    }>(
      `/Salaire/salaires/par-annee?id_employe=${globalStore.user?.employeId}&annee=${AnneeFilter.value}`
    )
    salaireData.value = response.data
    updatechart()
  } catch (err: any) {
    toast.error(err.response.data.message)
    salaireData.value = {
      employeId: '',
      employeNom: '',
      employePrenom: '',
      annee: new Date().getFullYear(),
      salaires: []
    }
    resetchart()
  } finally {
    loading.value = false
  }
}
fetchSalaire()

const productivityData = ref({
  labels: [
    moisEnLettres[0],
    moisEnLettres[1],
    moisEnLettres[2],
    moisEnLettres[3],
    moisEnLettres[4],
    moisEnLettres[5],
    moisEnLettres[6],
    moisEnLettres[7],
    moisEnLettres[8],
    moisEnLettres[9],
    moisEnLettres[10],
    moisEnLettres[11]
  ],
  datasets: [
    {
      label: 'Salaire',
      borderColor: 'rgba(34, 197, 94, 1)',
      backgroundColor: 'rgba(34, 197, 94, 0.2)',
      borderWidth: 2,
      pointRadius: 1,
      pointHoverBackgroundColor: 'rgba(254, 226, 226)',
      data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
      fill: true,
      tension: 0.1
    }
  ]
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  scales: {
    x: {
      grid: {
        display: true
      },
      ticks: {
        color: 'rgba(147, 197, 225, 1)',
        font: {
          weight: 'bold',
          size: 8
        }
      }
    },
    y: {
      grid: {
        display: false
      },
      ticks: {
        color: 'rgba(71, 85, 105, 1)',
        font: {
          weight: 'bold',
          size: 9
        }
      },
      beginAtZero: true
    }
  },
  plugins: {
    legend: {
      display: false
    },
    title: {
      display: false
    }
  }
}

const updatechart = () => {
  const salaires = salaireData.value.salaires

  const bilanAnnuel = new Array(12).fill(null).map((_, index) => ({
    mois: index + 1,
    montant: 0
  }))
  // Remplissage avec les salaires existants
  salaires.forEach((s) => {
    if (s.mois >= 1 && s.mois <= 12) {
      bilanAnnuel[s.mois - 1].montant = s.montant
    }
  })

  // Construire les labels et données
  const labels = bilanAnnuel.map((s) => moisEnLettres[s.mois - 1])
  const data = bilanAnnuel.map((s) => s.montant)

  // Crée un nouvel objet pour productivityData
  productivityData.value = {
    labels: [...labels],
    datasets: [
      {
        ...productivityData.value.datasets[0],
        data: [...data]
      }
    ]
  }
}

const resetchart = () => {
  productivityData.value = {
    labels: [
      moisEnLettres[0],
      moisEnLettres[1],
      moisEnLettres[2],
      moisEnLettres[3],
      moisEnLettres[4],
      moisEnLettres[5],
      moisEnLettres[6],
      moisEnLettres[7],
      moisEnLettres[8],
      moisEnLettres[9],
      moisEnLettres[10],
      moisEnLettres[11]
    ],
    datasets: [
      {
        ...productivityData.value.datasets[0],
        data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
      }
    ]
  }
}

const clearFilter = () => {
  AnneeFilter.value = new Date().getFullYear()
  salaireData.value = {
    employeId: '',
    employeNom: '',
    employePrenom: '',
    annee: new Date().getFullYear(),
    salaires: []
  }
  resetchart()
}

//Recherche automatique si la valeur de searchTerm change
watch(AnneeFilter, async (newValue, oldValue) => {
  console.log(`AnneeFilter a changé de ${oldValue} à ${newValue}`)
  fetchSalaire()
  updatechart()
})
</script>

<template>
  <div class="w-full h-full">
    <h1 class="font-semibold text-slate-600 text-2xl mt-5">Historique des salaires</h1>
    <section
      class="posteContenu w-full mt-6 rounded-lg flex flex-row justify-between p-4 bg-slate-200"
    >
      <div
        class="productivite w-full h-96 flex flex-col bg-gray-50 rounded-lg shadow-lg shadow-slate-300 p-4"
      >
        <h2 class="font-sans font-medium text-xl text-slate-700">Rapport annuel</h2>
        <div
          class="annee w-60 flex flex-row items-center justify-between font-sans text-xs text-slate-600"
        >
          <label class="font-semibold">Annee :</label>
          <input
            type="number"
            required
            id="anneeFilter"
            v-model="AnneeFilter"
            placeholder="Contact"
            class="w-36 my-2 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          />
          <button
            type="button"
            @click="clearFilter()"
            class="annuler mt-2 p-1 text-xs rounded font-sans font-medium border-[1px] border-slate-400 hover:border-red-400 text-slate-600 hover:bg-red-400 hover:text-slate-200 transition-all m-2"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="19"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="lucide lucide-paintbrush"
            >
              <path d="m14.622 17.897-10.68-2.913" />
              <path
                d="M18.376 2.622a1 1 0 1 1 3.002 3.002L17.36 9.643a.5.5 0 0 0 0 .707l.944.944a2.41 2.41 0 0 1 0 3.408l-.944.944a.5.5 0 0 1-.707 0L8.354 7.348a.5.5 0 0 1 0-.707l.944-.944a2.41 2.41 0 0 1 3.408 0l.944.944a.5.5 0 0 0 .707 0z"
              />
              <path
                d="M9 8c-1.804 2.71-3.97 3.46-6.583 3.948a.507.507 0 0 0-.302.819l7.32 8.883a1 1 0 0 0 1.185.204C12.735 20.405 16 16.792 16 15"
              />
            </svg>
          </button>
        </div>
        <LineChart
          :chartData="productivityData"
          :chart-options="chartOptions"
          class="w-full h-[80%]"
        />
        <h3 class="font-sans text-xs text-slate-600">
          Courbe des salaires enregistrés au cours de cette année
        </h3>
      </div>
    </section>
  </div>
</template>
