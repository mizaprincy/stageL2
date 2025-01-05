<script lang="ts">
import { defineComponent, ref, onMounted, onBeforeUnmount } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toast-notification'
import EffectuerPaiement from '@/components/effectuerPaiement.vue'
import LineChart from '@/components/ui/LineChart.vue'
import { eventBus } from '@/components/events/eventBus'

export default defineComponent({
  name: 'paiement',
  components: { EffectuerPaiement, LineChart },
  setup() {
    const backendUrl = import.meta.env.VITE_BACKEND_URL
    const toast = useToast({position: 'top-right'})
    const EmployeIdFilter = ref<string>('')
    const AnneeFilter = ref<number>(new Date().getFullYear())
    const moisEnLettres = [
      'Janvier',
      'Février',
      'Mars',
      'Avril',
      'Mai',
      'Juin',
      'Juillet',
      'Août',
      'Septembre',
      'Octobre',
      'Novembre',
      'Décembre'
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
    const error = ref<string | null>(null)

    const fetchSalaire = async () => {
      loading.value = true
      error.value = null

      if (!EmployeIdFilter.value || !AnneeFilter.value) {
        alert('Veuillez choisir les filtres')
        loading.value = false
        return
      }

      try {
        const response = await axios.get<{
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
          `${backendUrl}/Salaire/salaires/par-annee?id_employe=${EmployeIdFilter.value}&annee=${AnneeFilter.value}`
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
        error.value = 'Erreur lors du chargement des employés'
      } finally {
        loading.value = false
      }
    }

    const productivityData = ref({
      labels: [
        moisEnLettres[0],
        moisEnLettres[1],
        moisEnLettres[2],
        moisEnLettres[3],
        moisEnLettres[4],
        moisEnLettres[5]
      ],
      datasets: [
        {
          label: 'Salaire',
          borderColor: 'rgba(190, 252, 100, 1)',
          backgroundColor: 'rgba(34, 197, 94, 0.08)',
          borderWidth: 2,
          pointRadius: 1,
          pointHoverBackgroundColor: 'rgba(254, 226, 226)',
          data: [0, 0, 0, 0, 0, 0],
          fill: true,
          tension: 0.2
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
            color: 'rgba(147, 197, 225, .6)',
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
            color: 'rgba(248, 250, 262, 1)',
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
      const salaires = salaireData.value.salaires || []
      // Si le tableau est vide, vider le graphique
      if (salaires.length === 0) {
        productivityData.value.labels = []
        productivityData.value.datasets[0].data = []
        return
      }

      // Récupérer les trois derniers salaires
      const lastThree = salaires.slice(-6)

      // Compléter si moins de trois salaires
      const paddedSalaires = new Array(6 - lastThree.length)
        .fill({ mois: null, montant: null })
        .concat(lastThree)
      // Construire les labels et données
      const labels = paddedSalaires.map((s) =>
        s.mois !== null ? moisEnLettres[s.mois - 1] : '-'
      )
      const data = paddedSalaires.map((s) => (s.montant !== null ? s.montant : 0))
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
          moisEnLettres[5]
        ],
        datasets: [
          {
            ...productivityData.value.datasets[0],
            data: [0, 0, 0, 0, 0, 0]
          }
        ]
      }
    }

    const clearFilter = () => {
      EmployeIdFilter.value = ''
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

    //fetchEmployes apres un ajout
    function handlePaiementAdded() {
      fetchSalaire()
    }
    onMounted(() => {
      eventBus.on('paiementAdded', handlePaiementAdded)
    })

    onBeforeUnmount(() => {
      eventBus.off('paiementAdded', handlePaiementAdded)
    })

    return {
      EmployeIdFilter,
      AnneeFilter,
      fetchSalaire,
      clearFilter,
      salaireData,
      productivityData,
      chartOptions,
      loading,
      moisEnLettres
    }
  }
})
</script>

<template>
  <div class="w-full h-full p-6 relative">
    <h1 class="font-sans text-slate-100 text-2xl mt-5">Paiement de Salaire</h1>
    <h2 class="font-sans text-slate-300 text-sm mt-4">
      C'est ici qu'on enregistre les salaires de chaque employe
    </h2>
    <section class="employeContenu w-full mt-6 flex flex-row justify-between p-4">
      <div class="paiement w-[40%] rounded-3xl bg-slate-200">
        <h2 class="font-semibold text-slate-700 p-4 text-xl text-center">
          Effectuer un enregistrement
        </h2>
        <div>
          <EffectuerPaiement />
        </div>
      </div>

      <div class="productivite w-[55%] h-[26rem] flex flex-row rounded-3xl bg-slate-500 p-4">
        <div class="w-[50%] h-full mr-1">
          <div class="w-full h-[50%] mb-1 bg-slate-700 rounded-3xl px-6 py-4">
            <h2 class="font-semibold text-green-100 mb-2">Filtrage</h2>
            <form
              @submit.prevent="fetchSalaire()"
              class="filterForm w-full flex flex-col items-center"
            >
              <div
                class="identifiant w-full flex flex-row items-center justify-between font-sans text-[0.65rem] text-slate-200"
              >
                <label>Employe Id :</label>
                <input
                  type="text"
                  required
                  id="employeIdFilter"
                  v-model="EmployeIdFilter"
                  placeholder="Employe ID"
                  class="w-36 my-2 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
                />
              </div>
              <div
                class="annee w-full flex flex-row items-center justify-between font-sans text-xs text-slate-200"
              >
                <label>Annee :</label>
                <input
                  type="number"
                  required
                  id="anneeFilter"
                  v-model="AnneeFilter"
                  placeholder="Contact"
                  class="w-36 my-2 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
                />
              </div>
              <div class="action-btn flex flex-row">
                <button
                  type="submit"
                  class="filtrer w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-600 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
                >
                  Filtrer
                </button>
                <button
                  type="button"
                  @click="clearFilter()"
                  class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-200 transition-all m-2"
                >
                  Effacer
                </button>
              </div>
            </form>
          </div>
          <div class="slaireStat w-full h-[11.75rem] bg-slate-700 rounded-3xl p-4">
            <div>
              <LineChart :chartData="productivityData" :chart-options="chartOptions" class="w-full" />
              <h3 class="font-sans text-[0.60rem] text-slate-200 mx-2">
                Courbe des salaires des six derniers mois payés
              </h3>
            </div>
          </div>
        </div>
        <div class="w-[50%] h-full bg-slate-800 rounded-3xl">
          <h2 class="font-semibold text-green-100 pt-4 text-center">Historique de paie</h2>
          <div class="container w-full h-[90%] p-4">
            <div class="content w-full h-full bg-slate-700 rounded-2xl p-2 text-slate-200 text-xs">
              <header
                class="enTete w-full h-[22%] text-[0.65rem] flex flex-col pb-3 border-b-[1px] border-slate-400 mb-2"
              >
                <div class="profile w-full flex flex-row">
                  <label
                    >Employe :
                    <span class="font-normal"
                      >{{ salaireData.employeNom }} {{ salaireData.employePrenom }}</span
                    ></label
                  >
                </div>
                <label
                  >ID : <span class="font-light">{{ salaireData.employeId }}</span></label
                >
                <div class="w-full flex flex-row justify-between items-center">
                  <label
                    >Annee : <span class="font-light">{{ salaireData.annee }}</span></label
                  >
                  <button
                    v-if="salaireData.salaires.length"
                    class="ficheDePaie w-20 p-1 font-medium text-slate-200 hover:bg-slate-400 bg-slate-600 rounded-lg border-[1px] border-slate-400"
                  >
                    Fiche de paie
                  </button>
                </div>
              </header>
              <!-- Affichage du chargement -->
              <div v-if="loading" class="chargement w-full text-center">Chargement...</div>
              <div
                v-if="salaireData.salaires.length"
                class="wrapper w-full h-[78%] overflow-x-auto"
              >
                <div
                  v-for="(salaire, index) in salaireData.salaires"
                  :key="index"
                  class="salaire w-full flex flex-col text-[0.65rem] font-sans font-medium text-slate-300 p-2 hover:bg-slate-600 rounded overflow-hidden"
                >
                  <label class="text-slate-400"
                    >Mois de : {{ moisEnLettres[salaire.mois - 1] }}</label
                  >
                  <label>Date de paiement : {{ salaire.datePaiement }}</label>
                  <label>Montant totale : {{ salaire.montant + salaire.avanceDeduite }} Ar</label>
                  <label>Avance du mois : {{ salaire.avanceDeduite }} Ar</label>
                  <label>Montant Net : {{ salaire.montant }} Ar</label>
                </div>
              </div>
              <div v-else class="aucuneNotification w-full text-center">Aucun salaire trouvé.</div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
label {
  font-weight: 500;
}
.wrapper::-webkit-scrollbar {
  display: none;
}
</style>
