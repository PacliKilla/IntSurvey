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
        string enteredText = entry1.Text + entry2.Text + entry3.Text + entry4.Text + entry5.Text + entry6.Text + entry7.Text + entry8.Text + entry9.Text;

        MyKey = enteredText;

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
                   entry1.Text = entry2.Text = entry3.Text = entry4.Text = entry5.Text = entry6.Text = entry7.Text = entry8.Text = entry9.Text = "";
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
    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // Remove non-numeric characters
        var numericText = new string(e.NewTextValue.Where(char.IsDigit).ToArray());

        if (e.NewTextValue != numericText)
        {
            entry.Text = numericText;
        }

        if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length == 1)
        {
            // Move focus to the next Entry when a digit is entered
            switch (entry)
            {
                case var _ when entry == entry1:
                    entry2.Focus();
                    break;
                case var _ when entry == entry2:
                    entry3.Focus();
                    break;
                case var _ when entry == entry3:
                    entry4.Focus();
                    break;
                case var _ when entry == entry4:
                    entry5.Focus();
                    break;
                case var _ when entry == entry5:
                    entry6.Focus();
                    break;
                case var _ when entry == entry6:
                    entry7.Focus();
                    break;
                case var _ when entry == entry7:
                    entry8.Focus();
                    break;
                case var _ when entry == entry8:
                    entry9.Focus();
                    break;
            }
        }
        else if (string.IsNullOrEmpty(e.NewTextValue))
        {
            // Move focus to the previous Entry when deleting a digit
            switch (entry)
            {
                case var _ when entry == entry9:
                    entry8.Focus();
                    break;
                case var _ when entry == entry8:
                    entry7.Focus();
                    break;
                case var _ when entry == entry7:
                    entry6.Focus();
                    break;
                case var _ when entry == entry6:
                    entry5.Focus();
                    break;
                case var _ when entry == entry5:
                    entry4.Focus();
                    break;
                case var _ when entry == entry4:
                    entry3.Focus();
                    break;
                case var _ when entry == entry3:
                    entry2.Focus();
                    break;
                case var _ when entry == entry2:
                    entry1.Focus();
                    break;
            }
        }
        if (!string.IsNullOrEmpty(entry1.Text) &&
            !string.IsNullOrEmpty(entry2.Text) &&
            !string.IsNullOrEmpty(entry3.Text) &&
            !string.IsNullOrEmpty(entry4.Text) &&
            !string.IsNullOrEmpty(entry5.Text) &&
            !string.IsNullOrEmpty(entry6.Text) &&
            !string.IsNullOrEmpty(entry7.Text) &&
            !string.IsNullOrEmpty(entry8.Text) &&
            !string.IsNullOrEmpty(entry9.Text))
        {
            // Trigger the button click event
            btnGoBackToHomePage_Clicked(sender, e);
        }
    }


}
