<template>
  <v-app id="inspire">
    <v-navigation-drawer v-model="drawer">
      <v-sheet color="grey-lighten-4" class="pa-4">
        <v-avatar class="mb-4" color="grey-darken-1" size="64">
          <v-icon icon="mdi-account-circle"></v-icon>
        </v-avatar>

        <div>{{ playerName }}</div>
      </v-sheet>
    </v-navigation-drawer>

    <v-main>
      <v-container class="py-8 px-6" fluid>
        <v-row>
          <v-col>
            <ImportScenarioCard />
          </v-col>
          <v-col v-for="scenario in scenarios" :key="scenario.name" cols="12">
            <ScenarioCard
              :name="scenario.name"
              :countdown="scenario.countdown"
              :probability="scenario.probability"
              :bountyHunters="scenario.bountyHunters"
            />
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const drawer = ref(null)
</script>

<script lang="ts">
import ScenarioCard from '../components/ScenarioCard.vue'
import ImportScenarioCard from '../components/ImportScenarioCard.vue'
import { type Scenario, type AppState } from '../types/milleniumFalconChallenge'

export default {
  name: 'ScenarioViews',
  components: { ScenarioCard, ImportScenarioCard },
  computed: {
    playerName(): string {
      return this.$store.state.playerName
    },
    scenarios(): Scenario[] {
      return (this.$store.state as AppState).scenarios.sort((s1, s2) => {
        if (s1.id == null) {
          return -1
        }
        if (s2.id == null) {
          return 1
        }
        if (s1.id < s2.id) {
          return 1
        } else if (s1.id > s2.id) {
          return -1
        }
        return 0
      })
    }
  },
  mounted() {
    this.$store.dispatch('getScenariosAsync')
  }
}
</script>
