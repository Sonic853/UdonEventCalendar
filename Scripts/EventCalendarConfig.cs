
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
        [SerializeField] UrlSubmitter urlSubmitterData;
        [SerializeField] UrlSubmitter urlSubmitterImage;
        void Start()
        {
            if (urlSubmitterData != null)
            {
                if (!string.IsNullOrWhiteSpace(eventCalendarDataUrl.ToString()))
                    urlSubmitterData.url = eventCalendarDataUrl;
                urlSubmitterData.SubmitUrl();
            }
            if (urlSubmitterImage != null)
            {
                if (!string.IsNullOrWhiteSpace(eventCalendarImageUrl.ToString()))
                    urlSubmitterImage.url = eventCalendarImageUrl;
                urlSubmitterImage.SubmitUrlWithUpdate();
            }
        }
    }
}
