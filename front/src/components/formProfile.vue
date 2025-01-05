<template>
  <div>
    <div class="profileContent w-full flex flex-col p-2">
      <label for="nom" class="w-full my-2"
        >Nom : <span class="userFirstname m-1">{{ user!.nom }}</span>
        <span class="username">{{ user!.prenom }}</span></label
      >
      <label for="email" class="w-full my-2"
        >Email : <span class="usermail m-1">{{ user!.email }}</span></label
      >
      <label v-if="user!.employeId" for="email" class="w-full my-2"
        >Employe ID : <span class="usermail m-1">{{ user!.employeId }}</span></label
      >
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useGlobalStore } from '@/stores/globalStore'

export default defineComponent({
  name: 'UserProfile',
  setup() {
    const globalStore = useGlobalStore()
    const { user } = globalStore
    console.log(user)
    return {
      user
    }
  }
})
</script>
