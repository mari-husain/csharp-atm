﻿using System;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using BCrypt.Net;

namespace ATMWeb
{
    /* *
     * CodeBehind - Homepage for the ATM Web App.
     * */
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
                // tell the client where the API is located
                //client.BaseAddress = new Uri("http://localhost:5000/");
                client.BaseAddress = new Uri("https://atm-backend.herokuapp.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            // if necessary, get the ATM from the server,
            // set our ATM label text to reflect the bills contained in the ATM
            if (Session["atm"] == null)
            {
                var atm = GetATMBills().GetAwaiter().GetResult();

                if (atm != null)
                {
                    Session["atm"] = atm;
                    setLblATMBalance(atm);
                }
            } else {
                setLblATMBalance((ATM)Session["atm"]);
            }

            // reset error messages
            withdrawErrorText = depositErrorText = signupErrorText = loginErrorText = errorLabel;
        }

        /* *
         * Called before the page is rendered.
         * Makes last-minute changes to what is displayed on the page
         * (i.e. what to show, what to hide) 
         * */
        public void Page_PreRender() {
            
            // if we are logged in, show the logout button.
            if(Session["loggedInUser"] != null) {
                btnLogout.Visible = true;
            } else {
                btnLogout.Visible = false;
            }

            // if we are logged in, hide the login/signup page and show the transaction page.
            if (Session["loggedInUser"] != null && (formLogin.Visible || formSignUp.Visible))
            {
                formLogin.Visible = false;
                formSignUp.Visible = false;
                formTransaction.Visible = true;
            }

            // if there is an error in the login form, show the error message.
            if (loginErrorText != errorLabel) {
                lblLoginError.Text = loginErrorText.Trim();
                formLoginError.Visible = true;
            }

            // if there is an error in the signup form, show the error message.
            if (signupErrorText != errorLabel)
            {
                lblSignupError.Text = signupErrorText.Trim();
                formSignupError.Visible = true;
            }

            // if there is an error in the deposit form, show the error message.
            if (depositErrorText != errorLabel) {
                lblDepositError.Text = depositErrorText.Trim();
                formDepositError.Visible = true;
            }

            // if there is an error in the withdraw form, show the error message.
            if (withdrawErrorText != errorLabel)
            {
                lblWithdrawError.Text = withdrawErrorText.Trim();
                formWithdrawError.Visible = true;
            }

            // reset the error labels.
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

                    if(pin < 0) {
                        throw new FormatException();
                    }

                    // if all fields are valid, initiate the user authentication process
                    if (input_valid)
                    {
                        var user = GetUserByName(firstName, lastName).GetAwaiter().GetResult();

                        // verify password
                        if (user != null && BCrypt.Net.BCrypt.Verify(pin.ToString(), user.PINHash))
                        {
                            // if the user successfully logged in, save their login info.
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
            string pinHash;
            bool input_valid = true;

            // input validation
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
                    if(pin < 0) {
                        throw new FormatException();
                    }

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
                            if(pinConfirmation < 0) {
                                throw new FormatException();
                            }

                            if(pin != pinConfirmation) {
                                signupErrorText += "<br/>" + "PINs do not match.";
                                input_valid = false;
                            } else {
                                if (input_valid)
                                {
                                    // if user input is valid, hash the password and post the new user to the server.
                                    pinHash = BCrypt.Net.BCrypt.HashPassword(pin.ToString(), BCrypt.Net.BCrypt.GenerateSalt(12));
                                    var user = CreateNewUser(firstName, lastName, pinHash).GetAwaiter().GetResult();

                                    if (user != null)
                                    {
                                        // if the user was successfully created, log them in and save their login info.
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

        /* *
         * Takes the user to the homepage.
         * */
        protected void ToHome_Click(object sender, EventArgs e) {
            resetAllFields();

            formLogin.Visible = true;
            formSignUp.Visible = false;
            formTransaction.Visible = false;
            formDeposit.Visible = false;
            formWithdraw.Visible = false;
            formInquiry.Visible = false;
        }

        /* *
         * Called when the user clicks the "deposit" button from the transaction form.
         * Display the deposit form. 
         * */
        protected void InitDeposit_Click(object sender, EventArgs e) {
            formTransaction.Visible = false;
            formDeposit.Visible = true;
        }

        /* *
         * Called when the user clicks the "deposit" button from the deposit form.
         * Initiates the deposit process.
         * */
        protected void Deposit_Click(object sender, EventArgs e)
        {
            clearAlerts();

            Button btn = (Button)sender;
            string id = btn.ID;
            ATM atm = (ATM)Session["atm"];

            decimal depositAmt = 0;

            // update the amount of bills the ATM contains
            switch (id)
            {
                case "btn_01":
                    depositAmt = 0.01M;
                    atm.Pennies++;
                    break;
                case "btn_05":
                    depositAmt = 0.05M;
                    atm.Nickels++;
                    break;
                case "btn_010":
                    depositAmt = 0.10M;
                    atm.Dimes++;
                    break;
                case "btn_025":
                    depositAmt = 0.25M;
                    atm.Quarters++;
                    break;
                case "btn_1":
                    depositAmt = 1;
                    atm.Ones++;
                    break;
                case "btn_5":
                    depositAmt = 5;
                    atm.Fives++;
                    break;
                case "btn_10":
                    depositAmt = 10;
                    atm.Tens++;
                    break;
                case "btn_20":
                    depositAmt = 20;
                    atm.Twenties++;
                    break;
                case "btn_50":
                    depositAmt = 50;
                    atm.Fifties++;
                    break;
                default:
                    break;
            }

            if(depositAmt > 0) {
                
                // update the user to their new balance amount
                ((User)Session["loggedInUser"]).Balance += depositAmt;

                // POST the updated user balance to the server
                UpdateUser((User)Session["loggedInUser"]).GetAwaiter();

                // POST the udpated ATM balance to the server
                UpdateATM((ATM)Session["atm"]).GetAwaiter();

                // display success message
                formDepositSuccess.Visible = true;
            }
        }

        /* *
         * Called when the user clicks the "withdraw" button from the transaction form.
         * Displays the withdraw form.
         * */
        protected void InitWithdraw_Click(object sender, EventArgs e)
        {
            formTransaction.Visible = false;
            formWithdraw.Visible = true;
        }

        /* *
         * Called when the user clicks the "-" button on the withdraw form.
         * Decrements the amount to be withdrawn by $20.
         * */
        protected void Withdraw_Decr(object sender, EventArgs e)
        {
            int amountWithdraw = Int32.Parse(amountWithdrawText.Text);

            if (amountWithdraw > 20)
            {
                amountWithdrawText.Text = "" + (amountWithdraw - 20);
            }
        }

        /* *
         * Called when the user clicks the "+" button from the withdraw form.
         * Increments the amount to be withdrawn by $20.
         * */
        protected void Withdraw_Incr(object sender, EventArgs e)
        {
            int amountWithdraw = Int32.Parse(amountWithdrawText.Text);

            if(amountWithdraw / 20 < ((ATM)Session["atm"]).Twenties) {
                amountWithdrawText.Text = "" + (amountWithdraw + 20);
            }
        }

        /* *
         * Called when the user clicks the "withdraw" button from the withdraw form.
         * Initiates the withdraw process.
         * */
        protected void Withdraw_Click(object sender, EventArgs e)
        {
            clearAlerts();

            int withdrawalAmt = Int32.Parse(amountWithdrawText.Text);

            // check that the user has sufficient funds to withdraw from.
            if(((User)Session["loggedInUser"]).Balance >= withdrawalAmt) {


                // check that the ATM has sufficient bills to dispense.
                if(((ATM)Session["atm"]).Twenties > withdrawalAmt / 20 ) {

                    // update the user to their new balance amount
                    ((User)Session["loggedInUser"]).Balance -= withdrawalAmt;

                    // POST the updated user balance to the server.
                    UpdateUser((User)Session["loggedInUser"]).GetAwaiter();

                    // update the number of bills remaining in the ATM according to how many we dispensed.
                    ((ATM)Session["atm"]).Twenties -= withdrawalAmt / 20;

                    // POST the updated ATM balance to the server.
                    UpdateATM((ATM)Session["atm"]).GetAwaiter();

                    // display success message.
                    formWithdrawSuccess.Visible = true;
                } else {
                    withdrawErrorText += "<br />" + "ATM does not have enough bills.";
                }
            } else {
                withdrawErrorText += "<br />" + "Insufficent funds.";
            }
        }

        /* *
         * Called when the user clicks the "balance inquiry" button from the transaction form.
         * Displays the balance inquiry form.
         * */
        protected void InitInquiry_Click(object sender, EventArgs e)
        {
            formTransaction.Visible = false;
            formInquiry.Visible = true;

            User loggedInUser = (User)Session["loggedInUser"];

            // display the user's balance.
            balanceLbl.Text = "" + String.Format("{0:C}", decimal.Round(loggedInUser.Balance, 2, MidpointRounding.AwayFromZero));
        }

        /* *
         * Called when the user clicks the "logout" button while signed in.
         * Logs the user out.
         * */
        protected void BtnLogout_Click(object sender, EventArgs e) {
            Session["loggedInUser"] = null;
            Response.Redirect(Request.RawUrl);
        }

        /* *
         * Sent a GET request to a server to authenticate the given user.
         * */
        private async Task<User> GetUserByName(string firstName, string lastName) {
            User user = null;

            using (HttpResponseMessage response = await client.GetAsync($"api/login?firstName={firstName.ToLower()}&lastName={lastName.ToLower()}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    // get data as a JSON string
                    string data = await response.Content.ReadAsStringAsync();

                    // deserialize response to User class
                    user = JsonConvert.DeserializeObject<User>(data);
                }
            }

            return user;
        }

        /* *
         * Sent a POST request to a server to create the given user.
         * */
        private async Task<User> CreateNewUser(string firstName, string lastName, string pinHash)
        {
            User user = null;

            // create a JSON representing the user.
            string userRequest = "{\"FirstName\":\"" + firstName.ToLower() + "\"," 
                                + "\"LastName\":\"" + lastName.ToLower() + "\","
                                + "\"Balance\":\"" + 0.0 + "\"," 
                                + "\"PINHash\":\"" + pinHash + "\"}";

            // convert the JSON string to httpContent so we can send it using HttpResponseMessage
            var httpContent = new StringContent(userRequest, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await client.PostAsync($"api/account", httpContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    // get data as a JSON string
                    string data = await response.Content.ReadAsStringAsync();

                    //use JavaScriptSerializer from System.Web.Script.Serialization
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();

                    // deserialize response to User class
                    user = JSserializer.Deserialize<User>(data);

                }
                else
                {
                    // if the username is already in use, the server will return a 409 error.
                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        signupErrorText += "<br>" + "User already exists. Please login.";
                    }
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
            string userRequest = "{\"FirstName\":\"" + user.FirstName.ToLower() + "\","
                                + "\"LastName\":\"" + user.LastName.ToLower() + "\","
                                + "\"Balance\":\"" + user.Balance + "\"}";

            // convert the JSON string to httpContent so we can send it using HttpResponseMessage
            var httpContent = new StringContent(userRequest, Encoding.UTF8, "application/json");

            using(HttpResponseMessage response = await client.PostAsync($"api/update", httpContent)) {
                // nothing
            }
        }

        /* *
         * Send a GET request to the server to get the ATM state.
         * */
        private async Task<ATM> GetATMBills() {
            ATM atm = null;

            using (HttpResponseMessage response = await client.GetAsync($"api/atm"))
            {
                if (response.IsSuccessStatusCode)
                {
                    // get data as a JSON string
                    string data = await response.Content.ReadAsStringAsync();

                    // deserialize response to User class
                    atm = JsonConvert.DeserializeObject<ATM>(data);
                }
            }

            return atm;
        }

        /* *
         * Send a POST request to update the ATM state.
         * */
        private async Task UpdateATM(ATM atm) {
            // create a JSON representing the ATM.
            string userRequest = "{\"Pennies\":\"" + atm.Pennies + "\","
                                + "\"Nickels\":\"" + atm.Nickels + "\","                         
                                + "\"Dimes\":\"" + atm.Dimes + "\","                         
                                + "\"Quarters\":\"" + atm.Quarters + "\","                         
                                + "\"Ones\":\"" + atm.Ones + "\","                         
                                + "\"Fives\":\"" + atm.Fives + "\","                         
                                + "\"Tens\":\"" + atm.Tens + "\","
                                + "\"Twenties\":\"" + atm.Twenties + "\","
                                + "\"Fifties\":\"" + atm.Fifties + "\"}";

            // convert the JSON string to httpContent so we can send it using HttpResponseMessage
            var httpContent = new StringContent(userRequest, Encoding.UTF8, "application/json");

            using(HttpResponseMessage response = await client.PostAsync($"api/atm", httpContent)) {
                // nothing
            }
        }

        /* *
         * Reset all fields and labels.
         * */
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

            depositErrorText = errorLabel;
            formDepositError.Visible = false;
            formDepositSuccess.Visible = false;

            amountWithdrawText.Text = "20";
            withdrawErrorText = errorLabel;
            formWithdrawError.Visible = false;
            formWithdrawSuccess.Visible = false;
        }

        /* *
         * Reset all labels.
         * */
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
            formWithdrawSuccess.Visible = false;
        }

        /* *
         * Update the ATM balance label to reflect the state of the given ATM.
         * */
        private void setLblATMBalance(ATM atm) {
            lblATMBalance.Text = "ATM Balance: " + "$0.01 - " + atm.Pennies + " | $0.05 - " + atm.Nickels + " | 0.10 - " + atm.Dimes + " | 0.25 - " + atm.Quarters 
                + " | $1 - " + atm.Ones + " | $5 - " + atm.Fives + " | $10 - " + atm.Tens + " | $20 - " + atm.Twenties + " | $50 - " + atm.Fifties;
            
        }
    }
}