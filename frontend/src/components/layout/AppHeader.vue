<script setup lang="ts">
import { RouterLink } from 'vue-router'
import RoleSwitcher from '../RoleSwitcher.vue'
import {
  canAccessElsa,
  getAccessToken
} from '../../auth/auth'

const ELSA_STUDIO_URL = 'http://localhost:5209'

function openElsaStudio() {
  const token = getAccessToken()

  if (!token || !canAccessElsa()) {
    return
  }

  const url =
      `${ELSA_STUDIO_URL}/shaml-login` +
      `?token=${encodeURIComponent(token)}`

  //window.location.href = url
  window.open(url, '_blank', 'noopener,noreferrer');
}
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

        <a
            v-if="canAccessElsa()"
            href="#"
            @click.prevent="openElsaStudio"
        >
          Elsa Studio
        </a>
      </nav>

      <div class="header-user-context">
        <RoleSwitcher />
      </div>
    </div>
  </header>
</template>