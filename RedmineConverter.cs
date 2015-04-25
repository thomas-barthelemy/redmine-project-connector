using System.Collections.Generic;
using System.Linq;
using Project_Connector_Library.Connectors;
using Redmine.Net.Api.Types;
using Issue = Project_Connector_Library.Models.Issue;
using Project = Project_Connector_Library.Models.Project;
using ProjectMembership = Project_Connector_Library.Models.ProjectMembership;
using RedmineUser = Redmine.Net.Api.Types.User;
using RedmineProject = Redmine.Net.Api.Types.Project;
using RedmineIssue = Redmine.Net.Api.Types.Issue;
using RedmineProjectMembership = Redmine.Net.Api.Types.ProjectMembership;
using User = Project_Connector_Library.Models.User;

namespace RedmineConnector
{
    internal class RedmineConverter : GenericPlatformConverterBase
        <RedmineUser, RedmineProject, RedmineIssue, RedmineProjectMembership>
    {
        private static RedmineConverter _instance;

        protected RedmineConverter()
        {
        }

        public static RedmineConverter Instance
        {
            get { return _instance ?? (_instance = new RedmineConverter()); }
        }

        public override RedmineUser ToPlatformUser(User user)
        {
            return new RedmineUser
            {
                Login = user.Username,
                Email = user.Email,
                Id = user.Id
            };
        }

        #region Convert From Redmine To IPF

        public override User ToUser(RedmineUser redmineUser)
        {
            return new User
            {
                Id = redmineUser.Id,
                Email = redmineUser.Email,
                Username = redmineUser.Login
            };
        }

        public override Project ToProject(RedmineProject redmineProject,
            IEnumerable<RedmineProjectMembership> projectMemberships)
        {
            var project = new Project
            {
                Id = redmineProject.Id,
                CreationDate = redmineProject.CreatedOn,
                Description = redmineProject.Description,
                LastUpdate = redmineProject.UpdatedOn,
                Name = redmineProject.Name,
                Members = projectMemberships.Select(m =>
                    new ProjectMembership
                    {
                        Id = m.Id.ToString(),
                        Role = m.Roles.Any() ? m.Roles[0].Name : null
                    })
            };
            if (redmineProject.Parent != null)
                project.ParentId = redmineProject.Parent.Id;
            return project;
        }

        public override RedmineProject ToPlatformProject(Project project)
        {
            var redmineProject = new RedmineProject
            {
                Id = project.Id,
                Identifier = project.Identifier,
                Name = project.Name,
                Description = project.Description,
                Parent = new IdentifiableName {Id = project.ParentId},
                CreatedOn = project.CreationDate,
                UpdatedOn = project.LastUpdate
            };
            // TODO: project memberships
            return redmineProject;
        }

        public override Issue ToIssue(RedmineIssue redmineIssue)
        {
            var issue = new Issue
            {
                Id = redmineIssue.Id,
                AuthorId = redmineIssue.Author.Id.ToString(),
                Description = redmineIssue.Description,
                Title = redmineIssue.Subject,
                ProjectId = redmineIssue.Project.Id,
                CreationDate = redmineIssue.CreatedOn,
                LastUpdate = redmineIssue.UpdatedOn,
                DueDate = redmineIssue.DueDate,
                AssigneeId = redmineIssue.AssignedTo.Id.ToString()
            };
            if (redmineIssue.ParentIssue != null)
                issue.ParentId = redmineIssue.ParentIssue.Id.ToString();

            return issue;
        }

        public override RedmineIssue ToPlatformIssue(Issue issue)
        {
            return new RedmineIssue
            {
                Id = issue.Id,
                Subject = issue.Title
            };
        }

        #endregion
    }
}