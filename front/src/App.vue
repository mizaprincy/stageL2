<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="App hidden">
    <RouterLink to="/"> </RouterLink>
  </div>
  <RouterView class="w-full h-screen" />
</template>
