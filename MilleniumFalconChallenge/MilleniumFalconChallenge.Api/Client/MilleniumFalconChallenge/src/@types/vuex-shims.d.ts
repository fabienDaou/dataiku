import { ComponentCustomProperties } from 'vue'
import { Store } from 'vuex'
import { type AppState } from '../types/milleniumFalconChallenge'

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $store: Store<AppState>
  }
}
