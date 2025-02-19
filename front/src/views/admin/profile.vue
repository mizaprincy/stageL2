<script setup lang="ts">
import { Vue3Lottie } from 'vue3-lottie'
import Login from '@/assets/animations/login.json.json'
import formProfile from '@/components/formProfile.vue'



</script>

<template>
  <div class="w-full h-full p-6">
    <h1 class="font-sans text-slate-100 text-4xl text-center">Details personnelles</h1>
    <h2 class="font-sans text-slate-300 text-center m-4">
      Nous n'allons jamais vendre vos informations personnelles
    </h2>
    <section class="contenu w-full flex justify-center p-10">
      <div class="container w-[40rem] rounded-lg flex flex-row justify-between bg-slate-200">
        <div class="animation">
          <Vue3Lottie style="width: 18rem; height: 24rem" loop :animationData="Login" />
        </div>
        <div class="separate h-full flex flex-col justify-center">
          <span class="w-1 h-[85%] bg-slate-400"></span>
        </div>
        <div class="profile w-96 h-96 p-2 py-6">
          <div class="profileContainer w-full flex flex-col items-center text-slate-500">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="8rem"
              height="8rem"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="lucide lucide-circle-user-round"
            >
              <path d="M18 20a6 6 0 0 0-12 0" />
              <circle cx="12" cy="10" r="4" />
              <circle cx="12" cy="12" r="10" />
            </svg>
            <h1>Profile</h1>
            <formProfile />
            
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
