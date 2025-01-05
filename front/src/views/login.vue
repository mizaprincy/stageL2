<script lang="ts">
import router from '@/router'
import { ref } from 'vue'
import axios from 'axios'
import { useToast } from 'vue-toast-notification'
import { useGlobalStore } from '@/stores/globalStore'

interface Login {
  email: string
  password: string
}
interface User {
  Nom: string
  Prenom: string
  Email: string
  Password: string
  Confirm: string
  EmployeId: string
}

const newUser = ref<User>({
  Nom: '',
  Prenom: '',
  Email: '',
  Password: '',
  Confirm: '',
  EmployeId: ''
})

const backendUrl = import.meta.env.VITE_BACKEND_URL

const formemail = ref<string>('')
const formpassword = ref<string>('')
const isLoading = ref<boolean>(false)
const toast = useToast()

const hideLogin = () => {
  const contentContainer = document.getElementById('contentContainer')!
  contentContainer.style.left = '45%'
}
const showLogin = () => {
  const contentContainer = document.getElementById('contentContainer')!
  contentContainer!.style.left = '-55%'
}

//Ajout d'un utilisateur
const addUser = async () => {
  isLoading.value = true

  try {
    if (newUser.value.Password !== newUser.value.Confirm) {
      throw new Error('Verifiez bien votre mot de passe')
    }
    const globalStore = useGlobalStore()
    const userDataSignup = {
      Nom: newUser.value.Nom,
      Prenom: newUser.value.Prenom,
      Email: newUser.value.Email,
      Password: newUser.value.Password,
      EmployeId: newUser.value.EmployeId
    }
    // Envoi de la requête POST
    const response = await axios.post(`${backendUrl}/Auth/signup`, userDataSignup)
    const user = response.data.user
    const userData = JSON.stringify(user)
    if (userData) {
      globalStore.setUser(user)
    }
    console.log(globalStore.user)
    // Réinitialiser les champs après l'ajout
    newUser.value = {
      Nom: '',
      Prenom: '',
      Email: '',
      Password: '',
      Confirm: '',
      EmployeId: ''
    }
    router.push('/employe')
  } catch (error: any) {
    console.error('Erreur de la réponse :', error.response.data)
    if (error.response && error.response.data) {
      const data = error.response.data
      if (data.errors && typeof data.errors === 'object') {
        Object.keys(data.errors).forEach((key) => {
          const errorMessages = data.errors[key]
          if (Array.isArray(errorMessages)) {
            errorMessages.forEach((err: string) => {
              toast.error(err)
            })
          }
        })
      } else if (data.message) {
        toast.error(data.message)
      } else {
        toast.error('Une erreur inattendue est survenue.')
      }
    } else {
      // Gestion des erreurs réseau ou autres
      toast.error('Une erreur inattendue est survenue.')
    }
  } finally {
    isLoading.value = false
    newUser.value = {
      Nom: '',
      Prenom: '',
      Email: '',
      Password: '',
      Confirm: '',
      EmployeId: ''
    }
  }
}

//Login
const connexion = async () => {
  isLoading.value = true
  const globalStore = useGlobalStore()  
  const loginRequest: Login = {
    email: formemail.value,
    password: formpassword.value
  }

  try {
    const response = await axios.post(`${backendUrl}/Auth/login`, loginRequest)

    const user = response.data.user
    formemail.value = ''
    formpassword.value = ''
    const userData = JSON.stringify(user)
    if (userData) {
      globalStore.setUser(user)
    }
    if (globalStore.isAdmin) {
      router.push('/admin')
    } else {
      router.push('/employe')
    }
    console.log(globalStore.user)
  } catch (error: any) {
    console.error('Erreur de la réponse :', error.response.data)
    if (error.response && error.response.data) {
      const data = error.response.data
      if (data.errors && typeof data.errors === 'object') {
        Object.keys(data.errors).forEach((key) => {
          const errorMessages = data.errors[key]
          if (Array.isArray(errorMessages)) {
            errorMessages.forEach((err: string) => {
              toast.error(err)
            })
          }
        })
      } else if (data.message) {
        toast.error(data.message)
      } else {
        toast.error('Une erreur inattendue est survenue.')
      }
    } else {
      // Gestion des erreurs réseau ou autres
      toast.error('Une erreur inattendue est survenue.')
    }
  } finally {
    isLoading.value = false
  }
}

export default {
  setup() {
    return {   
      formemail,
      formpassword,
      isLoading,
      newUser,
      hideLogin,
      showLogin,
      connexion,
      addUser
    }
  }
}
</script>

