<script setup lang="ts">
import { ref, watch, onBeforeUnmount, onMounted } from 'vue'
import axiosInstance from '@/config/axios'
import { type PosteTravail } from './interface/Employe'
import { useRoute, useRouter } from 'vue-router'
import { type Employe } from './interface/Employe'
import { useToast } from 'vue-toast-notification'
import { eventBus } from './events/eventBus'
import '@vuepic/vue-datepicker/dist/main.css'
import ModifierEmploye from './modifierEmploye.vue'


const toast = useToast({position: 'top-right'})
// Utilisation de Vue Router pour gérer l'URL
const route = useRoute()
const router = useRouter()
// Liste d'employés
const employes = ref<Employe[]>([])
const loading = ref(false)
const searchTerm = ref<string>('') // État pour la recherche

// Fonction pour récupérer les employés
const fetchEmployes = async () => {
  loading.value = true

  try {
    const response = await axiosInstance.get<Employe[]>(`/Employe`)
    employes.value = response.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}
fetchEmployes()

// Fonction de recherche employe
const filteredEmployes = async () => {
  loading.value = true

  // Vérifier si le champ de recherche est vide
  if (!searchTerm.value) {
    // Si aucun terme de recherche, retirer `name` de l'URL et appliquer les filtres sans ce paramètre
    router.push({ query: { ...route.query, name: undefined } })
    await applyFilter()
    return
  }

  try {
    // Met à jour le paramètre `name` dans l'URL avec la valeur actuelle de `searchTerm`
    router.replace({ query: { ...route.query, name: searchTerm.value.toLowerCase() } })
    const response = await axiosInstance.get<Employe[]>(`/Employe/search`, {
      params: {
        name: searchTerm.value.toLowerCase(),
        id_poste: postefilter.value,
        statut: statut.value,
        embauche: embaucheFilter.value ? embaucheFilter.value.toISOString().split('T')[0] : null
      }
    })

    employes.value = response.data
  } catch (err) {
    employes.value = []
  } finally {
    loading.value = false
  }
}

//Filtre
const postefilter = ref<number>(0)
const statut = ref<string>('actif')
const actifFilter = ref(true)
const embaucheFilter = ref<Date | null>(null)

let filtreSelected = ref<boolean>(false)

const showFilter = () => {
  const filterContent = document.getElementById('filterContent')!
  filterContent.style.display = 'flex'
  filtreSelected.value = true
}
const hideFilter = () => {
  const filterContent = document.getElementById('filterContent')!
  filterContent.style.display = 'none'
  filtreSelected.value = false
}
const resetFilter = async () => {
  postefilter.value = 0
  statut.value = 'actif'
  actifFilter.value = true
  embaucheFilter.value = null

  // Mettre à jour l'URL en supprimant explicitement les filtres
  router.push({
    query: {
      ...route.query,
      name: undefined,
      statut: statut.value.toLowerCase(),
      id_poste: undefined,
      embauche: undefined
    }
  })

  await fetchEmployes() // Rafraîchit la liste des employés après réinitialisation
}

const applyFilter = async () => {
  loading.value = true
  // Formater la date d'embauche si présente
  const formattedEmbauche = embaucheFilter.value ? new Date(embaucheFilter.value).toISOString().split('T')[0]: null
  
  // Mettre à jour les paramètres de l'URL pour refléter les filtres actifs
  if(statut.value == 'actif') actifFilter.value = true
  else actifFilter.value = false
  router.push({
    query: {
      ...route.query,
      name: undefined,
      statut: statut.value.toLowerCase(),
      id_poste: postefilter.value || undefined,
      embauche: formattedEmbauche || undefined
    }
  })

  try {
    // Requête avec les paramètres de filtre
    const response = await axiosInstance.get<Employe[]>(`/Employe/filter`, {
      params: {
        id_poste: postefilter.value || undefined,
        statut: statut.value.toLowerCase(),
        embauche: formattedEmbauche
      }
    })

    hideFilter()
    employes.value = response.data
  } catch (err) {
    console.error(err)
    employes.value = []
  } finally {
    loading.value = false
  }
}

//Recherche automatique si la valeur de searchTerm change
watch(searchTerm, async () => {
  await filteredEmployes()
})

// Delete Employe
const deleteEmploye = async (id_employe: string) => {
  const confirmation = confirm(
    `Êtes-vous sûr de vouloir supprimer cet employé de l'entreprise: ${id_employe} ?`
  )
  if (!confirmation) return
  const dateDepart = prompt('Veuillez entrer la date de départ (YYYY-MM-DD) :')
  if (!dateDepart) {
    toast.error("La date de départ est requise pour marquer l'employé comme inactif.")
    return
  }
  const datePattern = /^\d{4}-\d{2}-\d{2}$/
  if (!datePattern.test(dateDepart)) {
    toast.error("La date de départ doit être au format : YYYY-MM-DD.")
    return
  }

  try {
    await axiosInstance.delete(`/Employe/${id_employe}/${dateDepart}`)
    // Mettre à jour la liste des employés après suppression
    await fetchEmployes()
    toast.success(`Employé ${id_employe} supprimé`)
    eventBus.emit('updateActiveEmployeCount')
  } catch (err : any) {
    toast.error(err.response.data.message)
    console.error(err)
  }
}

//Edit Employe
const isEditVisible = ref(false)
const employeSelectionne = ref<Employe>()

const editEmploye = (employe: Employe) => {
  employeSelectionne.value = { ...employe } // Cloner l'objet employe
  isEditVisible.value = true
}

const mettreAJourEmploye = async (employe: Employe) => {
  // Logique pour mettre à jour l'employé dans la liste ou via une requête backend
  const id = employe.id_employe
  try {
    // Appel de l'API avec la méthode PUT
    const response = await axiosInstance.put(`/Employe/${id}`, employe)

    // Vérifier le statut de la réponse
    if (response.status === 204) {
      toast.success(`Employé ${id} mis à jour avec succès !`)
      fetchEmployes()
    } else {
      console.error("Erreur lors de la mise à jour de l'employé", response.status)
    }
  } catch (err : any) {
    toast.error(err.response.data.message)
    console.error(err)
  }
  fermerModifierEmploye() // Cacher le formulaire après mise à jour
}

const fermerModifierEmploye = () => {
  isEditVisible.value = false
}

//fetchEmployes apres un ajout
function handleEmployeAdded() {
  fetchEmployes()
  eventBus.emit('updateActiveEmployeCount')
}

onMounted(() => {
  if (route.query.name) {
    searchTerm.value = String(route.query.name)
    filteredEmployes()
  }
  eventBus.on('employeAdded', handleEmployeAdded)
})

onBeforeUnmount(() => {
  eventBus.off('employeAdded', handleEmployeAdded)
})

// Liste des postes de travail
const postes = ref<PosteTravail[]>([])
// Fonction pour récupérer les postes
const fetchPostes = async () => {
  loading.value = true

  try {
    const response = await axiosInstance.get<PosteTravail[]>(`/PosteTravail`)
    postes.value = response.data
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}

fetchPostes()
</script>
<template>
  <div class="employee-table w-full mt-4">
    <div class="container w-full flex justify-center">
      <div class="recherche w-[60%] flex flex-row justify-evenly mb-5">
        <input
          type="text"
          placeholder="Rechercher..."
          class="recherche w-44 rounded-sm text-xs p-1.5 bg-slate-100 outline-none"
          v-model="searchTerm"
        />
        <div class="filtreContainer relative">
          <div class="buttonFilter w-20">
            <button
              v-if="!filtreSelected"
              @click="showFilter()"
              class="filtreNotSelected flex flex-row justify-center font-medium text-xs text-slate-700 bg-slate-100 p-1.5 rounded"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="13"
                height="13"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-filter mr-1 mt-0.5"
              >
                <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3" />
              </svg>
              Filtre
            </button>
            <button
              v-if="filtreSelected"
              @click="hideFilter()"
              class="filtreSelected font-medium text-xs text-slate-700 bg-slate-100 p-1.5 rounded"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-arrow-left"
              >
                <path d="m12 19-7-7 7-7" />
                <path d="M19 12H5" />
              </svg>
            </button>
          </div>
          <div
            id="filterContent"
            class="filterContent flex-col mt-1 rounded bg-slate-200 text-xs absolute hidden"
          >
            <header
              class="filterheader w-full h-10 flex items-center p-2 border-b-2 border-solid border-slate-600 font-sans font-medium text-slate-700"
            >
              Les filtres seront appliques sur la liste
            </header>
            <div class="filterSection inline-flex p-2 border-b-2 border-solid border-slate-600">
              <div id="posteFilter" class="posteFilter">
                <select
                  name="poste"
                  id="poste"
                  v-model="postefilter"
                  class="w-40 mr-2 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600 font-sans"
                >
                  <option value="0" disabled selected hidden>Poste de travail</option>
                  <option
                    :value="poste.id_poste"
                    class="hover:bg-red-300"
                    v-for="poste in postes"
                    :key="poste.id_poste"
                  >
                    {{ poste.design }}
                  </option>
                </select>
              </div>
              <div class="statutFilter">
                <select
                  name="statut"
                  id="statut"
                  v-model="statut"
                  class="w-20 mr-4 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600 font-sans"
                >
                  <option value="actif">Actif</option>
                  <option value="inactif">Inactif</option>
                </select>
              </div>
              <div class="embaucheFilter flex flex-row items-center">
                <span class="font-sans font-medium text-slate-600 mr-1">Embauche:</span>
                <input
                  type="date"
                  name="embaucheFilter"
                  v-model="embaucheFilter"
                  id="embauche"
                  class="w-36 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600 font-sans"
                />
              </div>
            </div>
            <footer
              class="filterfooter w-full h-10 flex flex-row justify-between items-center p-2 font-sans text-slate-700"
            >
              <button
                @click="resetFilter()"
                class="clear w-20 flex flex-row justify-center p-1 rounded hover:bg-red-400 hover:text-slate-200 transition-all"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="18"
                  height="18"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="lucide lucide-paintbrush mr-1"
                >
                  <path d="m14.622 17.897-10.68-2.913" />
                  <path
                    d="M18.376 2.622a1 1 0 1 1 3.002 3.002L17.36 9.643a.5.5 0 0 0 0 .707l.944.944a2.41 2.41 0 0 1 0 3.408l-.944.944a.5.5 0 0 1-.707 0L8.354 7.348a.5.5 0 0 1 0-.707l.944-.944a2.41 2.41 0 0 1 3.408 0l.944.944a.5.5 0 0 0 .707 0z"
                  />
                  <path
                    d="M9 8c-1.804 2.71-3.97 3.46-6.583 3.948a.507.507 0 0 0-.302.819l7.32 8.883a1 1 0 0 0 1.185.204C12.735 20.405 16 16.792 16 15"
                  />
                </svg>
                Effacer
              </button>
              <button
                @click="applyFilter()"
                class="clear w-20 p-1.5 rounded hover:bg-slate-700 hover:text-slate-200 transition-all"
              >
                Appliquer
              </button>
            </footer>
          </div>
        </div>
      </div>
    </div>
    <!-- Affichage du chargement -->
    <div v-if="loading" class="chargement w-full text-center text-slate-300 text-sm">
      Chargement...
    </div>
    <div class="listeEmployes w-full">
      <div v-if="!employes.length" class="AucunEmploye w-full text-center text-slate-300 text-sm">
        Aucun employé à afficher.
      </div>
      <table class="w-full text-slate-300">
        <tbody class="w-full text-xs h-4 overflow-hidden">
          <tr
            v-for="employe in employes"
            :key="employe.id_employe"
            class="text-center overflow-hidden"
          >
            <td class="text-start">{{ employe.nom }}</td>
            <td class="text-start">{{ employe.prenom }}</td>
            <td>{{ employe.email }}</td>
            <td class="text-end">{{ employe.poste_Travail?.design }}</td>
            <td v-if="!actifFilter" class="text-end">Départ : {{ employe.date_depart }}</td>
            <td v-if="actifFilter" class="flex flex-row mt-1 justify-end">
              <button @click="editEmploye(employe)" class="bg-blue-500 text-slate-200 rounded p-1">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="10"
                  height="10"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="lucide lucide-pen-line"
                >
                  <path d="M12 20h9" />
                  <path
                    d="M16.376 3.622a1 1 0 0 1 3.002 3.002L7.368 18.635a2 2 0 0 1-.855.506l-2.872.838a.5.5 0 0 1-.62-.62l.838-2.872a2 2 0 0 1 .506-.854z"
                  />
                </svg>
              </button>
              <button
                @click="deleteEmploye(employe.id_employe)"
                class="bg-red-400 text-slate-200 rounded ml-2 p-1"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="10"
                  height="10"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="lucide lucide-trash"
                >
                  <path d="M3 6h18" />
                  <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6" />
                  <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" />
                </svg>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <ModifierEmploye
      v-if="isEditVisible"
      :employe="employeSelectionne!"
      :isModifier="isEditVisible"
      @modifier-employe="mettreAJourEmploye"
      @fermer="fermerModifierEmploye"
    />
  </div>
</template>

<style scoped>
.listeEmployes::-webkit-scrollbar {
  display: none;
}
</style>
