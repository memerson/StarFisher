using StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate.Perisistence;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Domain.QuarterlyAwards.AwardWinnerListAggregate
{
    public class AwardWinnerListRepository : RepositoryBase<AwardWinnerList>
    {
        public AwardWinnerListRepository(DirectoryPath workingDirectoryPath)
            : base(typeof(AwardWinnerListDto), workingDirectoryPath, @"Winners") { }

        protected override AwardWinnerList GetAggregateRootFromDto(object dto)
        {
            var awardWinnerListDto = dto as AwardWinnerListDto;
            return awardWinnerListDto?.ToAwardWinnerList();
        }

        protected override object GetDtoFromAggregateRoot(AwardWinnerList awardWinnerList)
        {
            return new AwardWinnerListDto(awardWinnerList);
        }

        protected override Quarter GetQuarter(AwardWinnerList awardWinnerList)
        {
            return awardWinnerList.Quarter;
        }

        protected override Year GetYear(AwardWinnerList awardWinnerList)
        {
            return awardWinnerList.Year;
        }
    }
}
