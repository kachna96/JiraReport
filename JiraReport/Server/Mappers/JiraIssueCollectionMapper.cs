using AutoMapper;

namespace JiraReport.Server.Mappers
{
    public class JiraIssueCollectionMapper : Profile
    {
        public JiraIssueCollectionMapper()
        {
            CreateMap<IEnumerable<Shared.JiraIssue>, Shared.JiraIssueCollection>()
                .ForMember(dest => dest.Issues, o => o.MapFrom(x => x))
                .ForMember(dest => dest.TotalTimeSpendInSeconds, o => o.MapFrom(x => x.Sum(s => s.TimeSpendInSeconds)))
                .AfterMap((src, dest) => dest.TotalTime = TimeSpan.FromSeconds(dest.TotalTimeSpendInSeconds));
        }
    }
}
