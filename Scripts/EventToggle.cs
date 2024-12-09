
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class EventToggle : UdonSharpBehaviour
    {
        [SerializeField] public UdonEventCalendar eventCalendar;
        #region UI
        [SerializeField] Toggle toggleUI;
        public Toggle Toggle => toggleUI;
        [SerializeField] private TMP_Text toggleNameUI;
        #endregion
        string toggleName;
        public string ToggleName
        {
            get => toggleName;
            set
            {
                toggleName = value;
                var tTag = _($"{toggleName}");
                if (toggleName == tTag) tTag = _i18n(tTag);
                toggleNameUI.text = tTag;
            }
        }
        public void TagSelected() => eventCalendar.TagSelected();
        public void PlatformSelected() => eventCalendar.PlatformSelected();
        #region 翻译
        public string _(string text) => eventCalendar.Translate.GetText(text);
        public string _i18n(string text) => eventCalendar._i18n(text);
        #endregion
    }
}
