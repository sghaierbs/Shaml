<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

import { login } from '../../api/auth'
import { ApiError } from '../../api/http'
import { setSession } from '../../auth/auth'

const router = useRouter()

const externalId = ref('iam-local-001')

const loading = ref(false)
const errorMessage = ref<string | null>(null)

async function submitLogin() {
  const value = externalId.value.trim()

  if (!value) {
    errorMessage.value = 'User ID is required.'
    return
  }

  loading.value = true
  errorMessage.value = null

  try {
    const result = await login(value)

    setSession(
        result.accessToken,
        result.expiresAtUtc
    )

    await router.push('/centers')
  } catch (error) {
    if (error instanceof ApiError) {
      if (error.status === 404) {
        errorMessage.value =
            'The user could not be found.'
      } else if (error.status === 400) {
        errorMessage.value =
            error.message
      } else {
        errorMessage.value =
            'Unable to sign in.'
      }
    } else {
      errorMessage.value =
          'Unable to connect to Shaml.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-page">
    <div class="login-panel">
      <div class="brand">
        <div class="brand-mark">
          شمل
        </div>

        <div>
          <h1>Shaml</h1>
          <span>Development Portal</span>
        </div>
      </div>

      <div class="login-content">
        <h2>Sign in</h2>

        <p class="description">
          Enter your development IAM user ID
          to access the Shaml portal.
        </p>

        <form @submit.prevent="submitLogin">
          <div class="field">
            <label for="external-id">
              User ID
            </label>

            <input
                id="external-id"
                v-model="externalId"
                type="text"
                autocomplete="username"
                placeholder="iam-local-001"
                :disabled="loading"
            />
          </div>

          <div
              v-if="errorMessage"
              class="error-message"
          >
            {{ errorMessage }}
          </div>

          <button
              type="submit"
              class="login-button"
              :disabled="loading"
          >
            {{
              loading
                  ? 'Signing in...'
                  : 'Sign in'
            }}
          </button>
        </form>

        <div class="development-notice">
          Development authentication only
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px;
  background: #f5f7f8;
}

.login-panel {
  width: 100%;
  max-width: 440px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  box-shadow:
      0 10px 30px rgba(0, 0, 0, 0.06);
  overflow: hidden;
}

.brand {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 24px 28px;
  border-bottom: 1px solid #edf0f2;
}

.brand-mark {
  width: 48px;
  height: 48px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #0d6b5c;
  color: white;
  font-size: 20px;
  font-weight: 700;
}

.brand h1 {
  margin: 0;
  font-size: 20px;
}

.brand span {
  display: block;
  margin-top: 2px;
  font-size: 13px;
  color: #6b7280;
}

.login-content {
  padding: 32px 28px;
}

.login-content h2 {
  margin: 0;
  font-size: 26px;
}

.description {
  margin: 8px 0 28px;
  color: #6b7280;
  line-height: 1.5;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.field label {
  font-size: 14px;
  font-weight: 600;
}

.field input {
  height: 44px;
  padding: 0 13px;
  border: 1px solid #d6dadd;
  border-radius: 8px;
  font: inherit;
  outline: none;
}

.field input:focus {
  border-color: #0d6b5c;
  box-shadow:
      0 0 0 3px rgba(13, 107, 92, 0.1);
}

.error-message {
  margin-top: 14px;
  padding: 11px 13px;
  border-radius: 7px;
  background: #fef2f2;
  color: #b42318;
  font-size: 14px;
}

.login-button {
  width: 100%;
  height: 44px;
  margin-top: 22px;
  border: 0;
  border-radius: 8px;
  background: #0d6b5c;
  color: white;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
}

.login-button:hover:not(:disabled) {
  background: #09594d;
}

.login-button:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.development-notice {
  margin-top: 24px;
  padding-top: 18px;
  border-top: 1px solid #edf0f2;
  text-align: center;
  color: #8a9199;
  font-size: 12px;
}
</style>