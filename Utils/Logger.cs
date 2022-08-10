using System.Text;

namespace SoftTurFlights.Utils
{
    public class Logger
    {
        public static string LogPath { get; set; } = "";

        public static void Log(object obj)
        {
            if (obj == null)
            {
                Log("null");
                return;
            }

            var str = new StringBuilder();
            str.Append(obj.GetType().Name + " - ");
            foreach (var it in obj.GetType().GetProperties())
            {
                str.AppendFormat("{0}={1};", it.Name, it.GetValue(obj));
            }

            Log(str.ToString());
        }

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        public static void Log(string str, string fileName = "log")
        {
            try
            {
                var path = LogPath;
                if (string.IsNullOrEmpty(path))
                {
                    path = "C:\\logsApiVoos";
                    Directory.CreateDirectory(path);
                }

                var file = Path.Combine(path, fileName + ".txt");
                var info = new FileInfo(file);
                if (info.Exists && info.Length > (1024 * 100))
                {
                    var rename = string.Format(fileName + "-{0:yyyyMMdd-HHmmss}.txt", DateTime.Now);
                    File.Move(file, Path.Combine(path, rename));
                }

                var format = string.Format("[{0:dd/MM HH:mm:ss}] {1}\n", DateTime.Now, str);
                File.AppendAllText(file, format);
            }
            catch (Exception)
            {
                //TODO: rotina para enviar por email essa exception
            }
        }
    }
}