<template>
  <div class="container w-full h-screen relative">
    <div class="content absolute top-1/2 left-1/2">
      <div
        class="contentContainer w-[40rem] min-h-80 flex flex-row justify-between rounded-xl bg-slate-100 overflow-hidden relative"
      >
        <div
          id="contentContainer"
          class="contentContainer w-full h-full flex flex-row justify-between absolute -left-[55%]"
        >
          <div class="signup min-w-80 max-h-full py-4 flex-col justify-center items-center m-4">
            <h1 class="font-sans text-2xl font-medium text-slate-700 text-center m-1">
              Creation de compte
            </h1>
            <form
              @submit.prevent="addUser()"
              class="loginForm flex flex-col justify-center items-center px-6 text-slate-800 font-sans text-xs"
            >
              <div class="identifiant w-60 m-1.5 flex flex-row justify-between">
                <input
                  type="text"
                  required
                  id="nom"
                  v-model="newUser.Nom"
                  placeholder="Nom"
                  class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
                />
                <input
                  type="text"
                  required
                  id="prenom"
                  v-model="newUser.Prenom"
                  placeholder="Prenom(s)"
                  class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
                />
              </div>
              <input
                type="text"
                required
                id="EmployeId"
                v-model="newUser.EmployeId"
                placeholder="Employe ID"
                class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400"
              />
              <input
                type="email"
                required
                id="email"
                v-model="newUser.Email"
                placeholder="Email"
                class="w-60 m-1.5 p-2 rounded-sm outline-none border-[1px] border-slate-400"
              />
              <div class="password w-60 m-1.5 flex flex-row justify-between">
                <input
                  type="password"
                  required
                  id="password"
                  v-model="newUser.Password"
                  placeholder="Password"
                  class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
                />
                <input
                  type="password"
                  required
                  id="confirmPassword"
                  v-model="newUser.Confirm"
                  placeholder="Confirm"
                  class="w-28 p-2 rounded-sm outline-none border-[1px] border-slate-400"
                />
              </div>
              <button
                type="submit"
                :disabled="isLoading"
                class="bg-blue-900 text-white p-2 rounded-md mt-2 text-base"
              >
                S'inscrire
                <span v-if="isLoading" class="w-10 ml-2 spinner-border border-1"></span>
              </button>
            </form>
          </div>
          <div class="cacheContainer min-w-[45%] min-h-full">
            <div
              class="cache min-w-[200%] min-h-full flex flex-row justify-between rounded-[4rem] overflow-hidden"
            >
              <div
                class="cachesignup w-[50%] min-h-full flex flex-col items-center justify-center bg-blue-950"
              >
                <h1 class="font-sans text-3xl font-medium text-slate-200">Bonjour !</h1>
                <h2 class="font-sans text-xs font-light text-slate-100 mt-2">
                  Veuillez entrer vos details personnels
                </h2>
                <button
                  @click="showLogin()"
                  class="cachebtn text-slate-200 text-xs p-2 rounded-md mt-10"
                >
                  SE CONNECTER
                </button>
              </div>
              <div
                class="cachelogin w-[50%] min-h-full flex flex-col items-center justify-center bg-blue-950"
              >
                <h1 class="font-sans text-3xl font-medium text-slate-200">Bienvenue !</h1>
                <h2 class="font-sans text-xs font-light text-slate-100 mt-2">
                  Veuillez vous connecter
                </h2>
                <button
                  @click="hideLogin()"
                  class="cachebtn text-slate-200 text-xs p-2 rounded-md mt-10"
                >
                  S'INSCRIRE
                </button>
              </div>
            </div>
          </div>

          <div class="login min-w-80 max-h-full flex flex-col items-center m-4">
            <h1 class="font-sans text-2xl font-medium text-slate-700 mt-10">Page de Connexion</h1>
            <form
              @submit.prevent="connexion"
              class="loginForm flex flex-col justify-center items-center mt-6"
            >
              <input
                type="email"
                name="email"
                v-model="formemail"
                required
                class="w-60 m-2 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-800 font-sans text-xs"
                placeholder="Email"
              />
              <input
                type="password"
                v-model="formpassword"
                name="password"
                required
                class="w-60 m-2 p-2 rounded-sm outline-none border-[1px] border-slate-400 text-slate-800 font-sans text-xs"
                placeholder="Mot de passe"
              />
              <button
                type="submit"
                :disabled="isLoading"
                class="bg-blue-900 text-white p-2 rounded-md mt-2 text-base"
              >
                Se connecter
                <span v-if="isLoading" class="w-10 ml-2 spinner-border border-1"></span>
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  background: linear-gradient(135deg, #374151, #1e3a8a);
}
#contentContainer {
  transition: left 0.5s ease;
}
.content {
  transform: translateX(-50%) translateY(-50%);
}
.signup {
  transform: translateX(-90%);
}
.cache {
  transform: translateX(-50%);
}
/* Spinner CSS */
.spinner-border {
  border: 2px solid #fff;
  border-top: 2px solid transparent;
  border-radius: 50%;
  width: 16px;
  height: 16px;
  animation: spin 1s linear infinite;
}
.cachebtn {
  box-shadow: 0px 0px 2px #e2e8f0;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
