<template>
  <v-card class="mx-auto" max-width="800" variant="outlined">
    <v-card-item>
      <div>
        <div class="text-h3 mb-1">Ready to help rebels?</div>
      </div>
    </v-card-item>

    <v-card-actions>
      <v-text-field
        label="Player name"
        variant="outlined"
        append-inner-icon="mdi-send"
        v-model="playerName"
        @click:append-inner="goNextView"
        @keydown.enter.prevent="goNextView"
      ></v-text-field>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
export default {
  name: 'WelcomeView',
  data() {
    return {
      playerName: ''
    }
  },
  methods: {
    goNextView() {
      this.$store.commit('changePlayerName', this.playerName)
      this.$router.push('/scenarios')
    }
  }
}
</script>
