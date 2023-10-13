<template>
  <v-app id="inspire">
    <v-main>
      <v-container class="py-8 px-6" fluid>
        <v-row>
          <v-col>
            <ImportScenarioCard />
          </v-col>
          <v-col v-for="scenario in scenarios" :key="scenario.name" cols="12">
            <ScenarioCard :name="scenario.name" :countdown="scenario.countdown" :probability="scenario.probability"
              :bountyHunters="scenario.bountyHunters" />
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import ScenarioCard from '../components/ScenarioCard.vue'
import ImportScenarioCard from '../components/ImportScenarioCard.vue'
import { type Scenario, type AppState } from '../types/milleniumFalconChallenge'

export default {
  name: 'ScenarioViews',
  components: { ScenarioCard, ImportScenarioCard },
  computed: {
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
    let that = this;
    window.setInterval(() => {
      that.$store.dispatch('getScenariosAsync')
    }, 2000);
  }
}
</script>
