using System;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace TimelyProjects.Helpers
{
    public class AmazonUtils
    {
        private static CognitoAWSCredentials _credentials;

        public static CognitoAWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                {
                    _credentials = new CognitoAWSCredentials(Constants.COGNITO_IDENTITY_POOL_ID, Amazon.RegionEndpoint.USEast1);
                    _credentials.Clear();
                }
                return _credentials;
            }
        }

        public static AmazonDynamoDBClient _client;

        public static AmazonDynamoDBClient DynamoDBClient
        {
            get
            {
                if (_client == null)
                    _client = new AmazonDynamoDBClient(Credentials, Amazon.RegionEndpoint.USEast1);
                return _client;
            }
        }


        private static DynamoDBContext _context;

        public static DynamoDBContext DDBContext
        {
            get
            {
                if (_context == null)
                    _context = new DynamoDBContext(DynamoDBClient);
                return _context;
            }
        }
    }
}