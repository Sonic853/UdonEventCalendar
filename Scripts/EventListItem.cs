
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class EventListItem : UdonSharpBehaviour
    {
        [SerializeField] public UdonEventCalendar eventCalendar;
        #region UI
        [SerializeField] Toggle toggleUI;
        public Toggle Toggle => toggleUI;
        [SerializeField] TMP_Text titleUI;
        [SerializeField] TMP_Text descriptionUI;
        [SerializeField] TMP_Text authorUI;
        [SerializeField] TMP_Text timeUI;
        #endregion
        #region Data
        public string ID;
        string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                titleUI.text = _E("title", title);
            }
        }
        string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                descriptionUI.text = _E("description", description);
            }
        }
        public string Author
        {
            get => authorUI.text;
            set => authorUI.text = value;
        }
        DateTime startTime;
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                if (startTime != default && endTime != default)
                {
                    var elitime = _("ELI Time:{0} - {1}");
                    if (elitime.StartsWith("ELI Time:")) elitime = elitime.Substring(9);
                    timeUI.text = string.Format(elitime, startTime.ToString(_("HH:mm")), endTime.ToString(_("HH:mm")));
                }
                else if (startTime != default)
                {
                    timeUI.text = startTime.ToString(_("HH:mm"));
                }
            }
        }
        DateTime endTime;
        public DateTime EndTime
        {
            get => endTime;
            set
            {
                endTime = value;
                if (startTime != default && endTime != default)
                {
                    timeUI.text = string.Format(_("ELI Time:{0} - {1}"), startTime.ToString(_("HH:mm")), endTime.ToString(_("HH:mm")));
                }
                else if (startTime != default)
                {
                    timeUI.text = startTime.ToString(_("HH:mm"));
                }
            }
        }
        public string Location;
        public string InstanceType;
        public string[] Platform;
        public string[] Tags;
        public string GroupName;
        public string GroupID;
        public string Require;
        public string Join;
        public string Note;
        public string Language;
        public DataDictionary multilinguals = new DataDictionary();
        #endregion
        public void EventSelected() => eventCalendar.EventSelected();
        #region 翻译
        public string _(string text) => eventCalendar.Translate.GetText(text);
        string _E(string key, string originalText)
        {
            if (eventCalendar.OriginalLanguage) { return originalText; }
            if (eventCalendar.PlayerLanguage)
            {
                if (Language == VRCPlayerApi.GetCurrentLanguage()) { return originalText; }
                if (multilinguals.TryGetValue(VRCPlayerApi.GetCurrentLanguage(), out var pvalue))
                {
                    if (pvalue.TokenType != TokenType.DataDictionary) { return originalText; }
                    var dataDictionary = pvalue.DataDictionary;
                    if (!dataDictionary.TryGetValue(key, out var textValue)) { return originalText; }
                    if (textValue.TokenType != TokenType.String) { return originalText; }
                    return textValue.String;
                }
            }
            var currentLanguage = eventCalendar.Translate.currentLanguage;
            if (Language == currentLanguage) { return originalText; }
            if (multilinguals.TryGetValue(currentLanguage, out var cvalue))
            {
                if (cvalue.TokenType != TokenType.DataDictionary) { return originalText; }
                var dataDictionary = cvalue.DataDictionary;
                if (!dataDictionary.TryGetValue(key, out var textValue)) { return originalText; }
                if (textValue.TokenType != TokenType.String) { return originalText; }
                return textValue.String;
            }
            return originalText;
        }
        #endregion
    }
}
