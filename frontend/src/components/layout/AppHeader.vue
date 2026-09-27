<script setup lang="ts">
import { RouterLink } from 'vue-router'
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

      <div class="user-context">
        <div class="user-avatar">
          SA
        </div>

        <div class="user-details">
          <strong>Shaml Administrator</strong>
          <span>Organization</span>
        </div>

        <button class="context-button" type="button">
          ▾
        </button>
      </div>
    </div>
  </header>
</template>