namespace DuckDisk.utils
{
    public class FormatBytes
    {
        public static string Format(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            decimal d = bytes;
            
            while (d >= 1024 && i < suffix.Length - 1)
            {
                d /= 1024;
                i++;
            }
            
            return String.Format("{0:0.##} {1}", d, suffix[i]);
        }
    }
}