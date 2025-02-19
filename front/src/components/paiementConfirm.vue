<script lang="ts">
import { ref, watch, defineComponent, type PropType } from 'vue'
import axiosInstance from '@/config/axios';

export default defineComponent({
  props: {
    paiement: {
      type: Object as PropType<{
        id_employe: string
        date_paiement: Date | null
        mois: number
        annee: number
      }>,
      required: true
    },
    isPaiement: {
      type: Boolean,
      required: true
    }
  },
  setup(props, { emit }) {
    // Création d'une copie réactive de `employe`
    const localPaiement = ref({ ...props.paiement })
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

    // Met à jour `localEmploye` si `employe` change dans le parent
    watch(
      () => props.paiement,
      (newPaiement) => {
        localPaiement.value = { ...newPaiement }
      }
    )

    const effectuerPaiement = () => {
      // Emet l'événement avec la copie modifiée
      emit('effectuer-paiement', localPaiement.value)
    }

    const annulerPaiement = () => {
      emit('fermer')
    }

    const backendUrl = import.meta.env.VITE_BACKEND_URL
    // Liste des postes de travail
    const detailsPaiement = ref({
      annee: 0,
      avance: {
        montantAvance: 0,
        typeAvance: '',
        trancheActuelle: 0,
        nombreTranches: 0
      },
      datePaiement: '',
      employeId: '',
      employeNom: '',
      employePoste: '',
      employePrenom: '',
      mois: 0,
      montantTotal: 0
    })
    const loading = ref(false)
    const error = ref<string | null>(null)

    // Fonction pour récupérer les postes
    const fetchDetails = async () => {
      loading.value = true
      error.value = null

      try {
        const response = await axiosInstance.get(`${backendUrl}/Salaire/paiement/details`, {
          params: {
            id_employe: localPaiement.value.id_employe,
            datePaiement: localPaiement.value.date_paiement,
            mois: localPaiement.value.mois,
            annee: localPaiement.value.annee
          }
        })
        detailsPaiement.value = response.data
      } catch (err) {
        error.value = 'Erreur lors de la recuperation des details du paiement'
      } finally {
        loading.value = false
      }
    }

    fetchDetails()

    return {
      localPaiement,
      moisEnLettres,
      effectuerPaiement,
      annulerPaiement,
      loading,
      error,
      detailsPaiement
    }
  }
})
</script>
<template>
  <div v-if="isPaiement" class="modifier-employe w-full h-screen absolute top-0 left-0">
    <div
      class="exitFormAjoutEmploye w-full h-screen bg-slate-900 opacity-50"
      @click="annulerPaiement()"
    ></div>
    <div
      class="container w-96 p-4 rounded-lg absolute top-[10%] left-1/4 z-10 bg-white flex flex-col items-center text-slate-800"
    >
      <h2 class="font-sans m-2 p-1 text-center font-semibold text-2xl">Confirmer le paiement</h2>
      <div v-if="loading">Chargement...</div>
      <div v-else-if="!detailsPaiement">Aucune donnée trouvée.</div>
      <form
        v-else
        @submit.prevent="effectuerPaiement"
        class="formAjoutEmploye w-72 flex flex-col items-center font-sans text-xs"
      >
        <div class="profile flex flex-col mt-2">
          <label>Profile</label>
          <div class="identifiant w-60 my-2 flex flex-row justify-between">
            <input
              type="text"
              disabled
              id="nom"
              :value="detailsPaiement?.employeNom"
              class="nom w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
            <input
              type="text"
              disabled
              id="prenom"
              :value="detailsPaiement?.employePrenom"
              class="prenom w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
          </div>
        </div>
        <div class="Poste flex flex-col mb-2">
          <label>Poste de travail</label>
          <input
            type="text"
            disabled
            id="poste"
            :value="detailsPaiement?.employePoste"
            class="w-60 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="w-60 flex flex-row justify-between items-center mb-2">
          <label>Date :</label>
          <input
            type="text"
            disabled
            id="mois"
            :value="moisEnLettres[detailsPaiement.mois - 1]"
            class="w-20 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
          <input
            type="number"
            disabled
            id="annee"
            :value="detailsPaiement?.annee"
            class="w-24 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="MontantTotal w-60 flex flex-row justify-between items-center mb-2">
          <label>Montant Totale :</label>
          <input
            type="text"
            disabled
            id="montantTotal"
            :value="`${detailsPaiement?.montantTotal} Ar`"
            class="w-36 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="Avance flex flex-col mb-2">
          <div class="header w-60 flex flex-row justify-between items-center">
            <label>Avance pour ce mois :</label>
            <input
              v-if="detailsPaiement?.avance.typeAvance == 'exceptionnelle'"
              type="text"
              disabled
              id="trancheActuelle"
              :value="`${detailsPaiement?.avance.trancheActuelle}/${detailsPaiement!.avance.nombreTranches}`"
              class="w-10 text-center p-1 font-semibold rounded-sm outline-none border-[1px] border-slate-400"
            />
          </div>
          <div class="w-60 flex flex-row justify-between">
            <input
              type="text"
              disabled
              id="typeAvance"
              :value="detailsPaiement?.avance.typeAvance"
              class="w-24 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
            <input
              type="text"
              disabled
              id="montantAvance"
              :value="`${detailsPaiement?.avance.montantAvance} Ar`"
              class="w-32 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400"
            />
          </div>
        </div>
        <div class="MontantNet w-60 flex flex-row justify-between items-center mb-2">
          <label>Montant Net :</label>
          <input
            type="text"
            disabled
            id="montantNet"
            :value="`${detailsPaiement?.montantTotal - detailsPaiement.avance.montantAvance} Ar`"
            class="w-36 p-2 rounded-sm font-semibold outline-none border-[1px] border-slate-400"
          />
        </div>
        <div class="action-btn flex flex-row m-2">
          <button
            type="submit"
            class="enregistrer w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-700 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
          >
            Confirmer
          </button>
          <button
            type="button"
            @click="annulerPaiement()"
            class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
          >
            Annuler
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<style scoped>
label{
  font-weight: 500;
}
</style>
