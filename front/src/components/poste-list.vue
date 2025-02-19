<script setup lang="ts">
import { ref, onBeforeUnmount, onMounted } from 'vue'
import axiosInstance from '@/config/axios'
import { type PosteTravail } from './interface/Employe'
import { useToast } from 'vue-toast-notification'
import { eventBus } from './events/eventBus'
import '@vuepic/vue-datepicker/dist/main.css'
import ModifierPoste from './modifierPoste.vue'

const toast = useToast({position: 'top-right'})
// Liste d'employés
const postes = ref<PosteTravail[]>([])
const loading = ref(false)

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

// Delete Poste
const deletePoste = async (id_poste: number) => {
  const confirmation = confirm(
    `Êtes-vous sûr de vouloir supprimer ce poste de travail: ID = ${id_poste} ?`
  )
  if (!confirmation) return

  try {
    await axiosInstance.delete(`/Poste/${id_poste}`)
    // Mettre à jour la liste des postes après suppression
    await fetchPostes()
    toast.success(`Poste ID = ${id_poste} supprimé`)
    eventBus.emit('updateActiveEmployeCount')
  } catch (err : any) {
    toast.error(err.response.data.message)
    console.error(err)
  }
}

//Edit Employe
const isEditVisible = ref(false)
const posteSelectionne = ref<PosteTravail>()

const editPoste = (poste: PosteTravail) => {
  posteSelectionne.value = { ...poste } // Cloner l'objet poste
  isEditVisible.value = true
}

const mettreAJourPoste = async (poste: PosteTravail) => {
  const id = poste.id_poste
  console.log(poste)
  try {
    // Appel de l'API avec la méthode PUT
    const response = await axiosInstance.put(`/PosteTravail/${id}`, poste)

    // Vérifier le statut de la réponse
    if (response.status === 204) {
      toast.success(`Poste ID = ${id} mis à jour avec succès !`)
      fetchPostes()
    } else {
      console.error("Erreur lors de la mise à jour de l'employé", response.status)
    }
  } catch (err : any) {
    toast.error(err.response.data.message)
    console.error(err)
  }
  fermerModifierPoste() // Cacher le formulaire après mise à jour
}

const fermerModifierPoste = () => {
  isEditVisible.value = false
}

//fetchEmployes apres un ajout
function handlePosteAdded() {
  fetchPostes()
  eventBus.emit('updateActivePosteCount')
}

onMounted(() => {
  eventBus.on('PosteAdded', handlePosteAdded)
})

onBeforeUnmount(() => {
  eventBus.off('PosteAdded', handlePosteAdded)
})

</script>
<template>
  <div class="employee-table w-full mt-4">
    <!-- Affichage du chargement -->
    <div v-if="loading" class="chargement w-full text-center text-slate-300 text-sm">
      Chargement...
    </div>
    <div class="listeEmployes w-full">
      <div v-if="!postes.length" class="AucunEmploye w-full text-center text-slate-300 text-sm">
        Aucun employé à afficher.
      </div>
      <table class="w-full text-slate-300">
        <tbody class="w-full text-xs h-4 overflow-hidden">
          <tr
            v-for="poste in postes"
            :key="poste.id_poste"
            class="text-center overflow-hidden"
          >
            <td class="text-start">{{ poste.id_poste }}</td>
            <td class="text-start">{{ poste.design }}</td>
            <td>{{ poste.salaire_horaire }}</td>
            <td class="text-end">{{ poste.heures_travaillees }}</td>
            <td class="flex flex-row mt-1 justify-end">
              <button @click="editPoste(poste)" class="bg-blue-500 text-slate-200 rounded p-1">
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
                @click="deletePoste(poste.id_poste)"
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
    <ModifierPoste
      v-if="isEditVisible"
      :poste="posteSelectionne!"
      :isModifier="isEditVisible"
      @modifier-poste="mettreAJourPoste"
      @fermer="fermerModifierPoste"
    />
  </div>
</template>

<style scoped>
.listeEmployes::-webkit-scrollbar {
  display: none;
}
</style>
