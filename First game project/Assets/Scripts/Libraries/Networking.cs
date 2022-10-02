using System.ComponentModel;
using System.Net;


namespace Networking
{
    public static class ShaiNetwork
    {
        public static void DownloadFileAsync(string url, string filePath, 
            DownloadProgressChangedEventHandler[] progressEvent,
            AsyncCompletedEventHandler[] completedEvent)
        {
            WebClient webClient = new WebClient();
                
            if (progressEvent != null)
            {
                for (int i = 0; i < progressEvent.Length; i++)
                    webClient.DownloadProgressChanged += progressEvent[i];
            }

            if (completedEvent != null)
            {
                for (int i = 0; i < completedEvent.Length; i++)
                    webClient.DownloadFileCompleted += completedEvent[i];
            }

            webClient.DownloadFileAsync(new System.Uri(url), filePath);
        }
    }
}
