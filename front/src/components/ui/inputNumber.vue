<script setup lang="ts">
import { defineProps, defineEmits, ref, watch } from 'vue';

// Définir les props et événements
const props = defineProps<{ modelValue: number }>();
const emit = defineEmits<{
  (event: 'update:modelValue', value: number): void;
}>();

// Définir le formatteur selon la localisation et devise
const formatter = new Intl.NumberFormat('fr-FR', {
  style: 'decimal', // Pour éviter d'ajouter "€" automatiquement
  //minimumFractionDigits: 2,
  maximumFractionDigits: 2,
});

// État local pour afficher la valeur formatée
const displayValue = ref(formatter.format(props.modelValue));

// Observer `modelValue` et mettre à jour l'affichage
watch(() => props.modelValue, (newValue) => {
  displayValue.value = formatter.format(newValue);
  console.log(displayValue.value)
});

// Gérer la saisie utilisateur
const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement;
  let rawValue = target.value.replace(/\s/g, ''); // Supprime les espaces

  // Convertir en format numérique
  rawValue = rawValue.replace(',', '.'); // Remplace la virgule par un point pour parseFloat()
  const numericValue = parseFloat(rawValue);

  // Mise à jour de la valeur stockée
  emit('update:modelValue', isNaN(numericValue) ? 0 : numericValue);
};

// Reformater lors du `blur`
const handleBlur = () => {
  displayValue.value = formatter.format(props.modelValue);
};
</script>

<template>
  <input
    type="text"
    v-model="displayValue"
    @input="handleInput"
    @blur="handleBlur"
    class="border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500 text-right"
  />
</template>
