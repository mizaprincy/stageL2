<template>
  <div>
    <Line :data="props.chartData" :options="props.chartOptions" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  Filler
} from 'chart.js'

// Enregistrement des plugins nécessaires
ChartJS.register(
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
  Filler
)

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
