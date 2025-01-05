<script setup lang="ts">
import { Vue3Lottie } from 'vue3-lottie'
import Setting from '@/assets/animations/setting.json.json'
import FormService from '@/components/formService.vue'
</script>

<template>
  <div class="w-full h-full p-2">
    <h1 class="font-sans text-slate-600 text-4xl text-center">Demander une avance</h1>
    <h2 class="font-sans text-slate-400 text-center text-sm m-2">
      Vous pouvez demander votre avance seulement si il n'y en a pas une en cours
    </h2>
    <section class="contenu w-full flex justify-center">
      <div class="container w-full rounded-lg flex flex-row justify-between bg-slate-200 my-4">
        <div class="animation w-[50%]">
          <Vue3Lottie style="width: 18rem; height: 24rem" loop :animationData="Setting" />
        </div>
        <div class="separate h-full flex flex-col justify-center">
          <span class="w-1 h-[85%] bg-slate-400"></span>
        </div>
        <div class="profile w-full h-96 p-4">
          <div class="demandeAvance w-full flex flex-col items-center rounded bg-white text-slate-500 my-4">
            <h2 class="font-sans text-slate-700 text-center text-sm my-4">
              Veuillez remplir tout les champs
            </h2>
            <FormService />
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
