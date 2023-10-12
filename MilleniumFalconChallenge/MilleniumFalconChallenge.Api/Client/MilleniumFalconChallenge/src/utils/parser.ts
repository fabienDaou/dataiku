export type ParseResult<T> =
  | { parsed: T; hasError: false; error?: undefined }
  | { parsed?: undefined; hasError: true; error?: unknown }

export const safeJsonParse =
  <T>(typeguard: (o: any) => o is T) =>
  (text: string): ParseResult<T> => {
    try {
      const parsed = JSON.parse(text)
      return typeguard(parsed) ? { parsed, hasError: false } : { hasError: true }
    } catch (error) {
      return { hasError: true, error }
    }
  }
