<template>
  <v-card class="mx-auto" max-width="800" variant="outlined">
    <v-card-text>
      <div v-if="!isProcessing" class="text-h3 mb-1">
        {{ probability }}
        <v-icon class="text-h3 mb-1" icon="mdi-percent"></v-icon>
      </div>
      <div v-else class="text-h3 mb-1">Processing...</div>
      <span class="text-h5 mb-1">{{ name }}</span>
    </v-card-text>

    <v-card-actions>
      <v-expansion-panels>
        <v-expansion-panel>
          <v-expansion-panel-title>
            <v-row no-gutters>
              <v-col cols="4" class="d-flex justify-start"> Countdown {{ countdown }} </v-col>
              <v-col cols="8" class="text--secondary">
                <v-fade-transition leave-absolute>
                  {{ bountyHunters.length }} bounty hunters
                </v-fade-transition>
              </v-col>
            </v-row>
          </v-expansion-panel-title>

          <v-expansion-panel-text>
            <v-list :items="bountyHuntersList"></v-list>
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import type { BountyHunter } from '@/types/milleniumFalconChallenge'
import type { PropType } from 'vue'
export default {
  name: 'ScenarioCard',
  props: {
    name: { default: '' },
    countdown: { default: 0 },
    probability: { type: Object as PropType<number | null>, default: null },
    bountyHunters: { type: Object as PropType<BountyHunter[]>, default: [] }
  },
  computed: {
    bountyHuntersList() {
      return this.bountyHunters.map((bh) => bh.planet + ' ' + bh.day)
    },
    isProcessing() {
      return this.probability == null
    }
  }
}
</script>
