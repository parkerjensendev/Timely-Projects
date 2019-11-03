using System;
using Amazon.DynamoDBv2.DataModel;

namespace TimelyProjects.Models
{
    [DynamoDBTable("Organization")]
    public class Project
    {
        [DynamoDBProperty]
        public string ProjectId { get; set; }

        [DynamoDBProperty]
        public string OrganizationId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public DateTime StartDate { get; set; }

        [DynamoDBProperty]
        public string Icon { get; set; }
    }
}
