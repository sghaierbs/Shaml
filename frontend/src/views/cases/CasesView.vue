<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

import {
  claimAssignment,
  getMyAssignments,
  getRoleQueue,
  type MyAssignment,
  type QueueAssignment,
} from '../../api/assignments'

const myAssignments = ref<MyAssignment[]>([])
const queueAssignments = ref<QueueAssignment[]>([])

const loading = ref(false)
const claimingAssignmentId = ref<string | null>(null)
const errorMessage = ref<string | null>(null)

const activeTab = ref<'mine' | 'queue'>('mine')

const displayedAssignments = computed(() => {
  return activeTab.value === 'mine'
      ? myAssignments.value
      : queueAssignments.value
})

async function loadAssignments() {
  loading.value = true
  errorMessage.value = null

  try {
    const [mine, queue] = await Promise.all([
      getMyAssignments(),
      getRoleQueue(),
    ])

    myAssignments.value = mine
    queueAssignments.value = queue
  } catch (error) {
    console.error(
        'Failed to load assignments:',
        error
    )

    errorMessage.value =
        'Unable to load assignments.'
  } finally {
    loading.value = false
  }
}

async function claim(assignmentId: string) {
  claimingAssignmentId.value = assignmentId
  errorMessage.value = null

  try {
    await claimAssignment(assignmentId)

    /*
     * Reload both lists because the claimed
     * assignment should move from the role
     * queue into My Assignments.
     */
    await loadAssignments()

    /*
     * Show the user their newly claimed work.
     */
    activeTab.value = 'mine'
  } catch (error) {
    console.error(
        'Failed to claim assignment:',
        error
    )

    errorMessage.value =
        'Unable to claim the assignment.'
  } finally {
    claimingAssignmentId.value = null
  }
}

function taskName(taskCode: string): string {
  switch (taskCode) {
    case 'case.review':
      return 'Review Case'

    default:
      return taskCode
  }
}

function statusName(status: number): string {
  switch (status) {
    case 1:
      return 'Available'

    case 2:
      return 'Assigned'

    default:
      return `Status ${status}`
  }
}

function formatDate(value: string): string {
  return new Date(value).toLocaleString()
}

onMounted(loadAssignments)
</script>

<template>
  <div class="page">
    <div class="breadcrumb">
      <span>Home</span>
      <span>/</span>
      <strong>My Work</strong>
    </div>

    <div class="page-heading">
      <div>
        <h1>My Work</h1>

        <p>
          Review your assigned work or claim new tasks
          from your center queue.
        </p>
      </div>
    </div>

    <div
        v-if="errorMessage"
        class="assignment-error"
    >
      {{ errorMessage }}
    </div>

    <section class="work-summary">
      <button
          class="summary-card"
          :class="{ selected: activeTab === 'mine' }"
          type="button"
          @click="activeTab = 'mine'"
      >
        <span>My Assignments</span>
        <strong>{{ myAssignments.length }}</strong>
      </button>

      <button
          class="summary-card"
          :class="{ selected: activeTab === 'queue' }"
          type="button"
          @click="activeTab = 'queue'"
      >
        <span>Available Assignments</span>
        <strong>{{ queueAssignments.length }}</strong>
      </button>
    </section>

    <section class="panel table-panel">
      <div class="table-header">
        <div>
          <h2>
            {{
              activeTab === 'mine'
                  ? 'My Assignments'
                  : 'Available Assignments'
            }}
          </h2>

          <span>
            {{ displayedAssignments.length }} assignments
          </span>
        </div>

        <button
            class="refresh-button"
            type="button"
            :disabled="loading"
            @click="loadAssignments"
        >
          Refresh
        </button>
      </div>

      <div
          v-if="loading"
          class="loading-state"
      >
        Loading assignments...
      </div>

      <div
          v-else
          class="table-container"
      >
        <table>
          <thead>
          <tr>
            <th>Case</th>
            <th>Task</th>
            <th>Status</th>
            <th>Created</th>
            <th class="actions-column">
              Action
            </th>
          </tr>
          </thead>

          <tbody>
          <tr
              v-for="assignment in displayedAssignments"
              :key="assignment.assignmentId"
          >
            <td>
                <span class="case-id">
                  {{ assignment.caseId }}
                </span>
            </td>

            <td>
              <strong>
                {{ taskName(assignment.taskCode) }}
              </strong>
            </td>

            <td>
                <span
                    class="assignment-status"
                    :class="{
                    available: assignment.status === 1,
                    assigned: assignment.status === 2,
                  }"
                >
                  {{ statusName(assignment.status) }}
                </span>
            </td>

            <td>
              {{ formatDate(assignment.createdAtUtc) }}
            </td>

            <td class="actions-column">
              <button
                  v-if="activeTab === 'queue'"
                  class="claim-button"
                  type="button"
                  :disabled="
                    claimingAssignmentId ===
                    assignment.assignmentId
                  "
                  @click="claim(assignment.assignmentId)"
              >
                {{
                  claimingAssignmentId ===
                  assignment.assignmentId
                      ? 'Claiming...'
                      : 'Claim'
                }}
              </button>

              <button
                  v-else
                  class="open-button"
                  type="button"
              >
                Open
              </button>
            </td>
          </tr>

          <tr v-if="displayedAssignments.length === 0">
            <td
                colspan="5"
                class="empty-state"
            >
              {{
                activeTab === 'mine'
                    ? 'You have no assigned work.'
                    : 'There are no available assignments.'
              }}
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </section>
  </div>
</template>

<style scoped>
.work-summary {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
}

.summary-card {
  min-width: 210px;
  padding: 18px 20px;

  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 8px;

  border: 1px solid #e1e5e3;
  border-radius: 10px;

  background: #ffffff;
  color: #555e58;

  text-align: left;
}

.summary-card strong {
  color: #20252a;
  font-size: 26px;
}

.summary-card.selected {
  border-color: #087f5b;
  background: #f1faf6;
}

.summary-card.selected span,
.summary-card.selected strong {
  color: #087f5b;
}

.refresh-button,
.open-button,
.claim-button {
  height: 36px;
  padding: 0 14px;

  border-radius: 6px;
  font-weight: 500;
}

.refresh-button,
.open-button {
  border: 1px solid #d6dadd;
  background: #ffffff;
  color: #333333;
}

.claim-button {
  border: 1px solid #087f5b;
  background: #087f5b;
  color: #ffffff;
}

.claim-button:hover:not(:disabled) {
  background: #066c4d;
}

.refresh-button:disabled,
.claim-button:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.assignment-status {
  display: inline-flex;
  align-items: center;

  padding: 5px 9px;
  border-radius: 999px;

  font-size: 12px;
  font-weight: 600;
}

.assignment-status.available {
  background: #fff8e5;
  color: #946200;
}

.assignment-status.assigned {
  background: #e8f5ef;
  color: #087f5b;
}

.case-id {
  font-family: monospace;
  font-size: 12px;
}

.loading-state {
  padding: 40px;
  color: #6b7280;
  text-align: center;
}

.assignment-error {
  margin-bottom: 20px;
  padding: 12px 16px;

  border: 1px solid #f0b8b8;
  border-radius: 7px;

  background: #fff4f4;
  color: #b42318;
}
</style>