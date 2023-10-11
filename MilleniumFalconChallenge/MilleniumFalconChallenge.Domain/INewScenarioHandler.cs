namespace MilleniumFalconChallenge
{
    public interface INewScenarioHandler
    {
        Task<Result> HandleAsync(NewScenario scenario);

        public abstract record Result;

        public record InvalidScenario(string Message) : Result;
        public record UnexpectedError : Result;
        public record Success(int Id) : Result;
    }
}
