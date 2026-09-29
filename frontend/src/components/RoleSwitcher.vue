<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

import {
  getLandingRoute,
} from '../auth/navigation'

import {
  getCurrentUser,
  getMyRoles,
  switchRole,
  type UserRole,
} from '../api/roles'

import {
  clearSession,
  setSession,
} from '../auth/auth'

const router = useRouter()

const roles = ref<UserRole[]>([])
const activeUserRoleId = ref<string | null>(null)

const loading = ref(false)
const switching = ref(false)
const errorMessage = ref<string | null>(null)

const selectedUserRoleId = ref('')

const activeRole = computed(() =>
    roles.value.find(
        role =>
            role.userRoleId === activeUserRoleId.value
    )
)

function roleLabel(role: UserRole): string {
  if (role.centerName) {
    return `${role.roleName} — ${role.centerName}`
  }

  return role.roleName
}

async function loadRoles() {
  loading.value = true
  errorMessage.value = null

  try {
    const [availableRoles, currentUser] =
        await Promise.all([
          getMyRoles(),
          getCurrentUser(),
        ])

    console.log('Current user:', currentUser)
    console.log('Available roles:', availableRoles)
    console.log(
        'Active UserRole ID:',
        currentUser.activeUserRoleId
    )

    roles.value = availableRoles

    activeUserRoleId.value =
        currentUser.activeUserRoleId

    selectedUserRoleId.value =
        currentUser.activeUserRoleId
  } catch (error) {
    console.error(
        'Failed to load user context:',
        error
    )

    errorMessage.value =
        'Unable to load user roles.'
  } finally {
    loading.value = false
  }
}

async function changeRole() {
  if (
      !selectedUserRoleId.value ||
      selectedUserRoleId.value === activeUserRoleId.value
  ) {
    return
  }

  switching.value = true
  errorMessage.value = null

  /*
   * Remember which role the user selected.
   *
   * This is a UserRole assignment, not merely a Role.
   * This distinction is important because a user can
   * have the same role in multiple centers.
   */
  const selectedRole = roles.value.find(
      role =>
          role.userRoleId === selectedUserRoleId.value
  )

  if (!selectedRole) {
    errorMessage.value =
        'The selected role could not be found.'

    switching.value = false
    return
  }

  try {
    /*
     * Ask the backend to validate the selected
     * UserRole assignment and issue a new JWT.
     */
    const result = await switchRole(
        selectedRole.userRoleId
    )

    /*
     * Replace the old JWT with the new JWT.
     */
    setSession(
        result.accessToken,
        result.expiresAtUtc
    )

    /*
     * Fetch the new authenticated context.
     *
     * /api/me now uses the new JWT, so portal,
     * scope and center correspond to the
     * newly selected role.
     */
    const currentUser =
        await getCurrentUser()

    /*
     * Decide which portal/screen should become
     * the landing page.
     *
     * Examples:
     *
     * Public User
     *   -> /public
     *
     * Organization Administrator
     *   -> /centers
     *
     * Specialist
     *   -> /cases
     */
    const landingRoute =
        getLandingRoute(
            currentUser,
            selectedRole
        )

    /*
     * We deliberately perform a full reload
     * for now.
     *
     * The active security context changed,
     * therefore all components should be
     * reconstructed using the new JWT.
     */
    window.location.href = landingRoute
  } catch (error) {
    console.error(
        'Failed to switch role:',
        error
    )

    /*
     * Restore the dropdown to the currently
     * active role when switching fails.
     */
    selectedUserRoleId.value =
        activeUserRoleId.value ?? ''

    errorMessage.value =
        'Unable to switch role.'
  } finally {
    switching.value = false
  }
}

async function logout() {
  clearSession()

  await router.push('/login')
}

onMounted(loadRoles)
</script>

<template>
  <div class="role-switcher">
    <div
        v-if="activeRole"
        class="current-role"
    >
      <span class="role-label">
        Current role
      </span>

      <strong>
        {{ activeRole.roleName }}
      </strong>

      <span
          v-if="activeRole.centerName"
          class="center-name"
      >
        {{ activeRole.centerName }}
      </span>
    </div>

    <select
        v-model="selectedUserRoleId"
        class="role-select"
        :disabled="loading || switching"
        @change="changeRole"
    >
      <option
          v-if="loading"
          value=""
      >
        Loading roles...
      </option>

      <option
          v-for="role in roles"
          :key="role.userRoleId"
          :value="role.userRoleId"
      >
        {{ roleLabel(role) }}
      </option>
    </select>

    <button
        class="logout-button"
        type="button"
        :disabled="switching"
        @click="logout"
    >
      Logout
    </button>

    <span
        v-if="switching"
        class="switching-message"
    >
      Switching...
    </span>

    <span
        v-if="errorMessage"
        class="role-error"
    >
      {{ errorMessage }}
    </span>
  </div>
</template>

<style scoped>
.role-switcher {
  display: flex;
  align-items: center;
  gap: 12px;
}

.current-role {
  display: flex;
  flex-direction: column;
  min-width: 150px;
}

.role-label {
  font-size: 11px;
  color: #8a9199;
}

.current-role strong {
  font-size: 13px;
  color: #20252a;
}

.center-name {
  margin-top: 1px;
  font-size: 11px;
  color: #6b7280;
}

.role-select {
  min-width: 250px;
  height: 38px;
  padding: 0 34px 0 11px;

  border: 1px solid #d6dadd;
  border-radius: 7px;

  background: white;
  color: #20252a;

  font: inherit;
  font-size: 13px;
}

.role-select:focus {
  outline: none;
  border-color: #0d6b5c;

  box-shadow:
      0 0 0 3px rgba(13, 107, 92, 0.1);
}

.role-select:disabled {
  cursor: not-allowed;
  opacity: 0.65;
}

.logout-button {
  height: 38px;
  padding: 0 14px;

  border: 1px solid #d6dadd;
  border-radius: 7px;

  background: white;
  color: #333;

  cursor: pointer;
}

.logout-button:hover:not(:disabled) {
  background: #f6f7f8;
}

.logout-button:disabled {
  cursor: not-allowed;
  opacity: 0.65;
}

.switching-message {
  font-size: 12px;
  color: #6b7280;
}

.role-error {
  font-size: 12px;
  color: #b42318;
}
</style>