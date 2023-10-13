import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

import { createStore } from 'vuex'

import { type AppState } from './types/milleniumFalconChallenge'

const vuetify = createVuetify({
  icons: {
    defaultSet: 'mdi'
  },
  components,
  directives
})

const store = createStore<AppState>({
  state() {
    return {
      playerName: '',
      scenarios: [],
      totalScenarios: 0
    }
  },
  mutations: {
    changePlayerName(state: AppState, newPlayerName: string) {
      state.playerName = newPlayerName
    },
    updateScenarios(state: AppState, scenarios: []) {
      state.scenarios = scenarios
    },
    addScenario(state: AppState, newScenario: any) {
      state.scenarios.unshift(newScenario as never)
    },
    updateTotalScenarios(state: AppState, total: number) {
      state.totalScenarios = total
    }
  },
  actions: {
    async getScenariosAsync({ commit }: { commit: Function }) {
      const url = '/api/scenarios?page=1&pageSize=100000'
      const response = await fetch(url, { method: 'GET' })
      if (response.ok) {
        const body: { total: number; scenarios: [] } = await response.json()
        commit('updateTotalScenarios', body.total)
        commit('updateScenarios', body.scenarios)
      }
    },
    async createScenarioAsync({ commit }: { commit: Function }, scenario: any) {
      const url = '/api/scenarios'
      const response = await fetch(url, {
        method: 'POST',
        body: JSON.stringify(scenario),
        headers: {
          'Content-Type': 'application/json'
        }
      })
      if (response.ok) {
        const body: { id: number } = await response.json()
        scenario.id = body.id
        commit('addScenario', scenario)
      }
    }
  }
})

const app = createApp(App)
app.use(vuetify)
app.use(router)
app.use(store)

app.mount('#app')
