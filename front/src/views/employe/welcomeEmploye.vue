<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

export default defineComponent({
  name: 'WelcomeEmploye',
  setup() {
    const user = ref({ nom: '', prenom: '', email: '', role: '' })
    // Utilisation de Vue Router pour gérer l'URL
    const route = useRoute()
    const router = useRouter()

    onMounted(() => {
      const userData = sessionStorage.getItem('user')
      if (userData) {
        user.value = JSON.parse(userData)
      }
    })

    const getStarted = () => {
      router.push('/employe/apercu')
    }

    const showDescription = () => {
      router.push('/employe')
    }
    const Logout = () => {
      let confirmation = confirm('Voulez-vous terminer la session?')
      if (confirmation) {
        sessionStorage.removeItem('user')
        router.push('/')
      }
    }
    return {
      user,
      route,
      router,
      getStarted,
      showDescription,
      Logout
    }
  }
})
</script>

<template>
  <div
    class="w-full h-full p-6 bg-[url('../components/image/welcomeEmploye.webp')] bg-cover flex flex-col overflow-hidden relative"
  >
    <div class="description w-full flex flex-row justify-between items-center">
      <h1 class="font-sans text-slate-100 text-6xl font-semibold">
        Content de te revoir {{ user.prenom }}!
      </h1>
      <button @click="Logout()" class="Logout text-slate-200 font-medium p-2 rounded-xl bg-red-500">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="lucide lucide-power"
        >
          <path d="M12 2v10" />
          <path d="M18.4 6.6a9 9 0 1 1-12.77.04" />
        </svg>
      </button>
    </div>
    <section
      class="contenu w-[50%] h-[200%] flex flex-col self-center rounded-lg mt-10 p-6 bg-white overflow-y-auto"
    >
      <div v-if="route.path === '/employe'" class="welcome-message text-justify">
        <h1 class="text-3xl font-bold text-slate-600 text-center m-2">
          Bienvenue dans TeamUp!
        </h1>
        <h2 class="text-center font-semibold text-slate-500 mb-4">Votre application de gestion de paie et des employes</h2>
        <p class="text-gray-700 mb-4">
          Simplifiez la gestion de votre entreprise avec notre outil intuitif et puissant :
        </p>
        <ul class="list-disc list-inside text-gray-700 space-y-2">
          <li>
            <strong class="font-semibold">Suivi detaillés des paiements</strong> : Suivez les
            informations sur les différents paiements vous concernant.
          </li>
          <li>
            <strong class="font-semibold">Demande d'avance</strong> : Effectuez des demande d'avance
            a partir du quinzième jour du mois, ou demander une avance de type exceptionnelle a
            n'importe quel jour mais vous devez discuter avec le responsable.
          </li>
          <li>
            <strong class="font-semibold">Statistiques détaillées</strong> : Visualisez les données
            mensuelles sur les heures travaillées et les coûts salariaux.
          </li>
        </ul>
        <p class="text-gray-700 my-4">
          Optimisez votre temps et prenez des décisions éclairées grâce à notre solution adaptée à
          vos besoins professionnels.
        </p>
      </div>
      <button
        v-if="route.path === '/employe'"
        @click="getStarted"
        class="w-40 p-2 self-center rounded-sm bg-slate-200 m-2 font-medium hover:bg-slate-300 text-slate-700 transition-all"
      >
        Commencer
      </button>
      <div v-if="route.path !== '/employe'" class="w-full flex flex-row items-center">
        <button
          @click="showDescription"
          class="p-1 rounded bg-slate-200 font-medium hover:bg-slate-300 text-slate-700 transition-all"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="lucide lucide-chevron-left"
          >
            <path d="m15 18-6-6 6-6" />
          </svg>
        </button>
        <div
          class="action-btn w-[80%] self-center flex flex-row justify-evenly ml-4 rounded text-sm font-sans bg-blue-50 border-[1px] border-blue-200"
        >
          <RouterLink
            to="/employe/apercu"
            class="w-[50%] flex flex-row rounded-sm justify-center bg-slate-200 m-2"
          >
            <div
              type="button"
              class="apercu w-28 mt-2 p-2 rounded font-medium bg-slate-700 text-center text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
            >
              Aperçu
            </div>
          </RouterLink>
          <RouterLink
            to="/employe/service"
            class="w-[50%] flex flex-row rounded-sm justify-center bg-slate-200 m-2"
          >
            <div
              type="button"
              class="demandeAvance w-28 mt-2 p-2 rounded font-medium bg-slate-700 text-center text-slate-200 hover:bg-green-300 hover:text-slate-700 transition-all m-2"
            >
              Service
            </div>
          </RouterLink>
        </div>
      </div>
      <RouterView />
    </section>
  </div>
</template>

<style scoped>
.router-link-active {
  background-color: #94a3b8;
  color: #0f172a;
  transition: all 0.5s;
}
.contenu::-webkit-scrollbar {
  display: none;
}
</style>
