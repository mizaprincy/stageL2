<script setup lang="ts">
import { Vue3Lottie } from 'vue3-lottie'
import Login from '@/assets/animations/login.json.json'
import formProfile from '@/components/formProfile.vue'
import StatSalaire from '@/components/statSalaire.vue';

</script>

<template>
  <div class="w-full h-full p-2">
    <h1 class="font-sans text-slate-600 text-4xl text-center">Details personnelles</h1>
    <h2 class="font-sans text-slate-400 text-center text-sm m-2">
      Nous n'allons jamais vendre vos informations personnelles
    </h2>
    <section class="contenu w-full flex justify-center">
      <div class="container w-full rounded-lg flex flex-row justify-between bg-slate-200 my-4">
        <div class="animation w-[45%]">
          <Vue3Lottie style="width: 16rem; height: 24rem" loop :animationData="Login" />
        </div>
        <div class="separate h-full flex flex-col justify-center">
          <span class="w-1 h-[85%] bg-slate-400"></span>
        </div>
        <div class="profile w-full h-96 py-6">
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
    <div class="stat py-4">
      <StatSalaire/>
    </div>
  </div>
</template>
