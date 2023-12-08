using IntSurvey.QuestionModels;
using Microsoft.Maui.Layouts;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Maui.Controls;
using Plugin.Toast;


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
                Margin = new Thickness(10, 10, 10, 10),
            };

            // Update the counter label text for the current question index
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
                BorderWidth = 1,
                CornerRadius = 5,
                Shadow = new Shadow
                {
                    Offset = new Point(1, 1),
                    Radius = 2,
                },
                WidthRequest = App.Current.MainPage.Width / 2,
                HeightRequest = 65,
                VerticalOptions = LayoutOptions.End,
                
            };

            submitButton.Clicked += OnSubmitButtonClicked;
            stackLayout.Children.Add(submitButton);


            
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

            // Update the counter label text for the current question index
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
                BorderWidth = 0.5,
                CornerRadius = 5,
                Shadow = new Shadow
                {
                    Offset = new Point(1, 1),
                    Radius = 2,
                },
                WidthRequest = App.Current.MainPage.Width / 2,
                HeightRequest = 65,
                VerticalOptions = LayoutOptions.End,
                
            };

            submitButton.Clicked += OnSubmitButtonClicked;

            stackLayout.Children.Add(submitButton);

            
        }
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Call the existing OnBackButtonPressed logic
            OnBackButtonPressed();
        }

        private View CreateQuestionView(Question question)
        {
            Frame questionFrame = new Frame
            {
                BackgroundColor = Color.FromRgba(255, 255, 240, 0),
                Margin = new Thickness(25, 20, 10, 10),
                
            };

            Label questionLabel = new Label
            {
                FormattedText = new FormattedString
                {
                    Spans =
                {
                    new Span { Text = question.question, FontAttributes = FontAttributes.Bold, FontSize = 48},
                    new Span { Text = Environment.NewLine },
                    new Span { Text = "(" + question.comentary + ")" , FontSize = 32},
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
            answerStackLayout = new StackLayout();
            answerStackLayout.Orientation = StackOrientation.Horizontal;
            answerStackLayout.HorizontalOptions = LayoutOptions.Center;

            var trueRadioButton = new ImageRadioButton(this, true, "Assets/true_img.png");
            var falseRadioButton = new ImageRadioButton(this, false, "Assets/false_img.png");

            answerStackLayout.Children.Add(trueRadioButton);
            answerStackLayout.Children.Add(falseRadioButton);

            return answerStackLayout;
        }

        private StackLayout CreateTenPointScoreAnswerView(Question question)
        {
            StackLayout answerStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 10, 0, 10),
            };

            Button selectedButton = null;

            double buttonWidth = App.Current.MainPage.Width / 11;
            double buttonHeight = buttonWidth * 1;

            for (int i = 1; i <= 10; i++)
            {
                var numberButton = new Button
                {
                    BackgroundColor = Color.FromRgba(0, 0, 0, 0),
                    TextColor = Color.FromHex("#FFFFFF"),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 30,
                    WidthRequest = buttonWidth,
                    HeightRequest = buttonHeight,
                    CornerRadius = (int)(buttonWidth / 2),
                };

                // Set different background colors and text based on the number
                if (i <= 6)
                {
                    numberButton.Text = i.ToString();
                    numberButton.BackgroundColor = Color.FromHex("#FC644D"); // Red circle
                }
                else if (i <= 8)
                {
                    numberButton.Text = (i).ToString();
                    numberButton.BackgroundColor = Color.FromHex("#FEC830"); // Yellow circle
                }
                else
                {
                    numberButton.Text = (i).ToString();
                    numberButton.BackgroundColor = Color.FromHex("#37AA0F"); // Green circle
                }

                numberButton.Clicked += (s, e) =>
                {
                    var button = (Button)s;

                    if (selectedButton != null)
                    {
                        selectedButton.FontSize = 30;
                    }

                    button.FontSize = 45;
                    selectedButton = button;

                    selectedAnswersForType2[question.question] = button.Text;
                };

                answerStackLayout.Children.Add(numberButton);
            }

            return answerStackLayout;
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

            foreach (var answerVariant in question.answerVariants)
            {
                var answerCheckBox = new CheckBox
                {
                    Color = Color.FromHex("#37AA0F"),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                };

                var answerLabel = new Label
                {
                    Text = answerVariant,
                    FontSize = 32,
                    TextColor = Color.FromHex("#000000"),
                    VerticalOptions = LayoutOptions.Center,
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

                        selectedAnswersForType3[question.question] = answerLabel.Text;
                        EnlargeCheckbox(answerCheckBox);
                    }
                    else
                    {
                        selectedAnswersForType3.Remove(question.question);
                        RestoreCheckboxSize(answerCheckBox);
                    }
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

            return answerStackLayout;
        }
        private void EnlargeCheckbox(CheckBox checkBox)
        {
            checkBox.ScaleTo(1.5, 250); 
        }

        private void RestoreCheckboxSize(CheckBox checkBox)
        {
            checkBox.ScaleTo(1.0, 250); 
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

            foreach (var answerVariant in question.answerVariants)
            {
                var answerCheckBox = new CheckBox
                {
                    Color = Color.FromHex("#37AA0F"),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                };

                var answerLabel = new Label
                {
                    Text = answerVariant,
                    FontSize = 32,
                    TextColor = Color.FromHex("#000000"),
                    VerticalOptions = LayoutOptions.Center,
                };

                answerCheckBox.CheckedChanged += (s, e) =>
                {
                    if (answerCheckBox.IsChecked)
                    {
                        localMultipleAnswers.Add(answerLabel.Text);
                        EnlargeCheckbox(answerCheckBox);
                    }
                    else
                    {
                        localMultipleAnswers.Remove(answerLabel.Text);
                        RestoreCheckboxSize(answerCheckBox);
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

            return answerStackLayout;
        }

        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var responses = GetResponseData(answerStackLayout);

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
                    BorderWidth = 1,
                    CornerRadius = 5,
                    Shadow = new Shadow
                    {
                        Offset = new Point(1, 1),
                        Radius = 2,
                    },
                    WidthRequest = App.Current.MainPage.Width / 2,
                    HeightRequest = 65,
                    
                };

                submitButton.Clicked += OnSubmitButtonClicked;

                stackLayout.Children.Add(submitButton);

               
            }
            else
            {
                if (responses == null || responses.Any(response => response.response == null))
                {
                    
                    await DisplayAlert("Alertă", "Vă rugăm să răspundeți la toate întrebările", "OK");
                    currentQuestionIndex = 0;
                    UpdateQuestionView();
                    return;
                }
                await FinalSubmissionLogic(sender, e);
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
                    var responseMessage = await client.PostAsync("https://dev.edi.md/ISNPSAPI/Mobile/InsertResponses", content);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                        // Save the JSON content to a text file
                        //string fileName = "response_data.txt";
                        //string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                        //using (var writer = File.CreateText(filePath))
                        // {
                        //await writer.WriteAsync(json);
                        // }

                        //var successMessage = $"Răspunsul a fost trimis cu succes." + Environment.NewLine + "Mulțumim pentru opinia dumneavoastră.";
                        var successMessage = $"Responses submitted successfully.\n\nRequest content:\n{json}\n\nResponse content:\n{jsonResponse}";
                        await DisplayAlert("Success", successMessage, "OK");
                        //CrossToastPopUp.Current.ShowToastSuccess(successMessage);
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
                var questionResponse = new Response
                {
                    Question = question.question,
                    response = GetSelectedAnswers(question, answerStackLayout, ref selectedAnswer, multipleAnswers)
                };
               // if (questionResponse.response == null || !questionResponse.response.Any())
               // {
               //     return new List<Response>();
               // }

                responses.Add(questionResponse);
            }

            return responses;
        }

        List<string> GetSelectedAnswers(Question question, StackLayout answerStackLayout, ref string selectedAnswer, HashSet<string> multipleAnswers)
        {
            var answers = new List<string>();

            switch (question.gradingType)
            {
                case 1:
                    if (!IsRadioButtonAnswered())
                    {
                        DisplayUnansweredQuestionAlert();
                        return null;
                    }

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

            private static ImageRadioButton _selectedRadioButton;

            public ImageRadioButton(QuestionnairePage page, bool answer, string imageSource)
            {
                _page = page;
                _answer = answer;

                _image = new Image
                {
                    Source = ImageSource.FromFile(imageSource),
                    Aspect = Aspect.AspectFit,
                    HeightRequest = 200,
                    WidthRequest = 200,
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
                    _selectedRadioButton._image.HeightRequest = 200;
                    _selectedRadioButton._image.WidthRequest = 200;
                }

                _selectedRadioButton = this;

                _page.selectedAnswerForType1 = _answer;

                if (_image.HeightRequest == 200)
                {
                    _image.HeightRequest = 230;
                    _image.WidthRequest = 230;
                }
                else
                {
                    _image.HeightRequest = 200;
                    _image.WidthRequest = 200;
                }
            }
        }
    }
}