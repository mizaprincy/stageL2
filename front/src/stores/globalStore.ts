import { defineStore } from 'pinia';

interface User {
  nom: string
  prenom: string
  email: string
  role: string
  employeId: string | null
}
export const useGlobalStore = defineStore('global', {
  state: () => ({
    user: null as User | null, // Contient les informations de l'utilisateur connecté
    isAdmin: false, // Indique si l'utilisateur est admin
  }),
  persist: true,
  getters: {
    isLoggedIn: (state) => !!state.user, // Vérifie si un utilisateur est connecté
  },
  actions: {
    setUser(user: User) {
      this.user = user;
      this.isAdmin = user?.role === 'Admin';
    },
    logout() {
      this.user = null;
      this.isAdmin = false;
      sessionStorage.clear()
      console.log("clear data")
    },
  },
});
