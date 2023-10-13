<template>
  <v-card class="mx-auto" max-width="800" variant="outlined">
    <v-card-item>
      <div>
        <div class="text-h3 mb-1">Import another scenario, {{ playerName }}</div>
      </div>
    </v-card-item>

    <v-card-text>
      <v-text-field label="Scenario name" variant="outlined" v-model="scenarioName"></v-text-field>
      <v-file-input
        v-model="files"
        density="compact"
        variant="outlined"
        prepend-icon=""
        label="Import a scenario"
        accept="application/json"
      ></v-file-input>
      <v-card-subtitle v-if="invalidFile">Invalid json structure</v-card-subtitle>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { safeJsonParse } from '../utils/parser'

type Scenario = {
  countdown: number
  bounty_hunters: BountyHunter[]
}

type BountyHunter = {
  planet: string
  day: number
}

export default {
  name: 'ImportScenario',
  data() {
    return {
      files: [],
      invalidFile: false,
      scenarioName: ''
    }
  },
  computed: {
    playerName(): string {
      return this.$store.state.playerName
    }
  },
  methods: {
    submit(files: Blob[]) {
      if (files.length == 0) {
        return
      }

      let that = this
      const reader = new FileReader()
      reader.onload = function (event: ProgressEvent<FileReader>) {
        if (event.target == null || event.target.result == null) {
          that.files = []
          that.scenarioName = ''
          return
        }
        const scenarioAsString = event.target.result as string

        function isScenarioType(o: any): o is Scenario {
          return 'countdown' in o && 'bounty_hunters' in o
        }
        const parseResult = safeJsonParse(isScenarioType)(scenarioAsString)
        that.invalidFile = !parseResult.parsed
        if (parseResult.parsed) {
          const scenario = JSON.parse(scenarioAsString) as Scenario
          that.$store.dispatch('createScenarioAsync', {
            id: null,
            name: that.scenarioName,
            countdown: scenario.countdown,
            probability: null,
            bountyHunters: scenario.bounty_hunters.map((bh) => {
              return { ...bh }
            })
          })
        }

        that.files = []
        that.scenarioName = ''
      }
      reader.readAsText(files[0])
    }
  },
  watch: {
    files(newFiles) {
      this.submit(newFiles)
    }
  }
}
</script>
