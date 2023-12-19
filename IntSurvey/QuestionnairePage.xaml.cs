using IntSurvey.QuestionModels;
using Microsoft.Maui.Layouts;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Maui.Controls;
using Plugin.Toast;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Threading;
using System.Diagnostics;

namespace IntSurvey
{
    public partial class QuestionnairePage : ContentPage
    {
        private List<Question> questions;
        private int questionnaireOid;
        private int companyOid;
        private RadioButton trueRadioButton;
        private RadioButton falseRadioButton;
        Picker tenPointScorePicker;
        StackLayout answerStackLayout;
        private string selectedAnswer;
        private HashSet<string> multipleAnswers = new HashSet<string>();
        private bool selectedAnswerForType1;
        private Dictionary<string, string> selectedAnswersForType3 = new Dictionary<string, string>();
        private Dictionary<string, HashSet<string>> selectedAnswersForType4 = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, string> selectedAnswersForType2 = new Dictionary<string, string>();
        private int currentQuestionIndex = 0;
        private QuestionnairePage _page;
        private Dictionary<string, object> selectedAnswers = new Dictionary<string, object>();
        private Button selectedButton;
        private StackLayout lastRadioButtonStackLayout;
        private StackLayout lastTenPointScoreStackLayout;
        private StackLayout lastSingleAnswerVariantStackLayout;
        private StackLayout lastMultipleAnswerVariantStackLayout;



