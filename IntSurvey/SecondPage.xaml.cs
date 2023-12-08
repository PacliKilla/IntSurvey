using CommunityToolkit.Maui.Behaviors;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IntSurvey.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace IntSurvey;


public partial class SecondPage : ContentPage
{
    public string MyKey { get; set; }
    public string baseURL = "https://dev.edi.md/ISNPSAPI/Mobile/ActivateDevice?ActivationCode=";
    public string fullURl => $"{baseURL}{MyKey}";
    string username = "uSr_nps";
    string password = "V8-}W31S!l'D";

    public SecondPage()
    {
        InitializeComponent();
        BindingContext = this;
        //SecureStorage.Default.RemoveAll();
        NavigationPage.SetHasNavigationBar(this, false);




        
        string id = SecureStorage.GetAsync("LicenseID").Result;
        if (!string.IsNullOrEmpty(id))
        {
            Navigation.PushAsync(new HomePage());
        }

    }

    private async void btnGoBackToHomePage_Clicked(object sender, EventArgs e)
    {
       // if (keyValidator.IsNotValid)
       // {
        //    foreach (var error in keyValidator.Errors)
        //    {
         //       DisplayAlert("Error", error.ToString(), "OK");
         //   }
        //    return;
       // }

        string enteredText = MyKey;

        using (HttpClient client = new HttpClient())
        {
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            try
            {
                string json = client.GetStringAsync(fullURl).Result;
                keyVM CodeID = JsonConvert.DeserializeObject<keyVM>(json);
                if (CodeID.id == "00000000-0000-0000-0000-000000000000")
                {
                    await DisplayAlert("Alertă", "Cheia introdusă nu este validă..", "OK");
                    return; 
                }

                await SecureStorage.SetAsync("LicenseID", CodeID.id);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("License not exist"))
                {
                    DisplayAlert("Error", "License does not exist", "OK");
                }
                else
                {
                    throw;
                }
            }
        }

        Navigation.PushAsync(new HomePage());
    }
}
