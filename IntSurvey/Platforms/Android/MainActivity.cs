using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls;
using Android.Views;

namespace IntSurvey
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private int backButtonPressCount = 0;
        private const int MaxBackButtonPressCount = 3; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
        }

        public override void OnBackPressed()
        {
            if (App.Current.MainPage is NavigationPage navigationPage &&
                navigationPage.CurrentPage is ContentPage currentPage &&
                currentPage is HomePage)
            {
                backButtonPressCount++;

                if (backButtonPressCount >= MaxBackButtonPressCount)
                {
                    FinishAffinity();
                }
                else
                {
                    Toast.MakeText(this, $"Apasă butonul de {MaxBackButtonPressCount - backButtonPressCount} ori pentru a ieși.", ToastLength.Short).Show();
                }
            }
            else
            {
                backButtonPressCount = 0;

                base.OnBackPressed();
            }
        }
    }
}