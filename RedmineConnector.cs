using System;
using System.Linq;
using Project_Connector_Library.Connectors;
using Project_Connector_Library.Models;
using Redmine.Net.Api;
using RedmineUser = Redmine.Net.Api.Types.User;
using RedmineProject = Redmine.Net.Api.Types.Project;
using RedmineIssue = Redmine.Net.Api.Types.Issue;
using RedmineProjectMembership = Redmine.Net.Api.Types.ProjectMembership;

namespace RedmineConnector
{
    public class RedminePlatformConnector : PlatformConnector
    {
        private const string FormatVersion = "1.0";
        private const string PlatformName = "redmine";
        private readonly RedmineManager _redmineManager;

        public RedminePlatformConnector(string hostname, string apiKey)
            : base(hostname, apiKey)
        {
            _redmineManager = new RedmineManager(Hostname, ApiKey, MimeFormat.xml, false);
        }

        public override ProjectExchangeData Import()
        {
            var redmineUsers = _redmineManager.GetTotalObjectList<RedmineUser>(null);
            var redmineProjects = _redmineManager.GetTotalObjectList<RedmineProject>(null);
            var redmineIssues = _redmineManager.GetTotalObjectList<RedmineIssue>(null);
            var redmineProjectMemberships =
                _redmineManager.GetTotalObjectList<RedmineProjectMembership>(null);

            var result = new ProjectExchangeData
            {
                ExportDate = DateTime.Now,
                ExportOrigin = new ExportOrigin {Name = PlatformName},
                Version = FormatVersion,
                Users = redmineUsers.Select(RedmineConverter.Instance.ToUser),
                Projects = redmineProjects.Select(p =>
                    RedmineConverter.Instance.ToProject(p, redmineProjectMemberships
                        .Where(m => m.Project.Id == p.Id))),
                Issues = redmineIssues.Select(RedmineConverter.Instance.ToIssue)
            };

            return result;
        }

        public override void Export(ProjectExchangeData data)
        {
            throw new NotImplementedException();
        }
    }
}