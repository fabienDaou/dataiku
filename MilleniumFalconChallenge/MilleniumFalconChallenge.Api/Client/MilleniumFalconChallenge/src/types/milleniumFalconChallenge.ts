export type AppState = {
  playerName: string
  totalScenarios: number
  scenarios: Scenario[]
}
export type Scenario = {
  id: number | null
  name: string
  countdown: number
  probability: number | null
  bountyHunters: BountyHunter[]
}
export type BountyHunter = {
  planet: string
  day: number
}
