using Figgle;

namespace DuckDisk.utils;

    class DuckDriveTitle
    {
        
        public static void DisplayTitle()
        {
            string menuTitle = FiggleFonts.Doom.Render("Duck Drive");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(menuTitle);
            Console.ResetColor();
        }
    }
