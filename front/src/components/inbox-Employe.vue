<script lang="ts">
import { defineComponent, ref, onMounted, onBeforeUnmount } from 'vue'
import type { Inbox } from '@/components/interface/Employe'
import axiosInstance from '@/config/axios'
import { useGlobalStore } from '@/stores/globalStore'
import router from '@/router'
import { eventBus } from '@/components/events/eventBus'
import { useToast } from 'vue-toast-notification'

export default defineComponent({
  name: 'InboxAdmin',
  setup(_, { emit }) {
    const globalStore = useGlobalStore()
    const inboxes = ref<Inbox[]>([])
    const loading = ref(false)
    const error = ref<string | null>(null)
    const toast = useToast()

    const fetchInbox = async () => {
      loading.value = true
      error.value = null
      if (!(globalStore && globalStore.isLoggedIn)) {
        console.error('User non trouve')
        router.push('/')
        return
      }
      try {
        const response = await axiosInstance.get<Inbox[]>(
          `/Inbox/employe?email=${globalStore.user?.email}`
        )
        inboxes.value = response.data
        emit('nbInbox', inboxes.value.filter((i) => !i.isRead).length)
      } catch (err) {
        console.error(err)
        error.value = 'Erreur lors du chargement des messages'
      } finally {
        loading.value = false
      }
    }

    fetchInbox()

    const markAsRead = async (inboxId: number) => {
      try {
        await axiosInstance.put(`/Inbox/markAsRead/${inboxId}`)
        fetchInbox()
      } catch (err) {
        console.error(err)
        toast.error("Une erreur s'est produite")
      }
    }

    onMounted(() => {
      eventBus.on('refreshInbox', fetchInbox) // Écouter l'événement
    })
    onBeforeUnmount(() => {
      eventBus.off('refreshInbox', fetchInbox) // Retirer l'écouteur lorsque le composant est détruit
    })

    return {
      loading,
      error,
      markAsRead,
      inboxes
    }
  }
})
</script>
<template>
  <div>
    <!-- Affichage du chargement -->
    <div v-if="loading" class="chargement w-full text-center text-slate-600 text-sm">
      Chargement...
    </div>

    <div
      v-if="!inboxes.length"
      class="aucuneNotification w-full text-center text-slate-600 text-sm"
    >
      Aucune notification.
    </div>
    <div
      v-for="inbox in inboxes"
      :key="inbox.inboxId"
      class="inbox w-full flex flex-col text-xs font-sans text-slate-600 p-2 my-2 hover:bg-gray-200 cursor-pointer rounded overflow-hidden"
    >
      <div class="flex flex-row justify-between">
        <label class="text-slate-600 font-medium cursor-pointer">{{
          inbox.dateNotification
        }}</label>
        <button
          v-if="!inbox.isRead"
          @click="markAsRead(inbox.inboxId)"
          class="read flex flex-row justify-center p-1 rounded bg-blue-500 text-slate-200"
        >
          Read
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
            class="lucide lucide-check"
          >
            <path d="M20 6 9 17l-5-5" />
          </svg>
        </button>
        <span v-else class="text-slate-400">
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
            class="lucide lucide-check-check"
          >
            <path d="M18 6 7 17l-5-5" />
            <path d="m22 10-7.5 7.5L13 16" />
          </svg>
        </span>
      </div>
      <label class="cursor-pointer">{{ inbox.message }}</label>
    </div>
  </div>
</template>

<style scoped>
.decliner {
  box-shadow: 0px 0px 1px #e2e8f0;
}
</style>
