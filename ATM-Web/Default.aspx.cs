using System;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        private bool loggedIn = false;
        private string errorLabel = "<b>Error</b>";
        private string loginErrorText;


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

                loginErrorText = errorLabel;
            }
        }

        // Called before the page is rendered.
        // Makes last-minute changes to what is displayed on the page
        // (i.e. what to show, what to hide)
        public void Page_PreRender() {
            if (loggedIn)
            {
                formLogin.Visible = false;
                formTransaction.Visible = true;
            }

            if (loginErrorText != errorLabel) {
                lblLoginError.Text = loginErrorText.Trim();
                formLoginError.Visible = true;
            }
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            loginErrorText = errorLabel;
            bool input_valid = true;

            if(string.IsNullOrWhiteSpace(firstNameText.Text)) {
                loginErrorText += "<br>" + "Please enter your first name.";
                input_valid = false;
            }
               
            if(string.IsNullOrWhiteSpace(lastNameText.Text)) {
                loginErrorText += "<br>" + "Please enter your last name.";
                input_valid = false;
            }

            if (string.IsNullOrWhiteSpace(PINText.Text))
            {
                loginErrorText += "<br>" + "Please enter your PIN.";
                input_valid = false;
            }

            if(input_valid) {
                string firstName = firstNameText.Text;
                string lastName = lastNameText.Text;
                int pin = Int32.Parse(PINText.Text);

                var user = CheckLogin(firstName, lastName, pin).GetAwaiter().GetResult();

                if (user != null)
                {
                    loggedIn = true;
                }
                else
                {
                    loginErrorText += "<br>" + "Invalid name or PIN.";
                }
            }
        } 

        static async Task<User> CheckLogin(string firstName, string lastName, int pin) {
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


    }
}
