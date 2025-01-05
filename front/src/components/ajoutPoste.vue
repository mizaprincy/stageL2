<script setup lang="ts">
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toast-notification';
import { eventBus } from './events/eventBus'

interface PosteTravail {
  design: string
  salaire_horaire: number
  heures_travaillees: number
  description: string
}

const toast = useToast({position: 'top-right'})
const newPoste = ref<PosteTravail>({
  design: '',
  salaire_horaire: 0,
  heures_travaillees: 0,
  description: ''
})

function exitFormAjoutPoste() {
  const formAjoutPosteContainer = document.getElementById('formAjoutPosteContainer')!
  formAjoutPosteContainer.style.display = 'none'
  // Réinitialiser les données liées à v-model
  newPoste.value = {
    design: '',
    salaire_horaire: 0,
    heures_travaillees: 0,
    description: ''
  }
}

const backendUrl = import.meta.env.VITE_BACKEND_URL

//Ajout d'un poste
const AjoutPoste = async () => {
  try {
    if(newPoste.value.salaire_horaire == 0 || newPoste.value.heures_travaillees == 0){
      toast.error("Veuillez remplir tous les champs")
      return
    }
    const PosteData = {
      design: newPoste.value.design,
      salaire_horaire: newPoste.value.salaire_horaire,
      heures_travaillees: newPoste.value.heures_travaillees,
      description: newPoste.value.description
    }
    // Envoi de la requête POST
    const response = await axios.post(`${backendUrl}/PosteTravail`, PosteData)
    console.log('Poste ajouté avec succès', response.data)
    toast.success('Poste ajouté avec succès')
    eventBus.emit('PosteAdded')
    eventBus.emit('updateActivePosteCount')

    // Réinitialiser les champs après l'ajout
    newPoste.value = {
      design: '',
      salaire_horaire: 0,
      heures_travaillees: 0,
      description: ''
    }
  } catch (error: any) {
    console.error("Erreur lors de l'ajout du poste", error)
    console.log(newPoste.value)
    newPoste.value = {
      design: '',
      salaire_horaire: 0,
      heures_travaillees: 0,
      description: ''
    }
    toast.error(error.response.data.message)
  }
}

</script>

<template>
  <div
    id="formAjoutPosteContainer"
    class="formAjoutPosteContainer w-full h-screen absolute top-0 right-0 hidden"
  >
    <div
      class="exitFormAjoutPoste w-full h-screen bg-slate-900 opacity-50"
      @click="exitFormAjoutPoste()"
    ></div>
    <div
      class="formAjoutPosteContainer w-96 p-4 rounded-lg absolute top-[18%] left-1/4 z-10 bg-white flex flex-col items-center text-slate-800"
    >
      <h1 class="font-sans p-1 text-center font-semibold text-2xl">Ajout d'un poste de travail</h1>
      <span class="font-sans p-2 self-center text-xs">Veuillez remplir tous les champs</span>
      <form
        @submit.prevent="AjoutPoste()"
        id="formAjoutPoste"
        class="formAjoutPoste w-72 flex flex-col items-center font-sans text-xs"
      >
        <div class="design w-60 flex flex-row my-2 justify-between items-center">
          <label>Design :</label>
          <input
            type="text"
            required
            id="design"
            v-model="newPoste.design"
            placeholder="Designation"
            class="w-32 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="salaire w-60 flex flex-row my-2 justify-between items-center">
          <label>Salaire horaire (Ar):</label>
          <input
            type="number"
            required
            id="salaire_horaire"
            v-model="newPoste.salaire_horaire"
            class="w-32 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="heures w-60 flex flex-row my-2 justify-between items-center">
          <label>Heures de travail :</label>
          <input
            type="number"
            required
            id="heures_travaillees"
            v-model="newPoste.heures_travaillees"
            class="w-32 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>

        <div class="description w-60 flex flex-col my-2">
          <label for="description" class="text-sm font-sans font-medium text-slate-600"
            >Description :</label
          >
          <textarea
            required
            name="description"
            placeholder="Veuillez entrer une description ici..."
            rows="4"
            v-model="newPoste.description"
            id="description"
            class="w-full p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          >
          </textarea>
        </div>

        <div class="action-btn flex flex-row m-2">
          <button
            type="submit"
            class="ajoutEmploye-btn w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-700 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
          >
            Ajouter
          </button>
          <button
            type="button"
            @click="exitFormAjoutPoste()"
            class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
          >
            Annuler
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
