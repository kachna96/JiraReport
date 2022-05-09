using Atlassian.Jira;
using AutoMapper;

namespace JiraReport.Server.Mappers
{
    public class JiraIssueMapper : Profile
    {
        public JiraIssueMapper()
        {
            CreateMap<Issue, Shared.JiraIssue>()
                .ForMember(dest => dest.IssueType, o => o.MapFrom(x => x.Type.Name));
        }
    }
}
