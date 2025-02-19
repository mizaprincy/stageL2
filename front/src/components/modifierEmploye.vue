<script lang="ts">
import { ref, watch, defineComponent, type PropType } from 'vue'
import axiosInstance from '@/config/axios';
import { type Employe, type PosteTravail } from './interface/Employe'

export default defineComponent({
  props: {
    employe: {
      type: Object as PropType<Employe>,
      required: true
    },
    isModifier: {
      type: Boolean,
      required: true
    }
  },
  setup(props, { emit }) {
    // Création d'une copie réactive de `employe`
    const localEmploye = ref({ ...props.employe })

    // Met à jour `localEmploye` si `employe` change dans le parent
    watch(
      () => props.employe,
      (newEmploye) => {
        localEmploye.value = { ...newEmploye }
      }
    )

    const submitModification = () => {
      // Emet l'événement avec la copie modifiée
      emit('modifier-employe', localEmploye.value)
    }

    const annulerModification = () => {
      emit('fermer')
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

    return {
      localEmploye,
      submitModification,
      annulerModification,
      postes
    }
  }
})
</script>
<template>
  <div v-if="isModifier" class="modifier-employe w-full h-screen absolute top-0 left-0">
    <div
      class="exitFormAjoutEmploye w-full h-screen bg-slate-900 opacity-50"
      @click="annulerModification()"
    ></div>
    <div
      class="container w-96 p-4 rounded-lg absolute top-[18%] left-1/4 z-10 bg-white flex flex-col items-center text-slate-800"
    >
      <h2 class="font-sans m-2 p-1 text-center font-semibold text-2xl">Modifier l'employé</h2>
      <form
        @submit.prevent="submitModification"
        class="formAjoutEmploye w-72 flex flex-col items-center font-sans text-xs"
      >
        <div class="profile flex flex-col my-2">
          <label>Profile</label>
          <div class="identifiant w-60 my-2 flex flex-row justify-between">
            <input
              type="text"
              required
              id="nom"
              v-model="localEmploye.nom"
              placeholder="Nom"
              class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
            <input
              type="text"
              required
              id="prenom"
              v-model="localEmploye.prenom"
              placeholder="Prenom(s)"
              class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
          </div>
        </div>
        <div class="contact flex flex-col mb-2">
          <label>Contact</label>
          <input
            type="text"
            required
            id="tel"
            v-model="localEmploye.tel"
            placeholder="Contact"
            class="w-60 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="PosteTravail flex flex-col mb-2">
          <label>Poste de travail</label>
          <select
            name="poste"
            required
            id="poste"
            v-model="localEmploye.id_poste"
            class="w-60 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          >
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
        <div class="action-btn flex flex-row m-2">
          <button
            type="submit"
            class="enregistrer w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-700 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
          >
            Enregistrer
          </button>
          <button
            type="button"
            @click="annulerModification()"
            class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
          >
            Annuler
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
