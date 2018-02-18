using System;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace ATMWeb
{
    // TODO: Add encryption
    // TODO: Manage/display ATM's bill count
    // TODO: Consolidate error messages

    [Serializable]
    public class User {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Balance { get; set; }
        public int PIN { get; set; }

        public User() {
            FirstName = "John";
            LastName = "Doe";
            Balance = 1000;
        }

        public User(string _firstName, string _lastName, double _balance) {
            FirstName = _firstName;
            LastName = _lastName;
            Balance = _balance;
        }
    }

    public partial class Default : System.Web.UI.Page
    {
        private string errorLabel = "<b>Error</b>";
        private string loginErrorText;
        private string signupErrorText;
        private string depositErrorText;
        private string withdrawErrorText;

        private static readonly HttpClient client = new HttpClient();

        /* *
         * Initialize the page.
         * */
        public void Page_Load() {
            if (!IsPostBack)
            {
                // tell the client where to look for its data
                client.BaseAddress = new Uri("http://localhost:5000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                if(Request.Cookies["atm_bills"] == null){
                    Response.Cookies["atm_bills"].Value = "500";
                    Response.Cookies["atm_bills"].Expires = DateTime.Now.AddDays(1);
                }
            }

            withdrawErrorText = depositErrorText = signupErrorText = loginErrorText = errorLabel;
        }

        /* *
         * Called before the page is rendered.
         * Makes last-minute changes to what is displayed on the page
         * (i.e. what to show, what to hide) 
         * */
        public void Page_PreRender() {

            if (Session["loggedInUser"] != null && formLogin.Visible)
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

            if (depositErrorText != errorLabel) {
                lblDepositError.Text = depositErrorText.Trim();
                formDepositError.Visible = true;
            }

            if (withdrawErrorText != errorLabel)
            {
                lblWithdrawError.Text = withdrawErrorText.Trim();
                formWithdrawError.Visible = true;
            }

            withdrawErrorText = depositErrorText = signupErrorText = loginErrorText = errorLabel;
        }

        /* *
         * Called when the user clicks the "login" button in the login panel.
         * Initiates the user validation process.
         * */
        protected void Login_Click(object sender, EventArgs e)
        {
            clearAlerts();

            bool input_valid = true;

            string firstName;
            string lastName;
            int pin;

            // field validation
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

                    // if all fields are valid, initiate the user authentication process
                    if (input_valid)
                    {
                        var user = CheckLogin(firstName, lastName, pin).GetAwaiter().GetResult();

                        if (user != null)
                        {
                            // if the user successfully logged in, save their login info.
                            user.PIN = pin;
                            Session["loggedInUser"] = user;
                        }
                        else
                        {
                            // otherwise inform them that they failed authentication.
                            loginErrorText += "<br>" + "Invalid name or PIN.";
                        }
                    }
                } catch (FormatException) {
                    loginErrorText += "<br>" + "Invalid PIN.";
                    input_valid = false;
                }
            }
        }

        /* *
         * Called when the user clicks the "sign up" button from the login form.
         * Display the signup form.
         * */
        protected void Signup_Click(object sender, EventArgs e)
        {
            resetAllFields();

            formLogin.Visible = false;
            formSignUp.Visible = true;
            formTransaction.Visible = false;
        }

        /* *
         * Called when the user clicks the "sign up" button from the signup form.
         * Initiates the user creation process.
         * */
        protected void SignupSubmit_Click(object sender, EventArgs e)
        {
            clearAlerts();

            string firstName;
            string lastName;
            int pin;
            int pinConfirmation;
            bool input_valid = true;

            // validate user input
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
                                    // if user input is valid, post the new user to the server.
                                    var user = CreateNewUser(firstName, lastName, pin).GetAwaiter().GetResult();

                                    if (user != null)
                                    {
                                        // if the user was successfully created, log them in and save their login info.
                                        user.PIN = pin;
                                        Session["loggedInUser"] = user;
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

        protected void ToHome_Click(object sender, EventArgs e) {
            resetAllFields();

            formLogin.Visible = true;
            formSignUp.Visible = false;
            formTransaction.Visible = false;
            formDeposit.Visible = false;
            formWithdraw.Visible = false;
            formInquiry.Visible = false;
        }

        protected void InitDeposit_Click(object sender, EventArgs e) {
            formTransaction.Visible = false;
            formDeposit.Visible = true;
        }

        protected void Deposit_Click(object sender, EventArgs e)
        {
            clearAlerts();

            int depositAmt;

            if(Int32.TryParse(amountDepositText.Text, out depositAmt)) {

                // update the user to their new balance amount
                ((User)Session["loggedInUser"]).Balance += depositAmt;

                // POST the updated user balance to the server
                UpdateUser((User)Session["loggedInUser"]).GetAwaiter();

                formDepositSuccess.Visible = true;
            } else {
                depositErrorText += "<br />" + "Invalid amount.";
            }
        }

        protected void InitWithdraw_Click(object sender, EventArgs e)
        {
            formTransaction.Visible = false;
            formWithdraw.Visible = true;
        }

        protected void Withdraw_Decr(object sender, EventArgs e)
        {
            int amountWithdraw = Int32.Parse(amountWithdrawText.Text);

            if (amountWithdraw > 20)
            {
                amountWithdrawText.Text = "" + (amountWithdraw - 20);
            }
        }

        protected void Withdraw_Incr(object sender, EventArgs e)
        {
            int amountWithdraw = Int32.Parse(amountWithdrawText.Text);
            int atmBills = Int32.Parse(Request.Cookies["atm_bills"].Value);

            if(amountWithdraw / 20 < atmBills) {
                amountWithdrawText.Text = "" + (amountWithdraw + 20);
            }
        }

        protected void Withdraw_Click(object sender, EventArgs e)
        {
            clearAlerts();

            int withdrawalAmt = Int32.Parse(amountWithdrawText.Text);

            // check that the user has sufficient funds to withdraw from.
            if(((User)Session["loggedInUser"]).Balance >= withdrawalAmt) {

                // check that the AMT has sufficient bills to dispense.
                if(Int32.Parse(Request.Cookies["atm_bills"].Value) / 20 > withdrawalAmt / 20 ) {

                    // update the user to their new balance amount
                    ((User)Session["loggedInUser"]).Balance -= withdrawalAmt;

                    // POST the updated user balance to the server.
                    UpdateUser((User)Session["loggedInUser"]).GetAwaiter();

                    // update the number of bills remaining in the ATM according to how many we dispensed.
                    HttpCookie cookie = new HttpCookie("atm_bills");
                    cookie.Value = "" + (Int32.Parse(Request.Cookies["atm_bills"].Value) - withdrawalAmt / 20);
                    cookie.Expires = Request.Cookies["atm_bills"].Expires;

                    Response.Cookies.Add(cookie);

                    // display success message.
                    formWithdrawSuccess.Visible = true;
                } else {
                    withdrawErrorText += "<br />" + "ATM does not have enough bills.";
                }
            } else {
                withdrawErrorText += "<br />" + "Insufficent funds.";
            }
        }

        protected void InitInquiry_Click(object sender, EventArgs e)
        {
            formTransaction.Visible = false;
            formInquiry.Visible = true;

            User loggedInUser = (User)Session["loggedInUser"];

            balanceLbl.Text = "" + loggedInUser.Balance;
        }

        /* *
         * Sent a GET request to a server to authenticate the given user.
         * */
        private async Task<User> CheckLogin(string firstName, string lastName, int pin) {
            User user = null;

            HttpResponseMessage response = await client.GetAsync($"api/login?firstName={firstName}&lastName={lastName}&pin={pin}");

            if (response.IsSuccessStatusCode)
            {
                // get data as a JSON string
                string data = await response.Content.ReadAsStringAsync();

                // deserialize response to User class
                user = JsonConvert.DeserializeObject<User>(data);
            }

            return user;
        }

        /* *
         * Sent a POST request to a server to create the given user.
         * */
        private async Task<User> CreateNewUser(string firstName, string lastName, int pin)
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

        /* *
         * Sent a POST request to a server to create the given user.
         * */
        private async Task UpdateUser(User user)
        {
            // create a JSON representing the user.
            string userRequest = "{\"FirstName\":\"" + user.FirstName + "\","
                                + "\"LastName\":\"" + user.LastName + "\","
                                + "\"Balance\":\"" + user.Balance + "\","
                                + "\"PIN\":\"" + user.PIN + "\"}";

            // convert the JSON string to httpContent so we can send it using HttpResponseMessage
            var httpContent = new StringContent(userRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"api/update", httpContent);
        }

        private void resetAllFields() {
            firstNameText.Text = "";
            lastNameText.Text = "";
            PINText.Text = "";

            loginErrorText = errorLabel;
            formLoginError.Visible = false;

            firstNameSignupText.Text = "";
            lastNameSignupText.Text = "";
            PINSignupText.Text = "";
            PINSignupConfirmText.Text = "";

            signupErrorText = errorLabel;
            formSignupError.Visible = false;

            amountDepositText.Text = "";
            depositErrorText = errorLabel;
            formDepositError.Visible = false;
            formDepositSuccess.Visible = false;

            amountWithdrawText.Text = "20";
            withdrawErrorText = errorLabel;
            formWithdrawError.Visible = false;
            formWithdrawSuccess.Visible = false;
        }

        private void clearAlerts() {
            loginErrorText = errorLabel;
            formLoginError.Visible = false;

            signupErrorText = errorLabel;
            formSignupError.Visible = false;

            depositErrorText = errorLabel;
            formDepositError.Visible = false;
            formDepositSuccess.Visible = false;

            withdrawErrorText = errorLabel;
            formWithdrawError.Visible = false;
        }

    }
}
