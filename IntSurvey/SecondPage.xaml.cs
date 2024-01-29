using CommunityToolkit.Maui.Behaviors;
using static System.Runtime.InteropServices.JavaScript.JSType;
using IntSurvey.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using IntSurvey.QuestionModels;

namespace IntSurvey;
using System.Security.Cryptography;

public partial class SecondPage : ContentPage
{
    private ServiceInfo serviceInfo;
    private ServiceInfo prodInfo;

    public string MyKey { get; set; }

    public string baseURL;
    public string fullURL => $"{baseURL}{MyKey}";

    public string settingUri;
    public string username ;
    public string password ;

    public SecondPage()
    {
        InitializeComponent();
        BindingContext = this;
        SecureStorage.Default.RemoveAll();

        NavigationPage.SetHasNavigationBar(this, false);

        testModeCheckBox.CheckedChanged += TestModeCheckBox_CheckedChanged;

        InitializeAsync(); // Call the asynchronous initialization method
    }

    private async void InitializeAsync()
    {
        InitializeHttpClient();


        serviceInfo = await LoadFromCache<ServiceInfo>("ServiceInfo");
        prodInfo = await LoadFromCache<ServiceInfo>("prodInfo");


        await CheckAndNavigateToHomePage();

    }
    private async Task CheckAndNavigateToHomePage()
    {
        string id = await SecureStorage.GetAsync("LicenseID");
        if (!string.IsNullOrEmpty(id))
        {
            AppCredentials.CacID = id;
            AppCredentials.Username = username;
            AppCredentials.Password = password;
            AppCredentials.Uri = settingUri;
            Navigation.PushAsync(new HomePage());
        }
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        testModeCheckBox.IsChecked = true;
        testModeCheckBox.IsChecked = !testModeCheckBox.IsChecked;
    }

    private void OnFrameTapped(object sender, EventArgs e)
    {

        testModeCheckBox.IsChecked = !testModeCheckBox.IsChecked;
    }




    private void TestModeCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    
    {
        
        
        if (testModeCheckBox.IsChecked)
        {
            baseURL = $"{serviceInfo.ServiceUri}/Mobile/ActivateDevice?ActivationCode=";
            username = serviceInfo.User;
            password = serviceInfo.Password;
            settingUri = serviceInfo.ServiceUri;
        }
        else
        {
            baseURL = $"{prodInfo.ServiceUri}/Mobile/ActivateDevice?ActivationCode=";
            username = prodInfo.User;
            password = prodInfo.Password;
            settingUri = prodInfo.ServiceUri;
        }


    }

    public async void InitializeHttpClient()
    {
        bool useServiceInfo = testModeCheckBox.IsChecked;

        static string _getHash(string input)
        {
            if (input == null)
                return null;
            if (input.Length == 0)
                return "";

            string strResult = "";
            MD5CryptoServiceProvider md5Maker = new MD5CryptoServiceProvider();
            UTF8Encoding enc = new UTF8Encoding();
            byte[] result = md5Maker.ComputeHash(enc.GetBytes(input));
            foreach (byte b in result)
                strResult += b.ToString("x2");
            return strResult;
        }
        string GetDecrypt(EncodedObj obj)
        {
            string k1 = _getHash(obj.URI);
            if (k1.Length < 32)
            {
                k1 = k1.PadRight(32, '0');
            }
            if (k1.Length > 32)
            {
                k1 = k1.Substring(0, 32);
            }

            string keyIV = _getHash(k1);
            if (keyIV.Length < 16)
            {
                keyIV = keyIV.PadRight(16, '0');
            }
            if (keyIV.Length > 16)
            {
                keyIV = keyIV.Substring(0, 16);
            }

            return AesCrypt.DecryptStringFromText(obj.Settings, k1, keyIV);
        }
        
            //dev
            HttpClient _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri("https://dev.edi.md/ISConfigManagerServiceAPI/app/GetServiceURI?Service=38");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("dev_config:Mg4%22_q!~io3lL")));

            var response = _httpClient.GetAsync(_httpClient.BaseAddress).Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var encodedObj = JsonConvert.DeserializeObject<EncodedObj>(content);

            string result = GetDecrypt(encodedObj);
            serviceInfo = JsonConvert.DeserializeObject<ServiceInfo>(result);

            SaveToCache("ServiceInfo", serviceInfo);
       
            //prod
            HttpClient prodHttpClient = new HttpClient();


            prodHttpClient.BaseAddress = new Uri("https://configurationmanager.eservicii.md/ISConfigManagerServiceAPI/app/GetServiceURI?Service=38");

            prodHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes("pcfgmnt_usr:1<1W3C@*mgy~")));

            var prodResponse = prodHttpClient.GetAsync(prodHttpClient.BaseAddress).Result;
            prodResponse.EnsureSuccessStatusCode();

            var prodContent = prodResponse.Content.ReadAsStringAsync().Result;
            var prodEncodedObj = JsonConvert.DeserializeObject<EncodedObj>(prodContent);

            string prodResult = GetDecrypt(prodEncodedObj);
            prodInfo = JsonConvert.DeserializeObject<ServiceInfo>(prodResult);

            SaveToCache("prodInfo", prodInfo);
        

    }

    private void SaveToCache<T>(string key, T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        SecureStorage.SetAsync(key, serializedValue);
    }

    private async Task<T> LoadFromCache<T>(string key)
    {
        try
        {
            var serializedValue = await SecureStorage.GetAsync(key);
            if (!string.IsNullOrEmpty(serializedValue))
            {
                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from cache: {ex.Message}");
        }

        return default(T);
    }





    public class AesCrypt
    {
        public static string DecryptStringFromText(string cipherText, string Key1, string Key2)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key1 == null || Key1.Length != 32)
                throw new ArgumentNullException("Key");
            if (Key2 == null || Key2.Length != 16)
                throw new ArgumentNullException("IV");
            var Key = System.Text.Encoding.ASCII.GetBytes(Key1);
            var IV = System.Text.Encoding.ASCII.GetBytes(Key2);
            var qcipher = System.Text.Encoding.ASCII.GetBytes(cipherText);
            var cipher = Convert.FromBase64String(cipherText);
            return DecryptStringFromBytes(cipher, Key, IV);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    msDecrypt.Seek(0, SeekOrigin.Begin);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }



    private async void btnGoBackToHomePage_Clicked(object sender, EventArgs e)
    {

        string enteredText = entry1.Text + entry2.Text + entry3.Text + entry4.Text + entry5.Text + entry6.Text + entry7.Text + entry8.Text + entry9.Text;

        MyKey = enteredText;
        

        using (HttpClient client = new HttpClient())
        {
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            try
            {
                string json = client.GetStringAsync(fullURL).Result;
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
                    DisplayAlert("Error", "Licența nu există.", "OK");
                }
                else
                {
                    throw;
                }
            }
        }

        await CheckAndNavigateToHomePage();
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
            
            btnGoBackToHomePage_Clicked(sender, e);
        }
    }


}
