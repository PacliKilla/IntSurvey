using Android.Content;
using Google.Android.Material.Tabs;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntSurvey.Platforms.Android
{
    public class CustomShellRenderer : ShellRenderer
    {

        public CustomShellRenderer(Context context) : base(context)
        {

        }

        protected override IShellTabLayoutAppearanceTracker CreateTabLayoutAppearanceTracker(ShellSection shellSection)
        {
            return new MyTabLayoutAppearanceTracker(this);
        }
    }

    public class MyTabLayoutAppearanceTracker : ShellTabLayoutAppearanceTracker
    {
        public MyTabLayoutAppearanceTracker(IShellContext shellContext) : base(shellContext)
        {
        }
        public override void SetAppearance(TabLayout tabLayout, ShellAppearance appearance)
        {
            base.SetAppearance(tabLayout, appearance);

            tabLayout.TabMode = TabLayout.ModeFixed;
            tabLayout.TabGravity = TabLayout.GravityFill;
        }
    }
}
