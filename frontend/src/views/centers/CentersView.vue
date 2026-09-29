<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import {
  getCenters,
  type Center,
} from '../../api/centers'

import { ApiError } from '../../api/http'

const search = ref('')
const status = ref('')
const region = ref('')

const centers = ref<Center[]>([])

const loading = ref(false)
const errorMessage = ref<string | null>(null)

const page = ref(1)
const pageSize = ref(10)
const totalCount = ref(0)
const totalPages = ref(0)

const availableRegions = computed(() => {
  return [...new Set(centers.value.map(center => center.region))]
      .sort()
})

async function loadCenters() {
  loading.value = true
  errorMessage.value = null

  try {
    const result = await getCenters({
      page: page.value,
      pageSize: pageSize.value,
      search: search.value.trim() || undefined,
      region: region.value || undefined,
      status: status.value || undefined,
    })

    centers.value = result.items
    totalCount.value = result.totalCount
    totalPages.value = result.totalPages
  } catch (error) {
    centers.value = []

    if (error instanceof ApiError) {
      if (error.status === 401) {
        errorMessage.value = 'Your session has expired.'
      } else if (error.status === 403) {
        errorMessage.value =
            'You do not have permission to view centers.'
      } else {
        errorMessage.value = error.message
      }
    } else {
      errorMessage.value =
          'Unable to connect to the Shaml API.'
    }
  } finally {
    loading.value = false
  }
}

async function applyFilters() {
  page.value = 1
  await loadCenters()
}

async function goToPage(targetPage: number) {
  if (
      targetPage < 1 ||
      targetPage > totalPages.value ||
      targetPage === page.value
  ) {
    return
  }

  page.value = targetPage
  await loadCenters()
}

async function previousPage() {
  await goToPage(page.value - 1)
}

async function nextPage() {
  await goToPage(page.value + 1)
}

const visiblePages = computed(() => {
  const pages: number[] = []

  for (let i = 1; i <= totalPages.value; i++) {
    pages.push(i)
  }

  return pages
})

const showingFrom = computed(() => {
  if (totalCount.value === 0) {
    return 0
  }

  return (page.value - 1) * pageSize.value + 1
})

const showingTo = computed(() => {
  return Math.min(
      page.value * pageSize.value,
      totalCount.value
  )
})

onMounted(loadCenters)
</script>

<template>
  <div class="page">
    <div class="breadcrumb">
      <span>Home</span>
      <span>/</span>
      <strong>Centers</strong>
    </div>

    <div class="page-heading">
      <div>
        <h1>Centers Management</h1>
        <p>
          Manage Shaml centers, their status and assigned directors.
        </p>
      </div>

      <button class="primary-button">
        <span class="button-plus">+</span>
        Create Center
      </button>
    </div>

    <section class="panel filters">
      <div class="field search-field">
        <label for="center-search">Search</label>

        <input
            id="center-search"
            v-model="search"
            type="text"
            placeholder="Center number, name or city"
            @keyup.enter="applyFilters"
        />
      </div>

      <div class="field">
        <label for="status">Status</label>

        <select
            id="status"
            v-model="status"
            @change="applyFilters"
        >
          <option value="">All statuses</option>
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
        </select>
      </div>

      <div class="field">
        <label for="region">Region</label>

        <select
            id="region"
            v-model="region"
            @change="applyFilters"
        >
          <option value="">All regions</option>

          <option
              v-for="item in availableRegions"
              :key="item"
              :value="item"
          >
            {{ item }}
          </option>
        </select>
      </div>
    </section>

    <section class="panel table-panel">
      <div class="table-header">
        <div>
          <h2>Shaml Centers</h2>

          <span>
            {{ totalCount }}
            {{ totalCount === 1 ? 'center' : 'centers' }}
          </span>
        </div>
      </div>

      <div
          v-if="loading"
          class="empty-state"
      >
        Loading centers...
      </div>

      <div
          v-else-if="errorMessage"
          class="empty-state"
      >
        {{ errorMessage }}
      </div>

      <div
          v-else
          class="table-container"
      >
        <table>
          <thead>
          <tr>
            <th>Center No.</th>
            <th>Center Name</th>
            <th>Region</th>
            <th>City</th>
            <th>Director</th>
            <th>Phone</th>
            <th>Status</th>
            <th class="actions-column">Actions</th>
          </tr>
          </thead>

          <tbody>
          <tr
              v-for="center in centers"
              :key="center.id"
          >
            <td class="number">
              {{ center.code }}
            </td>

            <td>
              <strong class="center-name">
                {{ center.name }}
              </strong>
            </td>

            <td>{{ center.region }}</td>

            <td>{{ center.city }}</td>

            <!--
              Director is not part of the current
              GET /api/centers response yet.
            -->
            <td>Not assigned</td>

            <td>
              {{ center.phone ?? '—' }}
            </td>

            <td>
              <span
                  class="status"
                  :class="center.status.toLowerCase()"
              >
                <span class="status-dot"></span>

                {{ center.status }}
              </span>
            </td>

            <td class="actions-column">
              <button
                  class="action-button"
                  aria-label="Center actions"
              >
                ⋮
              </button>
            </td>
          </tr>

          <tr v-if="centers.length === 0">
            <td
                colspan="8"
                class="empty-state"
            >
              No centers match the selected filters.
            </td>
          </tr>
          </tbody>
        </table>
      </div>

      <div
          v-if="!loading && !errorMessage"
          class="pagination"
      >
        <span v-if="totalCount > 0">
          Showing {{ showingFrom }}–{{ showingTo }}
          of {{ totalCount }} centers
        </span>

        <span v-else>
          Showing 0 centers
        </span>

        <div
            v-if="totalPages > 0"
            class="pagination-buttons"
        >
          <button
              :disabled="page <= 1"
              @click="previousPage"
          >
            ‹
          </button>

          <button
              v-for="pageNumber in visiblePages"
              :key="pageNumber"
              :class="{ selected: pageNumber === page }"
              @click="goToPage(pageNumber)"
          >
            {{ pageNumber }}
          </button>

          <button
              :disabled="page >= totalPages"
              @click="nextPage"
          >
            ›
          </button>
        </div>
      </div>
    </section>
  </div>
</template>