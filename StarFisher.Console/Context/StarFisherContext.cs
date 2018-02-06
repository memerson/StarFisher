using StarFisher.Domain.QuarterlyAwards;
using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate;
using StarFisher.Domain.QuarterlyAwards.NominationListAggregate;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Context
{
    public interface IStarFisherContext
    {
        bool IsInitialized { get; }

        INominationListContext NominationListContext { get; }

        IAwardWinnerListContext AwardWinnerListContext { get; }
    }

    public class StarFisherContext : IStarFisherContext
    {
        private StarFisherContext() { }

        public static StarFisherContext Current { get; } = new StarFisherContext();

        public static void Initialize(INominationListRepository nominationListRepository,
            IRepository<AwardWinnerList> awardWinnerRepository, Year year, Quarter quarter)
        {
            if (!Context.NominationListContext.IsInitialized)
                Context.NominationListContext.Initialize(nominationListRepository, year, quarter);

            if (!Context.AwardWinnerListContext.IsInitialized)
                Context.AwardWinnerListContext.Initialize(awardWinnerRepository, year, quarter);
        }

        public bool IsInitialized => Context.NominationListContext.IsInitialized &&
                                     Context.AwardWinnerListContext.IsInitialized;

        public INominationListContext NominationListContext => Context.NominationListContext.Current;

        public IAwardWinnerListContext AwardWinnerListContext => Context.AwardWinnerListContext.Current;
    }
}
