<script setup lang="ts">
import { RouterLink } from 'vue-router'
import RoleSwitcher from '../RoleSwitcher.vue'
</script>

<template>
  <header class="header">
    <div class="header-inner">
      <RouterLink to="/" class="brand">
        <div class="brand-mark">ش</div>

        <div class="brand-text">
          <strong>SHAML</strong>
          <span>Family Services</span>
        </div>
      </RouterLink>

      <nav class="navigation">
        <RouterLink to="/centers">
          Centers
        </RouterLink>

        <a href="#">
          Users
        </a>

        <a href="#">
          Reports
        </a>
      </nav>

      <div class="header-user-context">
        <RoleSwitcher />
      </div>
    </div>
  </header>
</template>