<template>
    <div>
      <Bar :data="props.chartData" :options="props.chartOptions" />
    </div>
  </template>
  
  <script setup lang="ts">
  import { defineProps } from 'vue'
  import { Bar } from 'vue-chartjs'
  import {
    Chart as ChartJS,
    Title,
    Tooltip,
    Legend,
    BarElement,
    CategoryScale,
    LinearScale
  } from 'chart.js'
  
  // Enregistrement des plugins nécessaires pour un graphique de type Bar
  ChartJS.register(
    Title,
    Tooltip,
    Legend,
    BarElement,
    CategoryScale,
    LinearScale
  )
  
  // Props passées au composant
  const props = defineProps({
    chartData: {
      type: Object,
      required: true
    },
    chartOptions: {
      type: Object,
      required: true
    }
  })
  </script>
  