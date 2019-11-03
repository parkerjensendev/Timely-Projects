using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Extensions.CognitoAuthentication;
using TimelyProjects.Helpers;
using TimelyProjects.Models;
using Xamarin.Forms;

namespace TimelyProjects.Views
{
    public partial class SignInSignUp : ContentPage
    {
        private CognitoHelper _cognitoHelper;
        public SignInSignUp()
        {
            InitializeComponent();
            _cognitoHelper = new CognitoHelper();
            if (Application.Current.Properties.ContainsKey("User"))
            {
                EmailEntry.Text = (Application.Current.Properties["User"] as User).EmailAddress;
            }
        }

        private void CancelForgotPassword_Clicked(object sender, EventArgs e)
        {
            ForgotPasswordGrid.FadeTo(0, 1000);
            ForgotPasswordGrid.IsVisible = false;
            SignInGrid.IsVisible = true;
            SignInGrid.FadeTo(1, 1000);
        }

        private void ForgotPassword_Clicked(object sender, EventArgs e)
        {
            SignInGrid.FadeTo(0, 1000);
            SignInGrid.IsVisible = false;
            ForgotPasswordGrid.IsVisible = true;
            ForgotPasswordGrid.FadeTo(1, 1000);
        }

        private async void ResetPassword_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ForgotPasswordEmailEntry.Text) || !RegexHelper.IsValidEmail(ForgotPasswordEmailEntry.Text))
            {
                ForgotPasswordError.Text = "Please enter a valid email address.";
                ForgotPasswordError.IsVisible = true;
            }
            else
            {
                await _cognitoHelper.ForgotPassword(ForgotPasswordEmailEntry.Text.ToLower());
            }
        }

        private void CreateAccount_Clicked(object sender, EventArgs e)
        {
            SignInGrid.FadeTo(0, 1000);
            SignInGrid.IsVisible = false;
            CreateAccountGrid.IsVisible = true;
            CreateAccountGrid.FadeTo(1, 1000);
        }

        private void AlreadyHaveAccount_Clicked(object sender, EventArgs e)
        {
            CreateAccountGrid.FadeTo(0, 1000);
            CreateAccountGrid.IsVisible = false;
            SignInGrid.IsVisible = true;
            SignInGrid.FadeTo(1, 1000);
        }

        private async void EmailConfirmed_Clicked(object sender, EventArgs e)
        {
            await SignIn(NewEmailEntry.Text, NewPasswordEntry.Text);
        }

        private async void SignUp_Clicked(object sender, EventArgs e)
        {
            CreateAccountError.IsVisible = false;
            if (string.IsNullOrEmpty(NewEmailEntry.Text) || string.IsNullOrEmpty(NewPasswordEntry.Text) || string.IsNullOrEmpty(NewPasswordConfirmEntry.Text))
            {
                CreateAccountError.Text = "Please fill in all fields.";
                CreateAccountError.IsVisible = true;
                return;
            }
            else if (!RegexHelper.IsValidEmail(NewEmailEntry.Text))
            {
                CreateAccountError.Text = "Please enter a valid email address.";
                CreateAccountError.IsVisible = true;
                return;
            }
            else if (string.Compare(NewPasswordEntry.Text, NewPasswordConfirmEntry.Text) != 0)
            {
                CreateAccountError.Text = "The two passwords do not match. Please try Again.";
                CreateAccountError.IsVisible = true;
                return;
            }
            else if (NewPasswordEntry.Text.Length < 8 || !NewPasswordEntry.Text.Any(char.IsDigit) || !NewPasswordEntry.Text.Any(char.IsUpper) || !NewPasswordEntry.Text.Any(char.IsLower))
            {
                CreateAccountError.Text = "Your password must be at least 8 characters with an upercase letter, lowercase letter and a number";
                CreateAccountError.IsVisible = true;
                return;
            }
            Loading.IsVisible = true;
            CognitoContext context = await _cognitoHelper.SignUp(NewEmailEntry.Text.ToLower(), NewPasswordEntry.Text);
            switch (context.Result)
            {
                case CognitoResult.PasswordRequirementsFailed:
                    CreateAccountError.Text = "Your password must be at least 8 characters with an upercase letter, lowercase letter and a number";
                    CreateAccountError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                case CognitoResult.UserNameAlreadyUsed:
                    CreateAccountError.Text = "There is already an account using that email address.";
                    CreateAccountError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                case CognitoResult.SignupOk:
                    Loading.IsVisible = false;
                    CreateAccountGrid.FadeTo(0, 1000);
                    CreateAccountGrid.IsVisible = false;
                    ConfirmEmailGrid.IsVisible = true;
                    ConfirmEmailGrid.FadeTo(1, 1000);
                    break;
                default:
                    CreateAccountError.Text = "Something went wrong. Please try again later.";
                    CreateAccountError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
            }
        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            await SignIn(EmailEntry.Text, PasswordEntry.Text);
        }

        private async Task SignIn(string email = "", string password= "")
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SignInError.Text = "Please enter an email address and password.";
                SignInError.IsVisible = true;
                return;
            }
            else if (!RegexHelper.IsValidEmail(email))
            {
                SignInError.Text = "Please enter a valid email address.";
                SignInError.IsVisible = true;
                return;
            }
            SignInError.IsVisible = false;
            Loading.IsVisible = true;
            SignInContext context = await _cognitoHelper.SignIn(email.ToLower(), password);
            switch (context.Result)
            {
                case CognitoResult.NotConfirmed:
                    SignInError.Text = "You have not confirmed your email address. Please check your email and click on the provided link to confirm your email address and then try again.";
                    SignInError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                case CognitoResult.UserNotFound:
                    SignInError.Text = "No user with that email address was found.";
                    SignInError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                case CognitoResult.Error:
                    SignInError.Text = "Something went wrong. Please try again later.";
                    SignInError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                case CognitoResult.Ok:
                    await Application.Current.SavePropertiesAsync();
                    var cred = AmazonUtils.Credentials;
                    cred.AddLogin(Constants.PROVIDER_NAME, context.IdToken);
                    await GetUser(context.User.Username, context.User.UserID);
                    Loading.IsVisible = false;
                    Application.Current.MainPage = new NavigationPage(new MyHome());
                    
                    break;
                case CognitoResult.NotAuthorized:
                    SignInError.Text = "Incorrect password.";
                    SignInError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
                default:
                    SignInError.Text = "Something went wrong. Please try again later.";
                    SignInError.IsVisible = true;
                    Loading.IsVisible = false;
                    break;
            }
        }

        private async Task GetUser(string userId, string email)
        {
            var context = AmazonUtils.DDBContext;
            User user = await context.LoadAsync<User>(userId);
            if(user == null)
            {
                user = new User()
                {
                    FirstName = "",
                    LastName = "",
                    MobileNumber = "",
                    EmailAddress = email,
                    UserId = userId,
                    OrganizationId = "2cdaa5d7-13e3-4660-9061-62d20512ed5d"
                };
                await context.SaveAsync<User>(user);
            }
            if (Application.Current.Properties.ContainsKey("User"))
            {
                Application.Current.Properties["User"] = user;
            }
            else
            {
                Application.Current.Properties.Add("User", user);
            }
        }
    }
}
