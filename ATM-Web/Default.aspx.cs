using System;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ATMWeb
{

    public class User {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Balance { get; set; }
    }

    public partial class Default : System.Web.UI.Page
    {
        User loggedInUser = null;

        private string errorLabel = "<b>Error</b>";
        private string loginErrorText;
        private string signupErrorText;


        private static readonly HttpClient client = new HttpClient();

        // Initialize the page
        public void Page_Load() {
            if (!IsPostBack)
            {
                // tell the client where to look for its data
                client.BaseAddress = new Uri("http://localhost:5000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }

            signupErrorText = loginErrorText = errorLabel;
        }

        // Called before the page is rendered.
        // Makes last-minute changes to what is displayed on the page
        // (i.e. what to show, what to hide)
        public void Page_PreRender() {
            if (loggedInUser != null)
            {
                formLogin.Visible = false;
                formSignUp.Visible = false;
                formTransaction.Visible = true;
            }

            if (loginErrorText != errorLabel) {
                lblLoginError.Text = loginErrorText.Trim();
                formLoginError.Visible = true;
            }

            if (signupErrorText != errorLabel)
            {
                lblSignupError.Text = signupErrorText.Trim();
                formSignupError.Visible = true;
            }

            signupErrorText = loginErrorText = errorLabel;
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            bool input_valid = true;

            string firstName;
            string lastName;
            int pin;

            if(string.IsNullOrWhiteSpace(firstName = firstNameText.Text)) {
                loginErrorText += "<br>" + "Please enter your first name.";
                input_valid = false;
            }
               
            if(string.IsNullOrWhiteSpace(lastName = lastNameText.Text)) {
                loginErrorText += "<br>" + "Please enter your last name.";
                input_valid = false;
            }

            if (string.IsNullOrWhiteSpace(PINText.Text))
            {
                loginErrorText += "<br>" + "Please enter your PIN.";
                input_valid = false;
            } else {
                try {
                    pin = Int32.Parse(PINText.Text);

                    if (input_valid)
                    {
                        var user = CheckLogin(firstName, lastName, pin).GetAwaiter().GetResult();

                        if (user != null)
                        {
                            loggedInUser = user;
                        }
                        else
                        {
                            loginErrorText += "<br>" + "Invalid name or PIN.";
                        }
                    }
                } catch (FormatException) {
                    loginErrorText += "<br>" + "Invalid PIN.";
                    input_valid = false;
                }
            }
        }

        // Display the signup form.
        protected void Signup_Click(object sender, EventArgs e)
        {
            formLogin.Visible = false;
            formSignUp.Visible = true;
            formTransaction.Visible = false;
        }

        protected void SignupSubmit_Click(object sender, EventArgs e)
        {
            bool input_valid = true;
            string firstName;
            string lastName;
            int pin;
            int pinConfirmation;

            if (string.IsNullOrWhiteSpace(firstName = firstNameSignupText.Text))
            {
                signupErrorText += "<br/>" + "Please enter your first name.";
                input_valid = false;
            }

            if (string.IsNullOrWhiteSpace(lastName = lastNameSignupText.Text))
            {
                signupErrorText += "<br/>" + "Please enter your last name.";
                input_valid = false;
            }

            if (string.IsNullOrWhiteSpace(PINSignupText.Text))
            {
                signupErrorText += "<br/>" + "Please enter a PIN.";
                input_valid = false;
            }
            else
            {
                try
                {
                    pin = Int32.Parse(PINSignupText.Text);

                    if (string.IsNullOrWhiteSpace(PINSignupConfirmText.Text))
                    {
                        signupErrorText += "<br/>" + "Please confirm your PIN.";
                        input_valid = false;
                    }
                    else
                    {
                        try
                        {
                            pinConfirmation = Int32.Parse(PINSignupConfirmText.Text);

                            if(pin != pinConfirmation) {
                                signupErrorText += "<br/>" + "PINs do not match.";
                                input_valid = false;
                            } else {
                                if (input_valid)
                                {
                                    var user = CreateNewUser(firstName, lastName, pin).GetAwaiter().GetResult();

                                    if (user != null)
                                    {
                                        loggedInUser = user;
                                    }
                                }
                            }
                        }
                        catch (FormatException)
                        {
                            signupErrorText += "<br/>" + "Invalid PIN confirmation.";
                            input_valid = false;
                        }
                    }
                }
                catch (FormatException)
                {
                    signupErrorText += "<br/>" + "Invalid PIN.";
                    input_valid = false;
                }
            }

        }

        async Task<User> CheckLogin(string firstName, string lastName, int pin) {
            User user = null;

            HttpResponseMessage response = await client.GetAsync($"api/login?firstName={firstName}&lastName={lastName}&pin={pin}");

            if (response.IsSuccessStatusCode)
            {
                // get data as a JSON string
                string data = await response.Content.ReadAsStringAsync();

                //use JavaScriptSerializer from System.Web.Script.Serialization
                JavaScriptSerializer JSserializer = new JavaScriptSerializer();

                // deserialize response to User class
                user = JSserializer.Deserialize<User>(data);
            }

            return user;
        }

        async Task<User> CreateNewUser(string firstName, string lastName, int pin)
        {
            User user = null;

            // create a JSON representing the user.
            string userRequest = "{\"FirstName\":\"" + firstName + "\"," 
                                + "\"LastName\":\"" + lastName + "\","
                                + "\"Balance\":\"" + 0.0 + "\"," 
                                + "\"PIN\":\"" + pin + "\"}";

            // convert the JSON string to httpContent so we can send it using HttpResponseMessage
            var httpContent = new StringContent(userRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"api/account", httpContent);

            if (response.IsSuccessStatusCode)
            {
                // get data as a JSON string
                string data = await response.Content.ReadAsStringAsync();

                //use JavaScriptSerializer from System.Web.Script.Serialization
                JavaScriptSerializer JSserializer = new JavaScriptSerializer();

                // deserialize response to User class
                user = JSserializer.Deserialize<User>(data);

            } else {
                // if the username is already in use, the server will return a 409 error.
                if(response.StatusCode == HttpStatusCode.Conflict) {
                    signupErrorText += "<br>" + "User already exists. Please login.";
                }
            }

            // return the newly created user.
            return user;
        }

    }
}
