<script setup lang="ts">
import { ref } from 'vue'
import axiosInstance from '@/config/axios'
import { type PosteTravail } from './interface/Employe'
import { useToast } from 'vue-toast-notification'
import { eventBus } from './events/eventBus'

interface Employe {
  nom: string
  prenom: string
  id_employe: string
  email: string
  tel: string
  embauche: Date | undefined
  id_poste: number
}

const toast = useToast({position: 'top-right'})
const newEmploye = ref<Employe>({
  nom: '',
  prenom: '',
  id_employe: '',
  email: '',
  tel: '',
  embauche: undefined,
  id_poste: 0
})

function exitFormAjoutEmploye() {
  const formAjoutEmployeContainer = document.getElementById('formAjoutEmployeContainer')!
  const formAjoutEmploye = document.getElementById('formAjoutEmploye')! as HTMLFormElement
  formAjoutEmployeContainer.style.display = 'none'
  formAjoutEmploye.reset()
}

// Liste des postes de travail
const postes = ref<PosteTravail[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Fonction pour récupérer les postes
const fetchPostes = async () => {
  loading.value = true
  error.value = null

  try {
    const response = await axiosInstance.get<PosteTravail[]>(`/PosteTravail`)
    postes.value = response.data
  } catch (err) {
    error.value = 'Erreur lors du chargement des postes de travail'
  } finally {
    loading.value = false
  }
}

fetchPostes()

//Ajout d'un employe
const AjoutEmploye = async () => {
  if (newEmploye.value.id_poste === 0) {
    toast.error('Veuillez choisir un poste de travail')
    newEmploye.value = {
      nom: '',
      prenom: '',
      id_employe: '',
      email: '',
      tel: '',
      embauche: undefined,
      id_poste: 0
    }
  }
  try {
    const embaucheDate = new Date(newEmploye.value.embauche!)
    const formattedDate = embaucheDate.toISOString().split('T')[0]

    const employeData = {
      nom: newEmploye.value.nom,
      prenom: newEmploye.value.prenom,
      id_employe: newEmploye.value.id_employe,
      email: newEmploye.value.email,
      tel: newEmploye.value.tel,
      date_embauche: formattedDate,
      id_poste: newEmploye.value.id_poste
    }
    // Envoi de la requête POST
    const response = await axiosInstance.post(`/Employe`, employeData)
    console.log('Employé ajouté avec succès', response.data)
    toast.success('Employé ajouté avec succès')
    eventBus.emit('employeAdded')
    eventBus.emit('updateActiveEmployeCount')

    // Réinitialiser les champs après l'ajout
    newEmploye.value = {
      nom: '',
      prenom: '',
      id_employe: '',
      email: '',
      tel: '',
      embauche: undefined,
      id_poste: 0
    }
  } catch (error: any) {
    console.error("Erreur lors de l'ajout de l'employé", error)
    newEmploye.value = {
      nom: '',
      prenom: '',
      id_employe: '',
      email: '',
      tel: '',
      embauche: undefined,
      id_poste: 0
    }
    toast.error(error.response.data.message)
  }
}
</script>

<template>
  <div
    id="formAjoutEmployeContainer"
    class="formAjoutEmployeContainer w-full h-screen absolute top-0 right-0 hidden"
  >
    <div
      class="exitFormAjoutEmploye w-full h-screen bg-slate-900 opacity-50"
      @click="exitFormAjoutEmploye()"
    ></div>
    <div
      class="formAjoutEmployeContainer w-96 p-4 rounded-lg absolute top-[18%] left-1/4 z-10 bg-white flex flex-col items-center text-slate-800"
    >
      <h1 class="font-sans p-1 text-center font-semibold text-2xl">Ajout d'un Employe</h1>
      <span class="font-sans p-2 self-center text-xs">Veuillez remplir tous les champs</span>
      <form
        @submit.prevent="AjoutEmploye()"
        id="formAjoutEmploye"
        class="formAjoutEmploye w-72 flex flex-col items-center font-sans text-xs"
      >
        <div class="identifiant w-60 m-1.5 flex flex-row justify-between">
          <input
            type="text"
            required
            id="nom"
            v-model="newEmploye.nom"
            placeholder="Nom"
            class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
          <input
            type="text"
            required
            id="prenom"
            v-model="newEmploye.prenom"
            placeholder="Prenom(s)"
            class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <input
          type="text"
          required
          id="id_employe"
          v-model="newEmploye.id_employe"
          placeholder="Identifiant"
          class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400"
        />
        <input
          type="email"
          required
          id="email"
          v-model="newEmploye.email"
          placeholder="Email"
          class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400"
        />
        <input
          type="text"
          required
          id="telephone"
          v-model="newEmploye.tel"
          placeholder="Contact"
          class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400"
        />

        <div class="dateEmbauche w-60 flex flex-col my-2">
          <span class="text-sm font-sans font-medium text-slate-600">Embauche:</span>
          <input
            required
            type="date"
            name="embauche"
            v-model="newEmploye.embauche"
            id="embauche"
            class="w-36 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          />
        </div>

        <select
          name="poste"
          required
          id="poste"
          v-model="newEmploye.id_poste"
          class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
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
        <div class="action-btn flex flex-row m-2">
          <button
            type="submit"
            class="ajoutEmploye-btn w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-700 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
          >
            Ajouter
          </button>
          <button
            type="button"
            @click="exitFormAjoutEmploye()"
            class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
          >
            Annuler
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
