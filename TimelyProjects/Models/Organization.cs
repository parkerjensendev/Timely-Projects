using System;
using Amazon.DynamoDBv2.DataModel;

namespace TimelyProjects.Models
{
    [DynamoDBTable("Organization")]
    public class Organization
    {
        [DynamoDBProperty]
        public string OrganizationId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string AdminId { get; set; }
    }
}
