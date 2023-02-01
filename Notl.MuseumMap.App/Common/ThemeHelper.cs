using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.Colors;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Helper for managing themes.
    /// </summary>
    public class ThemeHelper
    {
        readonly static MudTheme lightTheme;
        readonly static MudTheme darkTheme;

        /// <summary>
        /// Static Constructor
        /// </summary>
        static ThemeHelper()
        {
            var typography = new Typography()
            {
                Default = new Default()
                {
                    FontSize = "0.70rem",
                },
                Subtitle1 = new Subtitle1
                {
                    FontSize = "0.6rem",
                },
                Subtitle2 = new Subtitle2
                {
                    FontSize = "0.5rem",
                },
                Caption = new Caption
                {
                    FontSize = "0.6rem",
                },
                H1 = new H1 { FontSize = "1.7rem", FontWeight = 550 },
                H2 = new H2 { FontSize = "1.5rem", FontWeight = 550 },
                H3 = new H3 { FontSize = "1.3rem", FontWeight = 550 },
                H4 = new H4 { FontSize = "1.1rem", FontWeight = 550 },
                H5 = new H5 { FontSize = "1.0rem", FontWeight = 550 },
                H6 = new H6 { FontSize = "0.9rem", FontWeight = 550 },
            };

            var layoutProperties = new LayoutProperties();
            var shadows = new Shadow();
            var paletteDark = new PaletteDark();
            var zIndex = new ZIndex();


            // Create the light theme.
            lightTheme = new MudTheme()
            {
                Palette = new Palette()
                {
                    Primary = "#1566B0",
                    Background = "#f5f5f5",
                    Surface = "#e5e5e5",
                    AppbarBackground = "#1566B0",
                    AppbarText = "#FFFFFF",
                    Dark = "#FFFFFF",
                },
                Typography = typography,
                LayoutProperties = layoutProperties,
                Shadows = shadows,
                PaletteDark = paletteDark,
                ZIndex = zIndex,
            };

            // Create the dark theme.
            darkTheme = new MudTheme()
            {
                Palette = new Palette()
                {
                    Dark = "#FFFFFF",
                    Primary = "#1566B0",
                    Black = "#27272f",
                    Background = "#303030",
                    BackgroundGrey = "#27272f",
                    Surface = "#424242",
                    DrawerBackground = "#27272f",
                    DrawerText = "rgba(255,255,255, 0.50)",
                    DrawerIcon = "rgba(255,255,255, 0.50)",
                    AppbarBackground = "#27272f",
                    AppbarText = "#FFFFFF",
                    TextPrimary = "#FFFFFF",
                    TextSecondary = "#FFFFFF",
                    ActionDefault = "#ADADB1",
                    ActionDisabled = "rgba(255,255,255, 0.26)",
                    ActionDisabledBackground = "rgba(255,255,255, 0.12)",
                    Divider = "#6F6F6F",
                    DividerLight = "#6F6F6F",
                    TableLines = "#FFFFFF",
                    LinesDefault = "#FFFFFF",
                    LinesInputs = "#FFFFFF",
                    TextDisabled = "rgba(255,255,255, 0.2)",
                },
                Typography = typography,
                LayoutProperties = layoutProperties,
                Shadows = shadows,
                PaletteDark = paletteDark,
                ZIndex = zIndex,
            };

        }

        /// <summary>
        /// Gets the application theme.
        /// </summary>
        /// <param name="appTheme"></param>
        /// <returns></returns>
        public static MudTheme GetTheme(AppTheme appTheme)
        {
            switch(appTheme)
            {
                case AppTheme.Dark:
                    return darkTheme;
                case AppTheme.Light:
                default:
                    return lightTheme;
            }
        }
    }
}
