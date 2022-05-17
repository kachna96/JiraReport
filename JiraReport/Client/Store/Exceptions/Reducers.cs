using Fluxor;

namespace JiraReport.Client.Store.Exceptions
{
    public static class Reducers
    {
        [ReducerMethod]
        public static ExceptionsState ReduceAddExceptionAction(ExceptionsState state, AddExceptionAction action) =>
            state with
            {
                Exceptions = state.Exceptions.Concat(new[] { action.Exception }).ToList()
            };

        [ReducerMethod]
        public static ExceptionsState ReduceResetExceptionsAction(ExceptionsState state, ResetExceptionsAction _) =>
            state with
            {
                Exceptions = new()
            };
    }
}
