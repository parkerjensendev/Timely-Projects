using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Xamarin.Essentials;

namespace TimelyProjects.Helpers
{
    public class CognitoHelper
    {                
        private static AmazonCognitoIdentityProviderClient cognitoIdentityProviderClient;
        public static AmazonCognitoIdentityProviderClient CognitoIdentityProviderClient
        {
            get
            {
                if (cognitoIdentityProviderClient == null)
                {
                    cognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient(AmazonUtils.Credentials, RegionEndpoint.USEast1);
                }

                return cognitoIdentityProviderClient;
            }
        }

        

        // Hold a reference to the Cognito User Pool
        private static CognitoUserPool cognitoUserPool;
        public static CognitoUserPool CognitoUserPool
        {
            get
            {
                if (cognitoUserPool == null)
                {
                    cognitoUserPool = new CognitoUserPool(Constants.COGNITO_USER_POOL_ID, Constants.COGNITO_CLIENT_ID, CognitoIdentityProviderClient);
                }

                return cognitoUserPool;
            }
        }
        public CognitoHelper()
        {
        }

        public async Task<SignInContext> SignIn(string userName, string password)
        {
            try
            {
                var something = CognitoIdentityProviderClient.ToString();
                CognitoUser user = new CognitoUser(userName, Constants.COGNITO_CLIENT_ID, CognitoUserPool, CognitoIdentityProviderClient);

                AuthFlowResponse context = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                {
                    Password = password
                }).ConfigureAwait(false);
                

                // TODO handle other challenges
                if (context.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
                    return new SignInContext(CognitoResult.PasswordChangeRequred)
                    {
                        //User = user,
                        SessionId = context.SessionID
                    };
                else
                {
                    return new SignInContext(CognitoResult.Ok)
                    {
                        User = user,
                        IdToken = context.AuthenticationResult?.IdToken,
                        RefreshToken = context.AuthenticationResult?.RefreshToken,
                        AccessToken = context.AuthenticationResult?.AccessToken,
                        TokenIssued = user.SessionTokens.IssuedTime,
                        Expires = user.SessionTokens.ExpirationTime,
                        SessionId = context.SessionID
                    };
                }
            }
            catch (Amazon.CognitoIdentityProvider.Model.NotAuthorizedException)
            {
                return new SignInContext(CognitoResult.NotAuthorized);
            }
            catch (UserNotFoundException)
            {
                return new SignInContext(CognitoResult.UserNotFound);
            }
            catch (UserNotConfirmedException)
            {
                return new SignInContext(CognitoResult.NotConfirmed);
            }
            catch (Exception e)
            {
                Console.WriteLine($"SignIn() threw an exception {e}");
            }
            return new SignInContext(CognitoResult.Unknown);
        }


        public async Task<SignInContext> RefreshToken(string userName, string idToken, string accessToken, String refreshToken, DateTime issued, DateTime expires)
        {
            try
            {
                CognitoUserPool userPool = new CognitoUserPool(Constants.COGNITO_USER_POOL_ID, Constants.COGNITO_CLIENT_ID, CognitoIdentityProviderClient);
                CognitoUser user = new CognitoUser("", Constants.COGNITO_CLIENT_ID, CognitoUserPool, CognitoIdentityProviderClient);

                user.SessionTokens = new CognitoUserSession(idToken, accessToken, refreshToken, issued, expires);

                AuthFlowResponse context = await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest
                {
                    AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                })
                .ConfigureAwait(false);

                // TODO handle other challenges
                return new SignInContext(CognitoResult.Ok)
                {
                    //User = user,
                    IdToken = context.AuthenticationResult?.IdToken,
                    RefreshToken = context.AuthenticationResult?.RefreshToken,
                    AccessToken = context.AuthenticationResult?.AccessToken,
                    TokenIssued = user.SessionTokens.IssuedTime,
                    Expires = user.SessionTokens.ExpirationTime,
                    SessionId = context.SessionID
                };
            }
            catch (Amazon.CognitoIdentityProvider.Model.NotAuthorizedException)
            {
                return new SignInContext(CognitoResult.NotAuthorized);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RefreshToken() threw an exception {e}");
            }
            return new SignInContext(CognitoResult.Unknown);
        }

