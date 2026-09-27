<script setup lang="ts">
import { computed, ref } from 'vue'

type CenterStatus = 'Active' | 'Inactive'

interface Center {
  id: string
  centerNumber: string
  name: string
  region: string
  city: string
  director: string
  phone: string
  status: CenterStatus
}

const search = ref('')
const status = ref('')

const centers = ref<Center[]>([
  {
    id: '1',
    centerNumber: 'CTR-001',
    name: 'Riyadh Shaml Center',
    region: 'Riyadh',
    city: 'Riyadh',
    director: 'Ahmed Mohammed',
    phone: '011 000 0001',
    status: 'Active',
  },
  {
    id: '2',
    centerNumber: 'CTR-002',
    name: 'Jeddah Shaml Center',
    region: 'Makkah',
    city: 'Jeddah',
    director: 'Khalid Abdullah',
    phone: '012 000 0002',
    status: 'Active',
  },
  {
    id: '3',
    centerNumber: 'CTR-003',
    name: 'Dammam Shaml Center',
    region: 'Eastern Province',
    city: 'Dammam',
    director: 'Not assigned',
    phone: '013 000 0003',
    status: 'Inactive',
  },
])

const filteredCenters = computed(() => {
  const value = search.value.trim().toLowerCase()

  return centers.value.filter(center => {
    const matchesSearch =
        !value ||
        center.name.toLowerCase().includes(value) ||
        center.centerNumber.toLowerCase().includes(value) ||
        center.city.toLowerCase().includes(value)

    const matchesStatus =
        !status.value ||
        center.status === status.value

    return matchesSearch && matchesStatus
  })
})
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
        />
      </div>

      <div class="field">
        <label for="status">Status</label>

        <select id="status" v-model="status">
          <option value="">All statuses</option>
          <option value="Active">Active</option>
          <option value="Inactive">Inactive</option>
        </select>
      </div>

      <div class="field">
        <label for="region">Region</label>

        <select id="region">
          <option>All regions</option>
          <option>Riyadh</option>
          <option>Makkah</option>
          <option>Eastern Province</option>
        </select>
      </div>
    </section>

    <section class="panel table-panel">
      <div class="table-header">
        <div>
          <h2>Shaml Centers</h2>
          <span>{{ filteredCenters.length }} centers</span>
        </div>
      </div>

      <div class="table-container">
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
              v-for="center in filteredCenters"
              :key="center.id"
          >
            <td class="number">
              {{ center.centerNumber }}
            </td>

            <td>
              <strong class="center-name">
                {{ center.name }}
              </strong>
            </td>

            <td>{{ center.region }}</td>
            <td>{{ center.city }}</td>
            <td>{{ center.director }}</td>
            <td>{{ center.phone }}</td>

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

          <tr v-if="filteredCenters.length === 0">
            <td colspan="8" class="empty-state">
              No centers match the selected filters.
            </td>
          </tr>
          </tbody>
        </table>
      </div>

      <div class="pagination">
        <span>
          Showing {{ filteredCenters.length }} centers
        </span>

        <div class="pagination-buttons">
          <button disabled>‹</button>
          <button class="selected">1</button>
          <button>2</button>
          <button>3</button>
          <button>›</button>
        </div>
      </div>
    </section>
  </div>
</template>