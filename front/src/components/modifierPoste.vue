<script lang="ts">
import { ref, watch, defineComponent, type PropType } from 'vue'
import { type PosteTravail } from './interface/Employe'

export default defineComponent({
  props: {
    poste: {
      type: Object as PropType<PosteTravail>,
      required: true
    },
    isModifier: {
      type: Boolean,
      required: true
    }
  },
  setup(props, { emit }) {
    // Création d'une copie réactive de `employe`
    const localPoste = ref({ ...props.poste })

    // Met à jour `localEmploye` si `employe` change dans le parent
    watch(
      () => props.poste,
      (newPoste) => {
        localPoste.value = { ...newPoste }
      }
    )

    const submitModification = () => {
      // Emet l'événement avec la copie modifiée
      emit('modifier-poste', localPoste.value)
    }

    const annulerModification = () => {
      emit('fermer')
    }

    return {
      localPoste,
      submitModification,
      annulerModification
    }
  }
})
</script>
<template>
  <div v-if="isModifier" class="modifier-poste w-full h-screen absolute top-0 left-0">
    <div
      class="exitFormAjoutPoste w-full h-screen bg-slate-900 opacity-50"
      @click="annulerModification()"
    ></div>
    <div
      class="container w-96 p-4 rounded-lg absolute top-[18%] left-1/4 z-10 bg-white flex flex-col items-center text-slate-800"
    >
      <h2 class="font-sans m-2 p-1 text-center font-semibold text-2xl">
        Modifier le poste de travail
      </h2>
      <form
        @submit.prevent="submitModification"
        class="formAjoutPoste w-72 flex flex-col items-center font-sans text-xs"
      >
        <div class="design w-60 flex flex-row my-2 justify-between items-center">
          <label>Design :</label>
          <input
            type="text"
            required
            id="design"
            v-model="localPoste.design"
            placeholder="Designation"
            class="w-32 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="salaire w-60 flex flex-row mb-2 justify-between items-center">
          <label>Salaire horaire :</label>
          <input
            type="number"
            required
            id="salaire_horaire"
            v-model="localPoste.salaire_horaire"
            class="w-32 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="heures w-60 flex flex-row mb-2 justify-between items-center">
          <label>Heures de travail :</label>
          <input
            type="number"
            required
            id="heures_travaillees"
            v-model="localPoste.heures_travaillees"
            class="w-32 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="description w-60 flex flex-col my-2">
          <label for="description" class="text-sm font-sans font-medium text-slate-600"
            >Description :</label
          >
          <textarea
            required
            name="description"
            rows="4"
            v-model="localPoste.description"
            id="description"
            class="w-full p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          >
          </textarea>
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