        public async Task<CognitoContext> SignUp(string userName, string password)
        {
            try
            {
                var result = await CognitoIdentityProviderClient.SignUpAsync(new SignUpRequest
                {
                    ClientId = Constants.COGNITO_CLIENT_ID,
                    Password = password,
                    Username = userName,
                });

                Console.WriteLine("Signed in.");

                return new CognitoContext(CognitoResult.SignupOk);
            }
            catch (UsernameExistsException)
            {
                return new CognitoContext(CognitoResult.UserNameAlreadyUsed);
            }
            catch (Amazon.CognitoIdentityProvider.Model.InvalidParameterException)
            {
                return new CognitoContext(CognitoResult.PasswordRequirementsFailed);
            }
            catch (Exception e)
            {
                Console.WriteLine($"SignUp() threw an exception {e}");
            }
            return new CognitoContext(CognitoResult.Unknown);
        }

        public async Task<CognitoContext> ForgotPassword(string userName)
        {
            try
            {
                CognitoUserPool userPool = new CognitoUserPool(Constants.COGNITO_USER_POOL_ID, Constants.COGNITO_CLIENT_ID, CognitoIdentityProviderClient);
                CognitoUser user = new CognitoUser(userName, Constants.COGNITO_CLIENT_ID, CognitoUserPool, CognitoIdentityProviderClient);

                await user.ForgotPasswordAsync();

                return new CognitoContext(CognitoResult.Ok);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ForgotPassword() threw an exception {e}");
            }
            return new CognitoContext(CognitoResult.Unknown);
        }

        public async Task<CognitoContext> VerifyWithCode(string userName, string code)
        {
            try
            {
                var result = await CognitoIdentityProviderClient.ConfirmSignUpAsync(new ConfirmSignUpRequest
                {
                    ClientId = Constants.COGNITO_CLIENT_ID,
                    Username = userName,
                    ConfirmationCode = code
                });

                return new CognitoContext(CognitoResult.Ok);
            }
            catch (Exception e)
            {
                Console.WriteLine($"VerifyWithCode() threw an exception {e}");
            }
            return new CognitoContext(CognitoResult.Unknown);
        }

        public async Task<CognitoContext> UpdatePassword(string userName, string newPassword, string sessionId)
        {
            try
            {
                CognitoUserPool userPool = new CognitoUserPool(Constants.COGNITO_USER_POOL_ID, Constants.COGNITO_CLIENT_ID, CognitoIdentityProviderClient);
                CognitoUser user = new CognitoUser(userName, Constants.COGNITO_CLIENT_ID, CognitoUserPool, CognitoIdentityProviderClient);

                var res = await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest
                {
                    SessionID = sessionId,
                    NewPassword = newPassword
                });

                return new CognitoContext(CognitoResult.Ok);
            }
            catch (Exception e)
            {
                Console.WriteLine($"UpdatePassword() threw an exception {e}");
            }
            return new CognitoContext(CognitoResult.Unknown);
        }

    }
}

public enum CognitoResult
{
    Unknown,
    Ok,
    PasswordChangeRequred,
    SignupOk,
    NotAuthorized,
    Error,
    UserNotFound,
    UserNameAlreadyUsed,
    PasswordRequirementsFailed,
    NotConfirmed
}

public class CognitoContext
{
    public CognitoContext(CognitoResult res = CognitoResult.Unknown)
    {
        Result = res;
    }
    public CognitoResult Result { get; set; }
}

public class SignInContext : CognitoContext
{
    public SignInContext(CognitoResult res = CognitoResult.Unknown) : base(res)
    {
    }
    public CognitoUser User { get; set; }
    public String IdToken { get; set; }
    public String AccessToken { get; set; }
    public String RefreshToken { get; set; }
    public String SessionId { get; set; }
    public DateTime TokenIssued { get; set; }
    public DateTime Expires { get; set; }
}

public class CredentialsContext : CognitoContext
{
    public CredentialsContext(CognitoResult res = CognitoResult.Unknown) : base(res)
    {
    }
    //public CognitoUser User { get; set; }
    public string AccessKeyId { get; set; }
    public DateTime Expiration { get; set; }
    public string SecretKey { get; set; }
    public string SessionToken { get; set; }
}
