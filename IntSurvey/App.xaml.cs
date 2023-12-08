namespace IntSurvey
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new SecondPage());
            Current.UserAppTheme = AppTheme.Light;
            RequestedThemeChanged += (s, e) => {Current.UserAppTheme = AppTheme.Light; };
        }
    }
}