        protected override bool OnBackButtonPressed()
        {
            if (currentQuestionIndex > 0)
            {

                currentQuestionIndex--;
                UpdateQuestionView();
                return true;
            }
            else
            {

                return base.OnBackButtonPressed();
            }
        }
        private void UpdateQuestionView()
        {

            stackLayout.Children.Clear();

            var counterLabel = new Label
            {
                FontSize = 32,
                TextColor = Color.FromHex("#000000"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(10, 10, 10, 0),
            };

            counterLabel.Text = $"{currentQuestionIndex + 1}/{questions.Count}";
            stackLayout.Children.Add(counterLabel);

            var contentContainer = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            stackLayout.Children.Add(new ContentView
            {
                Content = CreateQuestionView(questions[currentQuestionIndex]),
                Padding = new Thickness(10)
            });

            stackLayout.Children.Add(contentContainer);

            Button submitButton = new Button
            {
                Text = "Înainte",
                FontSize = 25,
                TextColor = Color.FromHex("#FFFFFF"),
                BackgroundColor = Color.FromHex("#37AA0F"),
                Margin = new Thickness(10, 10, 10, 45),
                BorderColor = Color.FromHex("#000000"),
                CornerRadius = 5,
                WidthRequest = App.Current.MainPage.Width / 3,
                HeightRequest = 65,
                VerticalOptions = LayoutOptions.End,

            };

            submitButton.Clicked += OnSubmitButtonClicked;
            stackLayout.Children.Add(submitButton);

            if (selectedAnswers.ContainsKey(questions[currentQuestionIndex].question))
            {
                var selectedAnswer = selectedAnswers[questions[currentQuestionIndex].question];

                switch (questions[currentQuestionIndex].gradingType)
                {
                    case 1:
                        if (selectedAnswer != null && selectedAnswer is bool answerForType1)
                        {
                            selectedAnswerForType1 = answerForType1;

                            if (lastRadioButtonStackLayout != null)
                            {
                                
                                foreach (var child in lastRadioButtonStackLayout.Children)
                                {
                                    if (child is ImageRadioButton radioButton && radioButton.Answer == selectedAnswerForType1)
                                    {
                                        
                                        radioButton.RestoreSelectionState(selectedAnswerForType1);

                                        
                                        if (!selectedAnswerForType1 && child is ImageRadioButton falseRadioButton)
                                        {
                                            falseRadioButton.SimulateTap();
                                        }
                                    }
                                }
                            }
                        }
                        break;







                    case 2:
                        if (selectedAnswer != null && selectedAnswer is string answerForType2)
                        {

                            selectedAnswersForType2[questions[currentQuestionIndex].question] = answerForType2;


                            if (lastTenPointScoreStackLayout != null)
                            {
                                foreach (var child in lastTenPointScoreStackLayout.Children)
                                {
                                    if (child is Button button && button.Text == answerForType2)
                                    {
                                        
                                        if (selectedButton != null && selectedButton != button)
                                        {
                                            selectedButton.FontSize = 30;
                                            ResetButtonStyle(selectedButton);
                                        }

                                        button.FontSize = 45;
                                        selectedButton = button;

                                        
                                        SetSelectedButtonStyle(button);
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            button.SendClicked();
                                        });
                                        return;
                                    }
                                }
                            }
                        }
                        break;

                    case 3:
                        if (selectedAnswer != null && selectedAnswer is string answerForType3)
                        {
                            selectedAnswersForType3[questions[currentQuestionIndex].question] = answerForType3;

                            if (lastSingleAnswerVariantStackLayout != null)
                            {
                                foreach (var child in lastSingleAnswerVariantStackLayout.Children)
                                {
                                    if (child is StackLayout rowLayout)
                                    {
                                        foreach (var element in rowLayout.Children)
                                        {
                                            if (element is StackLayout checkboxLayout)
                                            {
                                                var answerCheckBox = checkboxLayout.Children.OfType<CheckBox>().FirstOrDefault();
                                                var answerLabel = checkboxLayout.Children.OfType<Label>().FirstOrDefault();

                                                // Check if the answerVariant matches the selected answer
                                                var answerVariant = questions[currentQuestionIndex].responseVariants
                                                    .FirstOrDefault(av => av.response == answerLabel?.Text);

                                                if (answerCheckBox != null && answerLabel != null && answerVariant != null)
                                                {
                                                    Debug.WriteLine($"answerForType3: {answerForType3}, answerVariant.id.ToString(): {answerVariant.id.ToString()}");

                                                    if (answerForType3 == answerVariant.id.ToString())
                                                    {
                                                        Device.BeginInvokeOnMainThread(() =>
                                                        {
                                                            answerCheckBox.IsChecked = true;
                                                            Debug.WriteLine("CheckBox set to true");
                                                        });
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;





                    case 4:
                        if (selectedAnswer != null && selectedAnswer is HashSet<string> answerForType4)
                        {
                            selectedAnswersForType4[questions[currentQuestionIndex].question] = answerForType4;

                            if (lastMultipleAnswerVariantStackLayout != null)
                            {
                                foreach (var child in lastMultipleAnswerVariantStackLayout.Children)
                                {
                                    if (child is StackLayout rowLayout)
                                    {
                                        foreach (var element in rowLayout.Children)
                                        {
                                            if (element is StackLayout checkboxLayout)
                                            {
                                                foreach (var innerElement in checkboxLayout.Children)
                                                {
                                                    if (innerElement is CheckBox answerCheckBox)
                                                    {
                                                        
                                                        var answerLabel = checkboxLayout.Children.OfType<Label>().FirstOrDefault();

                                                        
                                                        var answerVariant = questions[currentQuestionIndex].responseVariants
                                                            .FirstOrDefault(av => av.response == answerLabel.Text);

                                                        if (answerLabel != null && answerVariant != null &&
                                                            answerForType4.Contains(answerVariant.id.ToString()))
                                                        {
                                                            answerCheckBox.IsChecked = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;





                }
            }

        }

        public QuestionnairePage(List<Question> questions, int questionnaireOid, int companyOid)
        {
            this.questions = questions;
            this.questionnaireOid = questionnaireOid;
            this.companyOid = companyOid;
            _page = this;

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var counterLabel = new Label
            {
                FontSize = 32,
                TextColor = Color.FromHex("#000000"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(10, 10, 10, 10),
            };


            counterLabel.Text = $"{currentQuestionIndex + 1}/{questions.Count}";
            stackLayout.Children.Add(counterLabel);

            var carouselView = new CarouselView
            {
                IsSwipeEnabled = false,
                ItemsSource = questions,
                ItemTemplate = new DataTemplate(() =>
                {
                    var questionContent = new ContentView
                    {
                        Content = CreateQuestionView(questions[currentQuestionIndex]),
                        Padding = new Thickness(10)
                    };
                    return questionContent;
                })
            };

            stackLayout.Children.Add(carouselView);

            var contentContainer = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            stackLayout.Children.Add(contentContainer);

            Button submitButton = new Button
            {
                Text = "Înainte",
                FontSize = 25,
                TextColor = Color.FromHex("#FFFFFF"),
                BackgroundColor = Color.FromHex("#37AA0F"),
                Margin = new Thickness(10, 10, 10, 45),
                BorderColor = Color.FromHex("#000000"),
                CornerRadius = 5,
                WidthRequest = App.Current.MainPage.Width / 3,
                HeightRequest = 65,
                VerticalOptions = LayoutOptions.End,

            };

            submitButton.Clicked += OnSubmitButtonClicked;

            stackLayout.Children.Add(submitButton);


        }
        private void OnBackButtonClicked(object sender, EventArgs e)
        {

            OnBackButtonPressed();
        }

        private View CreateQuestionView(Question question)
        {
            Frame questionFrame = new Frame
            {
                BackgroundColor = Color.FromRgba(255, 255, 240, 0),
                Margin = new Thickness(7, 10, 7, 10),

            };

            Label questionLabel = new Label
            {
                FormattedText = new FormattedString
                {
                    Spans =
                {
                    new Span { Text = question.question, FontAttributes = FontAttributes.Bold, FontSize = 48,FontFamily="Roboto"},
                    new Span { Text = Environment.NewLine },
                    new Span { Text = "(" + question.comentary + ")" , FontSize = 32,FontFamily="Roboto"},
                    new Span { Text = Environment.NewLine }
                }
                },
                TextColor = Color.FromHex("#000000"),
                FontSize = 16
            };

            questionFrame.Content = questionLabel;

            View answerView = null;

            if (question.gradingType == 1)
            {
                answerView = CreateRadioButtonAnswerView(question);
            }
            else if (question.gradingType == 2)
            {
                answerView = CreateTenPointScoreAnswerView(question);
            }
            else if (question.gradingType == 3)
            {
                answerView = CreateSingleAnswerVariantAnswerView(question);
            }
            else if (question.gradingType == 4)
            {
                answerView = CreateMultipleAnswerVariantAnswerView(question);
            }

            return new StackLayout
            {
                Children = { questionFrame, answerView }
            };
        }
        private StackLayout CreateRadioButtonAnswerView(Question question)
        {
            answerStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10 
            };

            var trueRadioButton = new ImageRadioButton(this, true, "Assets/true_clicked1.png", "Assets/true_img1.png");
            var falseRadioButton = new ImageRadioButton(this, false, "Assets/false_clicked1.png", "Assets/false_img1.png");


            answerStackLayout.Children.Add(trueRadioButton);
            answerStackLayout.Children.Add(falseRadioButton);

            lastRadioButtonStackLayout = answerStackLayout;
            return answerStackLayout;
        }

        private StackLayout CreateTenPointScoreAnswerView(Question question)
        {
            StackLayout answerStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(32, 10, 32, 32),
            };

            Button selectedButton = null;

            double buttonWidth = App.Current.MainPage.Width / 11.6;
            double buttonHeight = buttonWidth * 1;

            for (int i = 1; i <= 10; i++)
            {
                var numberButton = new Button
                {
                    BackgroundColor = Color.FromRgba(0, 0, 0, 0),
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 36,
                    WidthRequest = buttonWidth,
                    HeightRequest = buttonHeight,
                    CornerRadius = (int)(buttonWidth / 2),
                };

                
                if (i <= 6)
                {
                    numberButton.Text = i.ToString();
                    numberButton.BackgroundColor = Color.FromHex("#FC644D"); 
                }
                else if (i <= 8)
                {
                    numberButton.Text = (i).ToString();
                    numberButton.BackgroundColor = Color.FromHex("#FEC830"); 
                }
                else
                {
                    numberButton.Text = (i).ToString();
                    numberButton.BackgroundColor = Color.FromHex("#37AA0F"); 
                }

                numberButton.Clicked += (s, e) =>
                {
                    var button = (Button)s;
                    int selectedScore = int.Parse(button.Text);
                    if (selectedButton != null)
                    {
                        selectedButton.FontSize = 36;
                        
                        ResetButtonStyle(selectedButton);
                    }

                    button.FontSize = 36;
                    selectedButton = button;

                    selectedAnswersForType2[question.question] = button.Text;

                    SetSelectedButtonStyle(selectedButton);
                };


                answerStackLayout.Children.Add(numberButton);
            }

            lastTenPointScoreStackLayout = answerStackLayout;
            return answerStackLayout;
        }



        void ResetButtonStyle(Button button)
        {
            int selectedNumber = int.Parse(button.Text);

            
            if (selectedNumber <= 6)
            {
                button.BackgroundColor = Color.FromHex("#FC644D"); 
                button.TextColor = Color.FromHex("#FFFFFF");
            }
            else if (selectedNumber <= 8)
            {
                button.BackgroundColor = Color.FromHex("#FEC830"); 
                button.TextColor = Color.FromHex("#FFFFFF");
            }
            else
            {
                button.BackgroundColor = Color.FromHex("#37AA0F"); 
                button.TextColor = Color.FromHex("#FFFFFF");
            }
        }

        
        void SetSelectedButtonStyle(Button button)
        {
            int selectedNumber = int.Parse(button.Text);

            
            if (selectedNumber <= 6)
            {
                button.BackgroundColor = Color.FromRgba(0, 0, 0, 0); 
                button.TextColor = Color.FromHex("#FC644D");
                button.BorderWidth = 4;
                button.BorderColor = Color.FromHex("#FC644D");
            }
            else if (selectedNumber <= 8)
            {
                button.BackgroundColor = Color.FromRgba(0, 0, 0, 0); 
                button.TextColor = Color.FromHex("#FEC830");
                button.BorderWidth = 4;
                button.BorderColor = Color.FromHex("#FEC830");
            }
            else
            {
                button.BackgroundColor = Color.FromRgba(0, 0, 0, 0); 
                button.TextColor = Color.FromHex("#37AA0F");
                button.BorderWidth = 4;
                button.BorderColor = Color.FromHex("#37AA0F");
            }
        }


        private StackLayout CreateSingleAnswerVariantAnswerView(Question question)
        {
            StackLayout answerStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 10,
                Padding = new Thickness(30, 10, 0, 0),
            };

            int col = 0;
            int row = 0;

            StackLayout currentRowStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
               

            };

            answerStackLayout.Children.Add(currentRowStackLayout);

            foreach (var answerVariant in question.responseVariants)
            {
                var answerCheckBox = new CheckBox
                {
                    Color = Color.FromHex("#37AA0F"),
                    Scale = 1.8,
                };

                var answerLabel = new Label
                {
                    Text = answerVariant.response,
                    FontSize = 32,
                    TextColor = Color.FromHex("#000000"),
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(0, 0, 20, 0),
                };

                answerCheckBox.CheckedChanged += (s, e) =>
                {
                    if (answerCheckBox.IsChecked)
                    {
                        foreach (var child in answerStackLayout.Children)
                        {
                            if (child is StackLayout rowLayout)
                            {
                                foreach (var element in rowLayout.Children)
                                {
                                    if (element is StackLayout checkboxLayout)
                                    {
                                        foreach (var innerElement in checkboxLayout.Children)
                                        {
                                            if (innerElement is CheckBox otherCheckBox && otherCheckBox != answerCheckBox)
                                            {
                                                otherCheckBox.IsChecked = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        selectedAnswersForType3[question.question] = answerVariant.id.ToString();

                    }
                    else
                    {
                        selectedAnswersForType3.Remove(question.question);
                        
                    }
                };

                double labelWidthPercentage = 0.3;
                double labelWidth = DeviceDisplay.MainDisplayInfo.Width * labelWidthPercentage;


                answerLabel.WidthRequest = labelWidth;

                if (col % 2 == 0 && col > 0)
                {
                    currentRowStackLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 10,
                        Padding = new Thickness(0, 0, 0, 0),
                    };

                    answerStackLayout.Children.Add(currentRowStackLayout);
                    row++;
                }

                currentRowStackLayout.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 5,
                    Children = { answerCheckBox, answerLabel },
                    Padding = new Thickness(0, 0, 0, 50),
                });

                col++;
            }

            lastSingleAnswerVariantStackLayout = answerStackLayout;
            return answerStackLayout;
        }




        private StackLayout CreateMultipleAnswerVariantAnswerView(Question question)
        {
            HashSet<string> localMultipleAnswers = new HashSet<string>();

            StackLayout answerStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 10,
                Padding = new Thickness(30, 10, 0, 0),
            };

            int col = 0;
            int row = 0;

            StackLayout currentRowStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,

            };

            answerStackLayout.Children.Add(currentRowStackLayout);

            foreach (var answerVariant in question.responseVariants)
            {
                var answerCheckBox = new CheckBox
                {
                    Color = Color.FromHex("#37AA0F"),
                    Scale = 1.8,
                    VerticalOptions = LayoutOptions.Start
                };

                var answerLabel = new Label
                {
                    Text = answerVariant.response,
                    FontSize = 32,
                    TextColor = Color.FromHex("#000000"),
                    VerticalOptions = LayoutOptions.Center,
                };

                answerCheckBox.CheckedChanged += (s, e) =>
                {
                    if (answerCheckBox.IsChecked)
                    {
                        localMultipleAnswers.Add(answerVariant.id.ToString());

                    }
                    else
                    {
                        localMultipleAnswers.Remove(answerVariant.id.ToString());

                    }

                    selectedAnswersForType4[question.question] = new HashSet<string>(localMultipleAnswers);
                };

                double labelWidthPercentage = 0.8;
                double labelWidth = DeviceDisplay.MainDisplayInfo.Width * labelWidthPercentage;


                answerLabel.WidthRequest = labelWidth;

                if (col % 1 == 0 && col > 0)
                {
                    currentRowStackLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 10
                    };

                    answerStackLayout.Children.Add(currentRowStackLayout);
                    row++;
                }

                currentRowStackLayout.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 5,
                    Children = { answerCheckBox, answerLabel }
                });

                col++;
            }

            lastMultipleAnswerVariantStackLayout = answerStackLayout;
            return answerStackLayout;
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var responses = GetResponseData(answerStackLayout);

            SaveSelectedAnswers();

            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                stackLayout.Children.Clear();
                var counterLabel = new Label
                {
                    FontSize = 32,
                    TextColor = Color.FromHex("#000000"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(10, 10, 10, 10),
                };

                // Update the counter label text for the current question index
                counterLabel.Text = $"{currentQuestionIndex + 1}/{questions.Count}";
                stackLayout.Children.Add(counterLabel);

                stackLayout.Children.Add(new ContentView
                {
                    Content = CreateQuestionView(questions[currentQuestionIndex]),
                    Padding = new Thickness(10)
                });

                var contentContainer = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                stackLayout.Children.Add(contentContainer);

                Button submitButton = new Button
                {
                    Text = "Înainte",
                    FontSize = 25,
                    TextColor = Color.FromHex("#FFFFFF"),
                    BackgroundColor = Color.FromHex("#37AA0F"),
                    Margin = new Thickness(10, 10, 10, 45),
                    BorderColor = Color.FromHex("#000000"),
                    CornerRadius = 5,
                    WidthRequest = App.Current.MainPage.Width / 3,
                    HeightRequest = 65,

                };

                submitButton.Clicked += OnSubmitButtonClicked;

                stackLayout.Children.Add(submitButton);


            }
            else
            {
                if (responses == null || responses.Any(response => response.responseVariantId == null))
                {

                    await DisplayAlert("Alertă", "Vă rugăm să răspundeți la toate întrebările", "OK");
                    currentQuestionIndex = 0;
                    UpdateQuestionView();
                    return;
                }
                await FinalSubmissionLogic(sender, e);
            }
        }
        private void SaveSelectedAnswers()
        {
            if (questions != null && currentQuestionIndex < questions.Count)
            {
                var currentQuestion = questions[currentQuestionIndex];
                object answerToSave = null;

                switch (currentQuestion.gradingType)
                {
                    case 1:
                        answerToSave = selectedAnswerForType1;
                        break;
                    case 2:
                        answerToSave = selectedAnswersForType2.ContainsKey(currentQuestion.question) ? selectedAnswersForType2[currentQuestion.question] : null;
                        break;
                    case 3:
                        answerToSave = selectedAnswersForType3.ContainsKey(currentQuestion.question) ? selectedAnswersForType3[currentQuestion.question] : null;
                        break;
                    case 4:
                        answerToSave = selectedAnswersForType4.ContainsKey(currentQuestion.question) ? selectedAnswersForType4[currentQuestion.question] : null;
                        break;
                }

                if (answerToSave != null)
                {
                    // Use the question text as the key
                    selectedAnswers[currentQuestion.question] = answerToSave;
                }
            }
        }






        private bool unansweredQuestionAlertDisplayed = false;
        private async Task FinalSubmissionLogic(object sender, EventArgs e)
        {
            var licenseId = await SecureStorage.GetAsync("LicenseID");

            var response = new ResponseData
            {
                oid = 0,
                questionnaireId = questionnaireOid,
                responses = GetResponseData(answerStackLayout),
                companyOid = companyOid,
                licenseId = licenseId
            };

            //if (response.responses == null || !response.responses.Any())
            //{
            //    unansweredQuestionAlertDisplayed = true;
            //    return;
            // }

            var json = JsonConvert.SerializeObject(response);

            if (IsInternetConnected())
            {
                using (HttpClient client = new HttpClient())
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("uSr_nps:V8-}W31S!l'D"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var responseMessage = await client.PostAsync("https://dev.edi.md/ISNPSAPI/Mobile/InsertResponsesNEW", content);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                        // Save the JSON content to a text file
                        //string fileName = "response_data.txt";
                        //string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                        //using (var writer = File.CreateText(filePath))
                        // {
                       // await writer.WriteAsync(json);
                        //}
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        var successMessage = $"Răspunsul a fost trimis cu succes." + Environment.NewLine + "Mulțumim pentru opinia dumneavoastră.";
                        //var successMessage = $"Responses submitted successfully.\n\nRequest content:\n{json}\n\nResponse content:\n{jsonResponse}";
                        //await DisplayAlert("Success", successMessage, "OK");

                        var toast = Toast.Make(successMessage, duration: ToastDuration.Long);

                        await toast.Show(cancellationTokenSource.Token);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to submit responses", "OK");
                    }

                    await Navigation.PopAsync();
                }
            }
            else
            {
                SaveResponsesToCache(response);
            }
            if (!unansweredQuestionAlertDisplayed)
            {
                await Navigation.PopAsync();
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
        private async void SaveResponsesToCache(ResponseData response)
        {
            try
            {
                var json = JsonConvert.SerializeObject(response);
                await SecureStorage.SetAsync("cached_response", json);
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving responses to cache: {ex.Message}");
            }
        }

        List<Response> GetResponseData(StackLayout answerStackLayout)
        {
            var responses = new List<Response>();

            foreach (var question in questions)
            {
                var questionId = question.id;  

                switch (question.gradingType)
                {
                    case 1:


                        responses.Add(new Response
                        {
                            id = 0,
                            questionId = questionId,
                            
                            alternativeResponse = _page.selectedAnswerForType1 ? true.ToString() : false.ToString(),
                            comentary = string.Empty
                        });
                        break;

                    case 2:
                        
                        if (IsTenPointScoreAnswered(question.question))
                        {
                            var responseVariantId = int.Parse(selectedAnswersForType2[question.question]); 
                            responses.Add(new Response
                            {
                                id = 0,
                                questionId = questionId,
                                
                                alternativeResponse = selectedAnswersForType2[question.question],
                                comentary = string.Empty
                            });
                        }
                        break;

                    case 3:
                        
                        if (IsSingleAnswerVariantAnswered(question.question))
                        {
                            var responseVariantId = int.Parse(selectedAnswersForType3[question.question]); 
                            responses.Add(new Response
                            {
                                id = 0,
                                questionId = questionId,
                                responseVariantId = responseVariantId,
                                
                                comentary = string.Empty
                            });
                        }
                        break;

                    case 4:
                        
                        if (IsMultipleAnswerVariantAnswered(question.question))
                        {
                            var selectedVariants = selectedAnswersForType4[question.question];
                            foreach (var variant in selectedVariants)
                            {
                                var responseVariantId = int.Parse(variant); 
                                responses.Add(new Response
                                {
                                    id = 0,
                                    questionId = questionId,
                                    responseVariantId = responseVariantId,
                                    
                                    comentary = string.Empty
                                });
                            }
                        }
                        break;

                    default:
                        
                        break;
                }
            }

            return responses;
        }

        List<string> GetSelectedAnswers(Question question, StackLayout answerStackLayout, ref string selectedAnswer, HashSet<string> multipleAnswers)
        {
            var answers = new List<string>();

            switch (question.gradingType)
            {
                case 1:


                    answers.Add(selectedAnswerForType1.ToString());
                    break;


                case 2:
                    if (!IsTenPointScoreAnswered(question.question))
                    {
                        DisplayUnansweredQuestionAlert();
                        return null;
                    }

                    answers.Add(selectedAnswersForType2[question.question]);
                    break;


                case 3:
                    if (!IsSingleAnswerVariantAnswered(question.question))
                    {
                        DisplayUnansweredQuestionAlert();
                        return null;
                    }

                    answers.Add(selectedAnswersForType3[question.question]);
                    break;


                case 4:
                    if (!IsMultipleAnswerVariantAnswered(question.question))
                    {
                        DisplayUnansweredQuestionAlert();
                        return null;
                    }

                    answers.AddRange(selectedAnswersForType4[question.question]);
                    break;


                default:

                    break;
            }

            return answers;
        }
        private void DisplayUnansweredQuestionAlert()
        {
            //Device.BeginInvokeOnMainThread(async () =>
            // {
            //     await DisplayAlert("Alertă", "Vă rugăm să răspundeți la toate întrebările înainte de a trimite răspunsul.", "OK");
            // });
        }
        private bool IsRadioButtonAnswered()
        {
            return _page.selectedAnswerForType1;
        }



        private bool IsTenPointScoreAnswered(string questionKey)
        {
            return selectedAnswersForType2.ContainsKey(questionKey);
        }


        private bool IsSingleAnswerVariantAnswered(string questionKey)
        {
            return selectedAnswersForType3.ContainsKey(questionKey);
        }

        private bool IsMultipleAnswerVariantAnswered(string questionKey)
        {
            return selectedAnswersForType4.ContainsKey(questionKey) && selectedAnswersForType4[questionKey].Count > 0;
        }



        public class ImageRadioButton : ContentView
        {
            private readonly QuestionnairePage _page;
            private readonly bool _answer;
            private readonly Image _image;
            private readonly ImageSource _normalImage;
            private readonly ImageSource _clickedImage;

            public bool Answer { get; private set; }

            private static ImageRadioButton _selectedRadioButton;

            public ImageRadioButton(QuestionnairePage page, bool answer, string normalImageSource, string clickedImageSource)
            {
                _page = page;
                _answer = answer;
                _normalImage = ImageSource.FromFile(normalImageSource);
                _clickedImage = ImageSource.FromFile(clickedImageSource);
                Answer = answer; 

                _image = new Image
                {
                    Source = _normalImage,
                    Aspect = Aspect.AspectFit,
                    HeightRequest = 140,
                    WidthRequest = 140,
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnImageTapped;
                _image.GestureRecognizers.Add(tapGestureRecognizer);

                Content = new StackLayout { Children = { _image } };
            }


            private void OnImageTapped(object sender, EventArgs e)
            {
                if (_selectedRadioButton != null)
                {
                    _selectedRadioButton.RestoreSelectionState(false);
                }

                _selectedRadioButton = this;
                _page.selectedAnswerForType1 = Answer;
                _image.Source = _clickedImage;
            }

            public void SimulateTap()
            {
                if (_selectedRadioButton != null)
                {
                    _selectedRadioButton.RestoreSelectionState(false);
                }

                _selectedRadioButton = this;
                _page.selectedAnswerForType1 = Answer;
                _image.Source = _clickedImage;
            }

            public void RestoreSelectionState(bool isSelected)
            {
                _image.Source = isSelected ? _clickedImage : _normalImage;

                if (isSelected)
                {
                    _image.HeightRequest = 140;
                    _image.WidthRequest = 140;
                    _selectedRadioButton = this;
                }
                else
                {
                    _image.HeightRequest = 140;
                    _image.WidthRequest = 140;


                }
            }

        }





    }
}