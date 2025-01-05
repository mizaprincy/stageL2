<script lang="ts">
import { defineComponent, ref } from 'vue'
import type { Inbox } from '@/components/interface/Employe'
import axios from 'axios'
import { useGlobalStore } from '@/stores/globalStore';

export default defineComponent({
  name: 'InboxAdmin',
  setup() {
    const globalStore = useGlobalStore()
    const backendUrl = import.meta.env.VITE_BACKEND_URL
    const inboxes = ref<Inbox[]>([])
    const loading = ref(false)
    const error = ref<string | null>(null)
    const fetchInbox = async () => {
      loading.value = true
      error.value = null
      if (!(globalStore && globalStore.isLoggedIn)) {
        console.error('User non trouve')
        return
      }
      console.log(globalStore.user)
      try {
        const response = await axios.get<Inbox[]>(
          `${backendUrl}/Inbox/admin?email=${globalStore.user?.email}`
        )
        inboxes.value = response.data
      } catch (err) {
        error.value = 'Erreur lors du chargement des employés'
      } finally {
        loading.value = false
      }
    }
    fetchInbox()

    const repondreDemande = async (demandeId: number, statut: string) => {
      const confirmation = confirm(`Voulez-vous que catte demande soit ${statut}`)
      if (!confirmation) {
        return
      }
      const email = globalStore.user?.email
      const payload = {
        demandeId,
        statut,
        email
      }
      try {
        const response = await axios.patch(`${backendUrl}/DemandeAvance/admin/repondre`, payload)

        if (response.status === 200) {
          console.log('Réponse envoyée avec succès!', response.data)
          fetchInbox() // Rafraîchissement de la boîte de réception
        } else {
          console.error("Erreur lors de l'envoi de la réponse", response.status)
        }
      } catch (err) {
        console.error('Erreur réseau ou serveur', err)
      }
    }

    return {
      loading,
      error,
      repondreDemande,
      inboxes
    }
  }
})
</script>
<template>
  <div>
    <!-- Affichage du chargement -->
    <div v-if="loading" class="chargement w-full text-center text-slate-200 text-sm">
      Chargement...
    </div>

    <div
      v-if="!inboxes.length"
      class="aucuneNotification w-full text-center text-slate-200 text-sm"
    >
      Aucune notification.
    </div>
    <div
      v-for="inbox in inboxes"
      :key="inbox.inboxId"
      class="inbox w-full flex flex-col text-xs font-sans font-medium text-slate-300 p-2 my-2 hover:bg-slate-700 rounded overflow-hidden"
    >
      <label
        ><span class="text-slate-100"
          >{{ inbox.demandeAvance.employe?.nom }} {{ inbox.demandeAvance.employe?.prenom }}</span
        >
        {{ inbox.message }}</label
      >
      <label class="text-[0.65rem] text-slate-400">.{{ inbox.dateNotification }}</label>

      <div class="w-full flex justify-between">
        <label
          :class="{
            'bg-blue-200 text-blue-800 border-blue-500':
              inbox.demandeAvance.statut === 'En attente',
            'bg-red-200 text-red-800 border-red-500': inbox.demandeAvance.statut === 'Déclinée',
            'bg-green-200 text-green-800 border-green-500':
              inbox.demandeAvance.statut === 'Approuvée'
          }"
          class="bg-blue-200 h-[1.2rem] px-1 rounded-xl text-[0.7rem] text-blue-800 border-blue-500 border-[1px] my-1"
          >{{ inbox.demandeAvance.statut }}</label
        >
        <div
          v-if="inbox.demandeAvance.statut === 'En attente'"
          class="approbation w-[35%] inline-flex justify-between text-slate-200"
        >
          <button
            @click="repondreDemande(inbox.demandeId, 'Déclinée')"
            class="decliner p-1 rounded hover:bg-red-400 hover:text-slate-200 transition-all"
          >
            Decliner
          </button>
          <button
            @click="repondreDemande(inbox.demandeId, 'Approuvée')"
            class="approuver p-1 rounded bg-slate-800 hover:bg-green-300 hover:text-slate-700 transition-all"
          >
            Accepter
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.decliner {
  box-shadow: 0px 0px 1px #e2e8f0;
}
</style>
