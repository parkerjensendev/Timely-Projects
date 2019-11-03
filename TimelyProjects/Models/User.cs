using System;
using Amazon.DynamoDBv2.DataModel;

namespace TimelyProjects.Models
{
    [DynamoDBTable("User")]
    public class User
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        [DynamoDBProperty]
        public string FirstName { get; set; }

        [DynamoDBProperty]
        public string LastName { get; set; }

        [DynamoDBProperty]
        public string MobileNumber { get; set; }

        [DynamoDBProperty]
        public string EmailAddress { get; set; }

        [DynamoDBProperty]
        public string OrganizationId { get; set; }
    }
}
