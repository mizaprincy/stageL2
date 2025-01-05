<script lang="ts">
import { defineComponent, ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toast-notification'
import { useGlobalStore } from '@/stores/globalStore'

export default defineComponent({
  name: 'UserProfile',
  setup() {
    const backendUrl = import.meta.env.VITE_BACKEND_URL
    const toast = useToast()

    const globalStore = useGlobalStore()
    const { user } = globalStore

    const demandeAvance = ref<{
      employeId: string | null | undefined
      montant: number
      nombreTranches: number
      typeAvance: string
    }>({
      employeId: user?.employeId,
      montant: 0,
      nombreTranches: 1,
      typeAvance: 'quinzaine'
    })
    const confirmDemande = async () => {
      try {
        if (demandeAvance.value.montant <= 0) {
          toast.error('Le montant doit etre superieur à 0', {
            position: 'top-right'
          })
          return
        }
        const email = user?.email
        const demandeData = {
          employeId: demandeAvance.value.employeId,
          montant: demandeAvance.value.montant,
          nombreTranches: demandeAvance.value.nombreTranches,
          typeAvance: demandeAvance.value.typeAvance
        }
        // Envoi de la requête POST
        const response = await axios.post(
          `${backendUrl}/DemandeAvance/employe/demander?email=${email}`,
          demandeData
        )
        console.log('Demande effectue avec succes', response.data)
        toast.success('Demande effectuée avec succès', {
          position: 'top-right'
        })

        // Réinitialiser les champs après l'ajout
        demandeAvance.value = {
          employeId: user?.employeId,
          montant: 0,
          nombreTranches: 1,
          typeAvance: 'quinzaine'
        }
      } catch (error: any) {
        console.error('Erreur lors de la demande', error)
        // Réinitialiser les champs après l'ajout
        demandeAvance.value = {
          employeId: user?.employeId,
          montant: 0,
          nombreTranches: 1,
          typeAvance: 'quinzaine'
        }
        toast.error(error.response.data.message, {
          position: 'top-right'
        })
      }
    }

    const resetDemande = () => {
      demandeAvance.value = {
        employeId: user?.employeId,
        montant: 0,
        nombreTranches: 1,
        typeAvance: 'quinzaine'
      }
    }
    return {
      user,
      demandeAvance,
      confirmDemande,
      resetDemande
    }
  }
})
</script>
<template>
  <div>
    <div class="profileContent w-full flex flex-col">
      <form
        @submit.prevent="confirmDemande()"
        id="formAjoutEmploye"
        class="formAjoutEmploye w-full h-64 self-center px-4 flex flex-col justify-between font-sans text-sm text-slate-700"
      >
        <div class="champs">
          <div class="dateEmbauche w-full flex flex-row justify-between items-center mt-4">
            <label class="font-semibold">Votre ID :</label>
            <input
              type="text"
              disabled
              required
              :value="demandeAvance.employeId"
              class="w-32 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
            />
          </div>
          <div class="montant w-full flex flex-row justify-between items-center my-4">
            <label class="font-semibold">Montant :</label>
            <input
              required
              type="number"
              v-model="demandeAvance.montant"
              min="0"
              class="w-32 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
            />
          </div>
          <div class="typeAvance w-full flex flex-row justify-between items-center mb-4">
            <label class="font-semibold">Type :</label>

            <select
              required
              v-model="demandeAvance.typeAvance"
              class="w-32 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
            >
              <option value="quinzaine" selected>Quinzaine</option>
              <option value="exceptionnelle">Exceptionnelle</option>
            </select>
          </div>
          <div
            v-if="demandeAvance.typeAvance === 'exceptionnelle'"
            class="nombreTranches w-full flex flex-row justify-between items-center"
          >
            <label class="font-semibold">Nombre de Tranches :</label>
            <input
              required
              type="number"
              v-model="demandeAvance.nombreTranches"
              min="1"
              class="w-14 p-1.5 rounded-sm outline-none border-[1px] border-slate-400 text-slate-600"
            />
          </div>
        </div>

        <div class="action-btn flex flex-row m-2">
          <button
            type="submit"
            class="ajoutEmploye-btn w-20 mt-2 p-2 text-xs rounded font-sans font-medium bg-slate-700 text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
          >
            Confirmer
          </button>
          <button
            type="button"
            @click="resetDemande()"
            class="annuler w-20 mt-2 p-2 text-xs rounded font-sans font-medium hover:bg-red-400 hover:text-slate-200 text-slate-700 transition-all m-2"
          >
            Annuler
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
