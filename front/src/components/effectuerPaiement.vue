<script setup lang="ts">
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toast-notification'
import '@vuepic/vue-datepicker/dist/main.css'
import PaiementConfirm from './paiementConfirm.vue'
import { eventBus } from './events/eventBus'

const backendUrl = import.meta.env.VITE_BACKEND_URL
const toast = useToast({position: 'top-right'})

const salaire = ref<{
  id_employe: string
  date_paiement: Date | null
  mois: number
  annee: number
}>({
  id_employe: '',
  date_paiement: null,
  mois: 0,
  annee: new Date().getFullYear()
})

const months = [
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

//Confirmation du paiement
const isPaiementVisible = ref(false)
const paiementdata = ref<{
  id_employe: string
  date_paiement: Date | null
  mois: number
  annee: number
}>()

const confirmPaiement = (paiement: {
  id_employe: string
  date_paiement: Date | null
  mois: number
  annee: number
}) => {
  if (!paiement.date_paiement) {
    toast.error('Veuillez choisir la date de paiement')
    return 
  }
  // Regex pour valider l'année (4 chiffres)
  const yearPattern = /^[0-9]{4}$/

  // Vérification si le mois et l'année sont sélectionnés
  if (!paiement.mois) {
    toast.error('Veuillez sélectionner un mois.')
    return
  }

  // Validation de l'année
  if (!yearPattern.test(paiement.annee.toString())) {
    toast.error("Pattern invalide pour l'année : Veuillez entrer une année valide au format 'yyyy'.")
    return // Interrompre le traitement si l'année est invalide
  }
  paiementdata.value = { ...paiement } // Cloner l'objet paiement
  isPaiementVisible.value = true
}

const submitPaiement = async (paiement: {
  id_employe: string
  date_paiement: Date | null
  mois: number
  annee: number
}) => {
  try {
    const DatePaiement = new Date(paiement.date_paiement!)
    const formattedDate = DatePaiement.toISOString().split('T')[0]

    const salaireData = {
      id_employe: paiement.id_employe,
      date_paiement: formattedDate,
      mois: paiement.mois,
      annee: paiement.annee
    }
    // Envoi de la requête POST
    const response = await axios.post(`${backendUrl}/Salaire`, salaireData)
    console.log('Paiement effectuee avec succes', response.data)
    toast.success('Paiement effectuee avec succes')
    eventBus.emit('paiementAdded')

    // Réinitialiser les champs après l'ajout
    salaire.value = {
      id_employe: '',
      date_paiement: null,
      mois: 0,
      annee: 2024
    }
  } catch (error: any) {
    console.error('Erreur lors du paiement', error)
    salaire.value = {
      id_employe: '',
      date_paiement: null,
      mois: 0,
      annee: 2024
    }
    toast.error(error.response.data.message)
  }
  fermerPaiementConfirm()
}

const fermerPaiementConfirm = () => {
  isPaiementVisible.value = false
}

const resetPaiement = () => {
  salaire.value = {
    id_employe: '',
    date_paiement: null,
    mois: 0,
    annee: 2024
  }
}
</script>

<template>
  <div class="employee-table w-full">
    <form
      @submit.prevent="confirmPaiement(salaire)"
      id="formAjoutEmploye"
      class="formAjoutEmploye w-full flex flex-col items-center font-sans text-sm text-slate-700"
    >
      <div class="dateEmbauche w-64 flex flex-row justify-between items-center my-2">
        <label>Employe ID :</label>
        <input
          type="text"
          required
          id="idEmploye"
          v-model="salaire.id_employe"
          placeholder="Employe ID"
          class="w-40 my-2 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
        />
      </div>
      <div class="dateEmbauche w-64 flex flex-row justify-between items-center my-4">
        <label>Date de Paiement :</label>
        <input
          required
          type="date"
          name="datePaiement"
          v-model="salaire.date_paiement"
          id="date_paiement"
          class="w-32 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
        />
      </div>
      <div class="Salaire flex flex-col my-4">
        <label>Salaire du :</label>
        <div class="w-64 flex flex-row justify-between my-2">
          <select
            id="month-select"
            required
            v-model="salaire.mois"
            class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          >
            <option value="0" disabled selected hidden>Mois</option>
            <option v-for="(name, index) in months" :key="index" :value="index + 1">
              {{ name }}
            </option>
          </select>
          <input
            id="year-input"
            type="number"
            required
            v-model="salaire.annee"
            placeholder="Annee"
            min="2000"
            max="3000"
            class="w-32 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
          />
        </div>
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
          @click="resetPaiement()"
          class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
        >
          Annuler
        </button>
      </div>
    </form>
    <PaiementConfirm
      v-if="isPaiementVisible"
      :paiement="paiementdata!"
      :isPaiement="isPaiementVisible"
      @effectuer-paiement="submitPaiement"
      @fermer="fermerPaiementConfirm"
    />
  </div>
</template>
