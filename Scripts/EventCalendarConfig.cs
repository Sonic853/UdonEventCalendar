
using Sonic853.Udon.UrlLoader;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class EventCalendarConfig : UdonSharpBehaviour
    {
        [SerializeField] VRCUrl eventCalendarDataUrl;
        [SerializeField] VRCUrl eventCalendarImageUrl;
        [SerializeField] VRCUrl eventCalendarDataAltUrl;
        [SerializeField] VRCUrl eventCalendarImageAltUrl;
        [SerializeField] UrlSubmitter urlSubmitterData;
        [SerializeField] UrlSubmitter urlSubmitterImage;
        void Start()
        {
            var currentLanguage = VRCPlayerApi.GetCurrentLanguage() ?? "en";
            if (urlSubmitterData != null)
            {
                var url = eventCalendarDataUrl;
                var altUrl = eventCalendarDataAltUrl;
                if (currentLanguage == "zh-CN" && !string.IsNullOrWhiteSpace(altUrl.ToString()))
                {
                    url = eventCalendarDataAltUrl;
                    altUrl = eventCalendarDataUrl;
                }
                if (!string.IsNullOrWhiteSpace(url.ToString()))
                    urlSubmitterData.url = url;
                if (!string.IsNullOrWhiteSpace(altUrl.ToString()) && altUrl != url)
                    urlSubmitterData.altUrl = altUrl;
                urlSubmitterData.SubmitUrl();
            }
            if (urlSubmitterImage != null)
            {
                var url = eventCalendarImageUrl;
                var altUrl = eventCalendarImageAltUrl;
                if (currentLanguage == "zh-CN" && !string.IsNullOrWhiteSpace(altUrl.ToString()))
                {
                    url = eventCalendarImageAltUrl;
                    altUrl = eventCalendarImageUrl;
                }
                if (!string.IsNullOrWhiteSpace(url.ToString()))
                    urlSubmitterImage.url = url;
                if (!string.IsNullOrWhiteSpace(altUrl.ToString()) && altUrl != url)
                    urlSubmitterImage.altUrl = altUrl;
                urlSubmitterImage.SubmitUrlWithUpdate();
            }
        }
    }
}
