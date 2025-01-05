import './assets/style.css'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import Vue3Lottie from 'vue3-lottie'
import 'vue-toast-notification/dist/theme-bootstrap.css'
import VueToast from 'vue-toast-notification'
const app = createApp(App)
const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)
app.use(pinia)
app.use(VueToast)
app.use(router)
app.use(Vue3Lottie)

app.mount('#app')
