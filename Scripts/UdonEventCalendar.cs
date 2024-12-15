
using System;
using System.Linq;
using Sonic853.Translate;
using Sonic853.Udon.ArrayPlus;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class UdonEventCalendar : UdonSharpBehaviour
    {
        public TranslateManager translateManager;
        /// <summary>
        /// 使用原始语言
        /// </summary>
        public bool OriginalLanguage = true;
        /// <summary>
        /// 使用玩家的语言，前提为原始语言为false
        /// </summary>
        public bool PlayerLanguage = true;
        public TranslateManager Translate => translateManager == null ? translateManager = TranslateManager.Instance() : translateManager;
        [SerializeField] EventListTime eventListTimePrefab;
        DateTime[] dateTimes = new DateTime[0];
        EventListTime[] eventListTimes = new EventListTime[0];
        [SerializeField] EventListItem eventListItemPrefab;
        public EventListItem EventListItemPrefab => eventListItemPrefab;
        EventListItem[] eventListItems = new EventListItem[0];
        public EventListItem[] EventListItems => eventListItems;
        [SerializeField] RectTransform eventListRoot;
        public RectTransform EventListRoot => eventListRoot;
        [SerializeField] EventToggle eventPlatformPrefab;
        EventToggle[] eventTogglePlatforms = new EventToggle[0];
        [SerializeField] EventToggle eventTagPrefab;
        EventToggle[] eventToggleTags = new EventToggle[0];
        [SerializeField] TMP_InputField searchInput;
        string[] searchPlatforms = new string[0];
        string[] searchTags = new string[0];
        [SerializeField] RectTransform tagsRoot;
        [SerializeField] ToggleGroup eventListToggleGroup;
        [SerializeField] EventContent eventContent;
        [SerializeField] GameObject eventInfo;
        [SerializeField] GameObject eventLoading;
        [SerializeField] TextAsset testText;
        public int imageCount = 0;
        public DataList events = new DataList();
        public DataDictionary i18n = new DataDictionary();
        DataToken data;
        public string content;
        public void Start()
        {
            if (!translateManager.LoadedTranslate) translateManager.LoadTranslate();
            if (testText != null) LoadEvents(testText.text);
        }
        public void LoadEvents() => LoadEvents(content);
        public void LoadEvents(string _content)
        {
            ShowLoading();
            ClearEventList();
            if (!VRCJson.TryDeserializeFromJson(_content, out data)) { return; }
            if (data.TokenType != TokenType.DataDictionary) { return; }
            var dataDictionary = data.DataDictionary;
            if (dataDictionary.TryGetValue("imageCount", out var imageCountValue))
            {
                var tokenType = imageCountValue.TokenType;
                switch (tokenType)
                {
                    case TokenType.Int:
                        imageCount = imageCountValue.Int;
                        break;
                    case TokenType.Long:
                        imageCount = (int)imageCountValue.Long;
                        break;
                    case TokenType.Double:
                        imageCount = (int)imageCountValue.Double;
                        break;
                    case TokenType.Float:
                        imageCount = (int)imageCountValue.Float;
                        break;
                    default:
                        break;
                }
            }
            if (dataDictionary.TryGetValue("platform", out var platformValue)
            && platformValue.TokenType == TokenType.DataDictionary)
            {
                var platformDataDictionary = platformValue.DataDictionary;
                var platformKeyList = platformDataDictionary.GetKeys();
                var platformKeyListCount = platformKeyList.Count;
                // platforms = new string[platformKeyListCount];
                eventTogglePlatforms = new EventToggle[platformKeyListCount];
                for (var i = 0; i < platformKeyListCount; i++)
                {
                    if (!platformKeyList.TryGetValue(i, out var item)) { continue; }
                    if (item.TokenType != TokenType.String) { continue; }
                    var itemString = item.String;
                    var eventTogglePlatform = (EventToggle)Instantiate(eventPlatformPrefab.gameObject, tagsRoot).GetComponent(typeof(UdonBehaviour));
                    eventTogglePlatform.eventCalendar = this;
                    eventTogglePlatform.ToggleName = itemString;
                    eventTogglePlatform.gameObject.SetActive(true);
                    // eventTogglePlatforms.SetValue(eventTogglePlatform, i);
                    eventTogglePlatforms[i] = eventTogglePlatform;
                    // platforms.SetValue(itemString, i);
                }
            }
            if (dataDictionary.TryGetValue("i18n", out var i18nValue)
            && i18nValue.TokenType == TokenType.DataDictionary)
            {
                i18n = i18nValue.DataDictionary;
            }
            if (dataDictionary.TryGetValue("tags", out var tagsValue)
            && tagsValue.TokenType == TokenType.DataDictionary)
            {
                var tagsDataDictionary = tagsValue.DataDictionary;
                var tagsKeyList = tagsDataDictionary.GetKeys();
                var tagsKeyListCount = tagsKeyList.Count;
                // tags = new string[tagsKeyListCount];
                eventToggleTags = new EventToggle[tagsKeyListCount];
                for (var i = 0; i < tagsKeyListCount; i++)
                {
                    if (!tagsKeyList.TryGetValue(i, out var item)) { continue; }
                    if (item.TokenType != TokenType.String) { continue; }
                    var itemString = item.String;
                    var eventToggleTag = (EventToggle)Instantiate(eventTagPrefab.gameObject, tagsRoot).GetComponent(typeof(UdonBehaviour));
                    eventToggleTag.eventCalendar = this;
                    eventToggleTag.ToggleName = itemString;
                    eventToggleTag.gameObject.SetActive(true);
                    // eventToggleTags.SetValue(eventToggleTag, i);
                    eventToggleTags[i] = eventToggleTag;
                    // tags.SetValue(itemString, i);
                }
            }
            if (dataDictionary.TryGetValue("events", out var eventsValue)
            && eventsValue.TokenType == TokenType.DataList)
            {
                events = eventsValue.DataList;
                var eventsCount = events.Count;
                eventListItems = new EventListItem[0];
                for (var i = 0; i < eventsCount; i++)
                {
                    if (!events.TryGetValue(i, out var eventValue)) { continue; }
                    if (eventValue.TokenType != TokenType.DataDictionary) { continue; }
                    var eventDataDictionary = eventValue.DataDictionary;
                    if (!eventDataDictionary.TryGetValue("start", out var dateValue)) { continue; }
                    if (dateValue.TokenType != TokenType.String) { continue; }
                    var dateValueString = dateValue.String;
                    if (string.IsNullOrEmpty(dateValueString)) { continue; }
                    if (!DateTime.TryParse(dateValueString, out var startTime)) { continue; }
                    // 如果结束日期过了今天凌晨的00点00分，就不显示
                    var endTime = startTime;
                    var haveEndTime = false;
                    if (eventDataDictionary.TryGetValue("end", out var endValue)
                    && endValue.TokenType == TokenType.String
                    && !string.IsNullOrEmpty(endValue.String)
                    && DateTime.TryParse(endValue.String, out endTime)) { haveEndTime = true; }
                    if (endTime < DateTime.Now) { continue; }
                    var index = DateTimesIndex(startTime);
                    if (index == -1)
                    {
                        UdonArrayPlus.Add(ref dateTimes, startTime);
                        var eventListTime = (EventListTime)Instantiate(eventListTimePrefab.gameObject, eventListRoot).GetComponent(typeof(UdonBehaviour));
                        eventListTime.eventCalendar = this;
                        eventListTime.Date = startTime;
                        eventListTime.gameObject.SetActive(true);
                        UdonArrayPlus.Add(ref eventListTimes, eventListTime);
                        index = dateTimes.Length - 1;
                    }
                    EventListItem item;
                    if (haveEndTime)
                    {
                        item = eventListTimes[index].AddItem(eventDataDictionary, startTime, endTime);
                    }
                    else
                    {
                        item = eventListTimes[index].AddItem(eventDataDictionary, startTime, default);
                    }
                    if (item == null) { continue; }
                    UdonArrayPlus.Add(ref eventListItems, item);
                }
            }
            HideLoading();
        }
        public int DateTimesIndex(DateTime dateTime)
        {
            // 对比其中的年月日
            for (var i = 0; i < dateTimes.Length; i++)
            {
                if (dateTimes[i].Year == dateTime.Year && dateTimes[i].Month == dateTime.Month && dateTimes[i].Day == dateTime.Day)
                {
                    return i;
                }
            }
            return -1;
        }
        public void ClearEventList()
        {
            foreach (var eventListTime in eventListTimes)
            {
                if (eventListTime == null) { continue; }
                Destroy(eventListTime.gameObject);
            }
            eventListTimes = new EventListTime[0];
            dateTimes = new DateTime[0];
            foreach (var eventListItem in eventListItems)
            {
                if (eventListItem == null) { continue; }
                Destroy(eventListItem.gameObject);
            }
            eventListItems = new EventListItem[0];
            foreach (var eventTogglePlatform in eventTogglePlatforms)
            {
                if (eventTogglePlatform == null) { continue; }
                Destroy(eventTogglePlatform.gameObject);
            }
            eventTogglePlatforms = new EventToggle[0];
            foreach (var eventToggleTag in eventToggleTags)
            {
                if (eventToggleTag == null) { continue; }
                Destroy(eventToggleTag.gameObject);
            }
            eventToggleTags = new EventToggle[0];
        }
        public void EventSelected()
        {
            if (!eventListToggleGroup.AnyTogglesOn())
            {
                eventContent.CloseEventCard();
                return;
            }
            foreach (var eventListItem in eventListItems)
            {
                if (eventListItem == null) { continue; }
                if (eventListItem.Toggle.isOn)
                {
                    eventContent.UpdateData(eventListItem);
                    eventContent.OpenEventCard();
                    return;
                }
            }
        }
        public void ToggleTags()
        {
            if (eventTogglePlatforms.Length == 0 && eventToggleTags.Length == 0)
            {
                tagsRoot.gameObject.SetActive(false);
                return;
            }
            tagsRoot.gameObject.SetActive(!tagsRoot.gameObject.activeSelf);
        }
        public void ToggleInfo() => eventInfo.SetActive(!eventInfo.activeSelf);
        public void ShowLoading() => eventLoading.SetActive(true);
        public void HideLoading() => eventLoading.SetActive(false);
        public void Search()
        {
            // var show = new bool[eventListItems.Length];
            var searchInputText = searchInput.text;
            var notNullSearchInput = !string.IsNullOrWhiteSpace(searchInputText);
            foreach (var eventListItem in eventListItems)
            {
                var show = true;
                if (eventListItem == null) { continue; }
                if (notNullSearchInput) show = eventListItem.Title.Contains(searchInputText);
                if (!show)
                {
                    eventListItem.gameObject.SetActive(false);
                    continue;
                }
                foreach (var searchPlatform in searchPlatforms)
                {
                    if (string.IsNullOrEmpty(searchPlatform)) { continue; }
                    if (!UdonArrayPlus.Contains(eventListItem.Platform, searchPlatform))
                    {
                        show = false;
                        break;
                    }
                }
                if (!show)
                {
                    eventListItem.gameObject.SetActive(false);
                    continue;
                }
                foreach (var searchTag in searchTags)
                {
                    if (string.IsNullOrEmpty(searchTag)) { continue; }
                    if (!UdonArrayPlus.Contains(eventListItem.Tags, searchTag))
                    {
                        show = false;
                        break;
                    }
                }
                if (!show)
                {
                    eventListItem.gameObject.SetActive(false);
                    continue;
                }
                eventListItem.gameObject.SetActive(true);
            }
        }
        public void SearchUpdated() => Search();
        public void PlatformSelected()
        {
            searchPlatforms = new string[0];
            foreach (var eventTogglePlatform in eventTogglePlatforms)
            {
                if (eventTogglePlatform == null) { continue; }
                if (eventTogglePlatform.Toggle.isOn)
                {
                    var toggleName = eventTogglePlatform.ToggleName;
                    if (!string.IsNullOrEmpty(toggleName)) UdonArrayPlus.Add(ref searchPlatforms, toggleName);
                }
            }
            Search();
        }
        public void TagSelected()
        {
            searchTags = new string[0];
            foreach (var eventToggleTag in eventToggleTags)
            {
                if (eventToggleTag == null) { continue; }
                if (eventToggleTag.Toggle.isOn)
                {
                    var toggleName = eventToggleTag.ToggleName;
                    if (!string.IsNullOrEmpty(toggleName)) UdonArrayPlus.Add(ref searchTags, toggleName);
                }
            }
            Search();
        }
        #region 翻译
        public string _(string text) => Translate.GetText(text);
        public string _i18n(string text)
        {
            if (!i18n.TryGetValue(text, out var i18nvalue)) { return text; }
            if (i18nvalue.TokenType != TokenType.DataDictionary) { return text; }
            var dataDictionary = i18nvalue.DataDictionary;
            if (!dataDictionary.TryGetValue(Translate.currentLanguage, out var value)) { return text; }
            return value.String;
        }
        #endregion
    }
}
