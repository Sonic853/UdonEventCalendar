
using System;
using Sonic853.Udon.ArrayPlus;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.EventCalendar
{
    public class EventListTime : UdonSharpBehaviour
    {
        [SerializeField] public UdonEventCalendar eventCalendar;
        [SerializeField] TMP_Text titleUI;
        DateTime dateTime;
        public DateTime Date
        {
            get => dateTime;
            set
            {
                dateTime = value;
                var elt = _("ELT:{0} {1}");
                if (elt.StartsWith("ELT:")) elt = elt.Substring(4);
                titleUI.text = string.Format(elt, dateTime.ToString(_("yyyy-MM-dd")), _(dateTime.DayOfWeek.ToString()));
            }
        }
        public EventListItem[] eventListItems;
        public EventListItem AddItem(DataDictionary data)
        {
            if (!data.TryGetValue("start", out var dateValue)) { return null; }
            if (dateValue.TokenType != TokenType.String) { return null; }
            if (!DateTime.TryParse(dateValue.String, out var startTime)) { return null; }
            return AddItem(data, startTime);
        }
        public EventListItem AddItem(DataDictionary data, DateTime startTime)
        {
            if (data.TryGetValue("end", out var endTimeValue)
            && endTimeValue.TokenType == TokenType.String
            && !string.IsNullOrEmpty(endTimeValue.String)
            && DateTime.TryParse(endTimeValue.String, out var endTime))
            {
                return AddItem(data, startTime, endTime);
            }
            return AddItem(data, startTime, default);
        }
        public EventListItem AddItem(DataDictionary data, DateTime startTime, DateTime endTime)
        {
            // 数据处理
            var id = "";
            if (data.TryGetValue("id", out var idValue) && idValue.TokenType != TokenType.String)
            {
                id = idValue.String;
                if (!string.IsNullOrEmpty(id))
                {
                    var index = EventListItemsIndex(id);
                    if (index != -1) { return eventListItems[index]; }
                }
            }
            var eventListItem = (EventListItem)Instantiate(eventCalendar.EventListItemPrefab.gameObject, eventCalendar.EventListRoot).GetComponent(typeof(UdonBehaviour));
            eventListItem.eventCalendar = eventCalendar;
            eventListItem.ID = id;
            if (data.TryGetValue("language", out var languageValue) && languageValue.TokenType == TokenType.String)
            {
                eventListItem.Language = languageValue.String;
            }
            if (data.TryGetValue("multilingual", out var multilingualValue) && multilingualValue.TokenType == TokenType.DataDictionary)
            {
                eventListItem.multilinguals = multilingualValue.DataDictionary;
            }
            if (data.TryGetValue("title", out var titleValue) && titleValue.TokenType == TokenType.String)
            {
                eventListItem.Title = titleValue.String;
            }
            if (data.TryGetValue("description", out var descriptionValue) && descriptionValue.TokenType == TokenType.String)
            {
                eventListItem.Description = descriptionValue.String;
            }
            if (data.TryGetValue("author", out var authorValue) && authorValue.TokenType == TokenType.String)
            {
                eventListItem.Author = authorValue.String;
            }
            eventListItem.StartTime = startTime;
            eventListItem.EndTime = endTime;
            if (data.TryGetValue("location", out var locationValue) && locationValue.TokenType == TokenType.String)
            {
                eventListItem.Location = locationValue.String;
            }
            if (data.TryGetValue("instance_type", out var instanceTypeValue) && instanceTypeValue.TokenType == TokenType.String)
            {
                eventListItem.InstanceType = instanceTypeValue.String;
            }
            if (data.TryGetValue("platform", out var platformValue) && platformValue.TokenType == TokenType.DataList)
            {
                var platformList = platformValue.DataList;
                var platforms = new string[platformList.Count];
                for (var i = 0; i < platformList.Count; i++)
                {
                    if (platformList[i].TokenType == TokenType.String)
                    {
                        platforms[i] = platformList[i].String;
                    }
                }
                eventListItem.Platform = platforms;
            }
            if (data.TryGetValue("tags", out var tagsValue) && tagsValue.TokenType == TokenType.DataList)
            {
                var tagsList = tagsValue.DataList;
                var tags = new string[tagsList.Count];
                for (var i = 0; i < tagsList.Count; i++)
                {
                    if (tagsList[i].TokenType == TokenType.String)
                    {
                        tags[i] = tagsList[i].String;
                    }
                }
                eventListItem.Tags = tags;
            }
            // group 既有可能是 string 也有可能是 dataDictionary
            if (data.TryGetValue("group", out var groupValue))
            {
                if (groupValue.TokenType == TokenType.String)
                {
                    eventListItem.GroupName = groupValue.String;
                }
                else if (groupValue.TokenType == TokenType.DataDictionary)
                {
                    var groupDictionary = groupValue.DataDictionary;
                    if (groupDictionary.TryGetValue("name", out var groupNameValue) && groupNameValue.TokenType == TokenType.String)
                    {
                        eventListItem.GroupName = groupNameValue.String;
                    }
                    if (groupDictionary.TryGetValue("id", out var groupIdValue) && groupIdValue.TokenType == TokenType.String)
                    {
                        eventListItem.GroupID = groupIdValue.String;
                    }
                }
            }
            if (data.TryGetValue("require", out var requireValue) && requireValue.TokenType == TokenType.String)
            {
                eventListItem.Require = requireValue.String;
            }
            if (data.TryGetValue("join", out var joinValue) && joinValue.TokenType == TokenType.String)
            {
                eventListItem.Join = joinValue.String;
            }
            if (data.TryGetValue("note", out var noteValue) && noteValue.TokenType == TokenType.String)
            {
                eventListItem.Note = noteValue.String;
            }
            // 数据处理结束
            var itemIndex = EventListItemsInset(eventListItem);
            if (itemIndex == 0)
            {
                eventListItem.transform.parent = eventCalendar.EventListRoot;
                var transformIndex = gameObject.transform.GetSiblingIndex();
                eventListItem.transform.SetSiblingIndex(transformIndex + 1);
            }
            else
            {
                var preEventListItem = eventListItems[itemIndex - 1];
                var preTransformIndex = preEventListItem.transform.GetSiblingIndex();
                eventListItem.transform.SetSiblingIndex(preTransformIndex + 1);
            }
            eventListItem.gameObject.SetActive(true);
            return eventListItem;
        }
        int EventListItemsIndex(string id)
        {
            for (var i = 0; i < eventListItems.Length; i++)
            {
                if (eventListItems[i].ID == id) { return i; }
            }
            return -1;
        }
        /// <summary>
        /// 根据 EventListItem[] 里的 StartTime 从旧到新排序放入 EventListItem
        /// </summary>
        int EventListItemsInset(EventListItem eventListItem)
        {
            if (eventListItem.StartTime == default)
            {
                UdonArrayPlus.Add(ref eventListItems, eventListItem);
                return eventListItems.Length - 1;
            }
            for (var i = 0; i < eventListItems.Length; i++)
            {
                var _eventListItem = eventListItems[i];
                if (_eventListItem == null) { continue; }
                if (_eventListItem.StartTime == default)
                {
                    // 前一个
                    var preIndex = i - 1;
                    if (preIndex < 0)
                    {
                        UdonArrayPlus.Add(ref eventListItems, eventListItem);
                    }
                    var preEventListItem = eventListItems[preIndex];
                    if (preEventListItem == null)
                    {
                        UdonArrayPlus.Insert(ref eventListItems, i, eventListItem);
                        return i;
                    }
                    if (eventListItem.StartTime >= preEventListItem.StartTime)
                    {
                        UdonArrayPlus.Insert(ref eventListItems, i, eventListItem);
                        return i;
                    }
                    UdonArrayPlus.Add(ref eventListItems, eventListItem);
                    return eventListItems.Length - 1;
                }
                if (eventListItem.StartTime < _eventListItem.StartTime)
                {
                    UdonArrayPlus.Insert(ref eventListItems, i, eventListItem);
                    return i;
                }
            }
            UdonArrayPlus.Add(ref eventListItems, eventListItem);
            return eventListItems.Length - 1;
        }
        #region 翻译
        public string _(string text) => eventCalendar.Translate.GetText(text);
        #endregion
    }
}
