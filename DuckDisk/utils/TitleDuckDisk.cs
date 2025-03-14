using Figgle;

namespace DuckDisk.utils;

    class DuckDiskTitle
    {
        
        public static void DisplayTitle()
        {
            string menuTitle = FiggleFonts.Doom.Render("Duck Disk");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(menuTitle);
            Console.ResetColor();
        }
    }
