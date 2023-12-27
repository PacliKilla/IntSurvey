using System;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IntSurvey.Models;
using IntSurvey.QuestionModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Layouts;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Grid = Microsoft.Maui.Controls.Grid;
using Questionnaire = IntSurvey.QuestionModels.Questionnaire;


namespace IntSurvey
{
    public partial class HomePage : ContentPage
    {
        Dictionary<int, List<Question>> questionnaireQuestions = new Dictionary<int, List<Question>>();
        string cachedID = SecureStorage.GetAsync("LicenseID").Result;
        public string baseLink = "https://dev.edi.md/ISNPSAPI/Mobile/GetQuestionnairesNEW?LicenseID=";
        public string baseOidLink = "https://dev.edi.md/ISNPSAPI/Mobile/GetQuestionnaireNEW?LicenseId=";
        public string licenseIDLink => $"{baseLink}{cachedID}";
        public string OidLink => $"{baseOidLink}{cachedID}";
        string username = "uSr_nps";
        string password = "V8-}W31S!l'D";
        Root questionnaires;
        public string QuestionnaireName { get; set; }

        public HomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);




            BindingContext = this;

            QuestionnaireName = "Chestionare";

            // Set the initial text for the label
            titleLabel.Text = QuestionnaireName;






            if (IsInternetConnected())
            {
                LoadQuestionnairesFromServer();
            }
            else
            {
                LoadQuestionnairesFromCache();
            }



        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (IsInternetConnected())
            {
                LoadCachedResponsesAndSend();
            }
        }

        private bool IsInternetConnected()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                return current == NetworkAccess.Internet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking internet connectivity: {ex.Message}");
                return false;
            }
        }

        private async void LoadQuestionnairesFromServer()
        {
            using (HttpClient client = new HttpClient())
            {
                var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                string json = client.GetStringAsync(licenseIDLink).Result;
                Root questionnaires = JsonConvert.DeserializeObject<Root>(json);

                SaveToCacheAsync("all_questionnaires", JsonConvert.SerializeObject(questionnaires, new JsonSerializerSettings { Formatting = Formatting.None }));

                foreach (var questionnaire in questionnaires.questionnaires)
                {
                    int oid = questionnaire.oid;
                    string linkWithOid = OidLink + "&id=" + oid;
                    string oidjson = client.GetStringAsync(linkWithOid).Result;
                    RootObject oidquestionnaires = JsonConvert.DeserializeObject<RootObject>(oidjson);

                    SaveToCacheAsync($"oidquestionnaires_{questionnaire.oid}", JsonConvert.SerializeObject(oidquestionnaires, new JsonSerializerSettings { Formatting = Formatting.None }));


                    questionnaireQuestions.Add(oid, oidquestionnaires.questionnaire.questions);

                    Label questionnaireLabel = new Label
                    {
                        Text = questionnaire.name,
                        FontSize = 20,
                        TextColor = Color.FromHex("#FFFFFF"),
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(45, 0, 0, 0),
                        FontAttributes = FontAttributes.Bold,
                        InputTransparent = true,
                    };


                    Button questionnaireButton = new Button
                    {
                        Text = questionnaire.name,
                        FontSize = 30,
                        CommandParameter = new QuestionnaireInfo { SelectedOid = oid, CompanyOid = questionnaire.companyOid },
                        Margin = new Thickness(25, 5, 25, 5),
                        BackgroundColor = Color.FromHex("#37AA0F"),
                        TextColor = Color.FromRgba(0, 0, 0, 0),
                        CornerRadius = 5,
                      
                        FontAttributes = FontAttributes.Bold
                    };

                    questionnaireButton.Clicked += OnQuestionnaireButtonClicked;
                    
                    Grid grid = new Grid();
                    grid.Children.Add(questionnaireButton);
                    grid.Children.Add(questionnaireLabel);

                    
                    Grid.SetRow(questionnaireButton, 0);
                    Grid.SetColumn(questionnaireButton, 0);
                    Grid.SetRow(questionnaireLabel, 0);
                    Grid.SetColumn(questionnaireLabel, 0);

                    
                    stackLayout.Children.Add(grid);
                }
            }
        }

        private async Task SaveToCacheAsync(string key, string value)
        {
            try
            {
                await SecureStorage.SetAsync(key, value);
                Console.WriteLine($"Saved to cache: Key={key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to cache: {ex.Message}");
            }
        }


        private async Task<string> LoadJsonFromCacheAsync(string key)
        {
            try
            {
                string result = await SecureStorage.GetAsync(key);
                Console.WriteLine($"Loaded from cache: Key={key}, Value={(result ?? "null")}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from cache: Key={key}, Message={ex.Message}");
                return null;
            }
        }


        private async Task LoadQuestionnairesFromCache()
        {
            try
            {
                string cachedJson = await LoadJsonFromCacheAsync("all_questionnaires");

                if (!string.IsNullOrEmpty(cachedJson))
                {
                    Root cachedQuestionnaires = JsonConvert.DeserializeObject<Root>(cachedJson);

                    if (cachedQuestionnaires != null && cachedQuestionnaires.questionnaires != null)
                    {
                        foreach (var questionnaire in cachedQuestionnaires.questionnaires)
                        {
                            int oid = questionnaire.oid;

                            
                            string cachedOidQuestionnaires = await SecureStorage.GetAsync($"oidquestionnaires_{questionnaire.oid}");


                            if (!string.IsNullOrEmpty(cachedOidQuestionnaires))
                            {
                                RootObject oidquestionnaires = JsonConvert.DeserializeObject<RootObject>(cachedOidQuestionnaires);
                                questionnaireQuestions.Add(oid, oidquestionnaires.questionnaire.questions);

                                Label questionnaireLabel = new Label
                                {
                                    Text = questionnaire.name,
                                    FontSize = 20,
                                    TextColor = Color.FromHex("#FFFFFF"),
                                    HorizontalOptions = LayoutOptions.Start,
                                    VerticalOptions = LayoutOptions.Center,
                                    Padding = new Thickness(45, 0, 0, 0),
                                    FontAttributes = FontAttributes.Bold,
                                    InputTransparent = true,
                                };


                                Button questionnaireButton = new Button
                                {
                                    Text = questionnaire.name,
                                    FontSize = 30,
                                    CommandParameter = new QuestionnaireInfo { SelectedOid = oid, CompanyOid = questionnaire.companyOid },
                                    Margin = new Thickness(25, 5, 25, 5),
                                    BackgroundColor = Color.FromHex("#37AA0F"),
                                    TextColor = Color.FromRgba(0, 0, 0, 0),
                                    CornerRadius = 5,

                                    FontAttributes = FontAttributes.Bold
                                };

                                questionnaireButton.Clicked += OnQuestionnaireButtonClicked;

                                Grid grid = new Grid();
                                grid.Children.Add(questionnaireButton);
                                grid.Children.Add(questionnaireLabel);


                                Grid.SetRow(questionnaireButton, 0);
                                Grid.SetColumn(questionnaireButton, 0);
                                Grid.SetRow(questionnaireLabel, 0);
                                Grid.SetColumn(questionnaireLabel, 0);


                                stackLayout.Children.Add(grid);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading questionnaires from cache: {ex.Message}");
            }
        }
        private async void LoadCachedResponsesAndSend()
        {
            try
            {
                var cachedResponsesJson = await SecureStorage.GetAsync("cached_response");

                if (!string.IsNullOrEmpty(cachedResponsesJson))
                {

                    using (HttpClient client = new HttpClient())
                    {
                        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        var json = (cachedResponsesJson);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var responseMessage = await client.PostAsync("https://dev.edi.md/ISNPSAPI/Mobile/InsertResponsesNEW", content);

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            //var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                            //var successMessage = $"Responses submitted successfully.\n\nRequest content:\n{json}\n\nResponse content:\n{jsonResponse}";
                            //await DisplayAlert("Success", successMessage, "OK");
                            
                            SecureStorage.Remove("cached_response");
                            Console.WriteLine("Cached responses sent successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cached responses: {ex.Message}");
            }
        }

        private async void OnQuestionnaireButtonClicked(object sender, EventArgs e)
        {
            var questionnaireInfo = (QuestionnaireInfo)((Button)sender).CommandParameter;
            int selectedOid = questionnaireInfo.SelectedOid;
            int companyOid = questionnaireInfo.CompanyOid;

            try
            {
                // Show the activity indicator
                loadingRow.Height = new GridLength(1, GridUnitType.Star);
                loadingOverlay.IsVisible = true;

                // Load the new page
                List<Question> selectedQuestions = questionnaireQuestions[selectedOid];
                await Navigation.PushAsync(new QuestionnairePage(selectedQuestions, selectedOid, companyOid));
            }
            finally
            {
                // Hide the activity indicator
               
                loadingOverlay.IsVisible = false;
                loadingRow.Height = GridLength.Auto;
            }
        }



        public class QuestionnaireInfo
        {
            public int SelectedOid { get; set; }
            public int CompanyOid { get; set; }
        }

    }

}
