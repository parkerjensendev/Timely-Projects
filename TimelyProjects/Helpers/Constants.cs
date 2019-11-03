using System;
namespace TimelyProjects.Helpers
{
    public class Constants
    {
        public const string COGNITO_USER_POOL_ID = "us-east-1_CP4Pzfnfd";
        public const string COGNITO_CLIENT_ID = "1763d71j3n4pejsmde6do3j9m1";
        public const string COGNITO_IDENTITY_POOL_ID = "us-east-1:c41b89f6-49b7-4d85-aa61-5aa7bae5df91";
        public const string PROVIDER_NAME = "cognito-idp.us-east-1.amazonaws.com/us-east-1_CP4Pzfnfd";
        public Amazon.RegionEndpoint COGNITO_REGION = Amazon.RegionEndpoint.USEast1;

        public const string PROJECT_TABLE_NAME = "Project";
        public const string PROJECT_ORGANIZATION_INDEX = "OrganizationId-index";

    }
}
