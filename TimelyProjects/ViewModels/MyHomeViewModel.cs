using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TimelyProjects.Helpers;
using TimelyProjects.Models;
using Xamarin.Forms;

namespace TimelyProjects.ViewModels
{
    public class MyHomeViewModel : ObservableProperties
    {
        #region backing variables

        private bool _isLoading = true;
        private User _user;

        #endregion

        public MyHomeViewModel()
        {
            _user = Application.Current.Properties["User"] as User;
            Projects = new ObservableCollection<Project>();
        }

        #region properties

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if(_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Project> Projects;

        #endregion

        #region methods

        public async Task LoadData()
        {
            IsLoading = true;
            await LoadProjects();
            IsLoading = false;
        }

        private async Task LoadProjects()
        {
            try
            {
                var query = new QueryRequest()
                {
                    TableName = Constants.PROJECT_TABLE_NAME,
                    IndexName = Constants.PROJECT_ORGANIZATION_INDEX,
                    KeyConditionExpression = $"OrganizationId = :org_id",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {
                            ":org_id", new AttributeValue
                            {
                                S = _user.OrganizationId
                            }
                        }
                    }
                };
                var queryResponse = await AmazonUtils.DynamoDBClient.QueryAsync(query);
                queryResponse.Items.ForEach((project) =>
                {
                    Projects.Add(new Project()
                    {
                        ProjectId = project["ProjectId"].S,
                        Name = project["Name"].S,
                    //StartDate = project["StartDate"].S),
                    OrganizationId = project["OrganizationId"].S,
                        Icon = project["Icon"].S
                    });
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        #endregion
    }
}
