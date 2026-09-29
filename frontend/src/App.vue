<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

import AppHeader from './components/layout/AppHeader.vue'

const route = useRoute()

const showHeader = computed(() => {
  return route.name !== 'login'
})
</script>

<template>
  <div class="app">
    <AppHeader v-if="showHeader" />

    <main class="app-content">
      <RouterView />
    </main>
  </div>
</template>