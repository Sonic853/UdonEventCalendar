
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class EventSource : UdonSharpBehaviour
    {
        [NonSerialized]
        public UdonEventCalendar eventCalendar;
        public string eventName;
        public VRCUrl url;
        public VRCUrl altUrl;
        [NonSerialized]
        public string content;
        [NonSerialized]
        public DataDictionary data;
    }
}
