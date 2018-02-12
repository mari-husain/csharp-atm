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



        private static readonly HttpClient client = new HttpClient();

        public void Page_Load() {

            // tell the client where to look for its data
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var user = CheckLogin().GetAwaiter().GetResult();

            if(user != null) {
                formLogin.Visible = false;
                formTransaction.Visible = true;
            }
        }

        static async Task<User> CheckLogin() {
            User user = null;

            string firstName = "Mari";
            string lastName = "Husain";

            HttpResponseMessage response = await client.GetAsync($"api/account?firstName={firstName}&lastName={lastName}");

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
