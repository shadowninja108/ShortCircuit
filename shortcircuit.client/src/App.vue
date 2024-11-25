<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div id="app" class="content">
    <router-view/>
  </div>
</template>

<style scoped>
  .content {
    display: flex;
    flex-grow: 1;
    min-height: 100vh;
  }
  
@media (min-width: 1024px) {
  .content {
    display: flex;
  }

  .logo {
    margin: 0 2rem 0 0;
  }
}
</style>
