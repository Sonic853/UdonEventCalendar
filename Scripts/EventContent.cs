
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
namespace Sonic853.Udon.EventCalendar
{
    public class EventContent : UdonSharpBehaviour
    {
        [SerializeField] public UdonEventCalendar eventCalendar;
        [SerializeField] Animator animator;
        #region UI
        /// <summary>
        /// 标题
        /// </summary>
        [SerializeField] TMP_Text titleUI;
        /// <summary>
        /// 举办者
        /// </summary>
        [SerializeField] TMP_Text authorUI;
        /// <summary>
        /// 描述
        /// </summary>
        [SerializeField] TMP_Text descriptionUI;
        /// <summary>
        /// 描述物体
        /// </summary>
        [SerializeField] GameObject descriptionObject;
        /// <summary>
        /// 群组名
        /// </summary>
        [SerializeField] TMP_Text groupUI;
        /// <summary>
        /// 群组按钮
        /// </summary>
        [SerializeField] Button groupBtn;
        /// <summary>
        /// 群组输入框
        /// </summary>
        [SerializeField] InputField groupInput;
        /// <summary>
        /// 群组物体
        /// </summary>
        [SerializeField] GameObject groupObject;
        /// <summary>
        /// 地点
        /// </summary>
        [SerializeField] TMP_Text locationUI;
        /// <summary>
        /// 地点物体
        /// </summary>
        [SerializeField] GameObject locationObject;
        /// <summary>
        /// 房间类型
        /// </summary>
        [SerializeField] TMP_Text instanceTypeUI;
        /// <summary>
        /// 房间类型物体
        /// </summary>
        [SerializeField] GameObject instanceTypeObject;
        /// <summary>
        /// 平台
        /// </summary>
        [SerializeField] TMP_Text platformUI;
        /// <summary>
        /// 平台物体
        /// </summary>
        [SerializeField] GameObject platformObject;
        /// <summary>
        /// 标签
        /// </summary>
        [SerializeField] TMP_Text tagsUI;
        /// <summary>
        /// 标签物体
        /// </summary>
        [SerializeField] GameObject tagsObject;
        /// <summary>
        /// 时间
        /// </summary>
        [SerializeField] TMP_Text timeUI;
        /// <summary>
        /// 要求
        /// </summary>
        [SerializeField] TMP_Text requireUI;
        /// <summary>
        /// 要求物体
        /// </summary>
        [SerializeField] GameObject requireObject;
        /// <summary>
        /// 加入方式
        /// </summary>
        [SerializeField] TMP_Text joinUI;
        /// <summary>
        /// 加入方式物体
        /// </summary>
        [SerializeField] GameObject joinObject;
        /// <summary>
        /// 备注
        /// </summary>
        [SerializeField] TMP_Text noteUI;
        /// <summary>
        /// 备注物体
        /// </summary>
        [SerializeField] GameObject noteObject;
        /// <summary>
        /// Scroll View
        /// </summary>
        [SerializeField] ScrollRect scrollView;
        [SerializeField] bool useGroupBtn = true;
        #endregion
        #region Property
        /// <summary>
        /// 标题
        /// </summary>
        string title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                titleUI.text = _E("title", title);
            }
        }
        string author;
        /// <summary>
        /// 举办者
        /// </summary>
        public string Author
        {
            get => author;
            set
            {
                author = value;
                authorUI.text = string.Format(_("<b>Author:</b> {0}"), value);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        string description;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                description = value;
                descriptionUI.text = _E("description", description);
                descriptionObject.SetActive(!string.IsNullOrEmpty(descriptionUI.text));
            }
        }
        /// <summary>
        /// 群组名
        /// </summary>
        string groupName;
        /// <summary>
        /// 群组名
        /// </summary>
        public string GroupName
        {
            get => groupName;
            set
            {
                groupUI.text = groupName = value;
                if (!string.IsNullOrEmpty(groupID) && !groupID.StartsWith("grp_"))
                {
                    var ecgroup = _("EC Group:{0} ({1})");
                    if (ecgroup.StartsWith("EC Group:")) ecgroup = ecgroup.Substring(9);
                    groupUI.text = string.Format(ecgroup, groupName, groupID);
                }
                groupObject.SetActive(!string.IsNullOrEmpty(groupUI.text));
            }
        }
        /// <summary>
        /// 群组ID
        /// </summary>
        string groupID;
        /// <summary>
        /// 群组ID
        /// </summary>
        public string GroupID
        {
            get => groupID;
            set
            {
                groupID = value;
                if (!string.IsNullOrEmpty(groupID))
                {
                    if (!groupID.StartsWith("grp_"))
                    {
                        groupBtn.gameObject.SetActive(false);
                        var ecgroup = _("EC Group:{0} ({1})");
                        if (ecgroup.StartsWith("EC Group:")) ecgroup = ecgroup.Substring(9);
                        groupUI.text = string.Format(ecgroup, groupName, groupID);
                        groupInput.gameObject.SetActive(true);
                        groupInput.text = $"https://vrc.group/{groupID}";
                    }
                    else
                    {
                        groupBtn.gameObject.SetActive(useGroupBtn);
                        groupInput.gameObject.SetActive(!useGroupBtn);
                        groupInput.text = $"https://vrchat.com/home/group/{groupID}";
                    }
                }
                groupObject.SetActive(!string.IsNullOrEmpty(groupUI.text));
            }
        }
        /// <summary>
        /// 地点
        /// </summary>
        public string Location
        {
            get => locationUI.text;
            set
            {
                locationUI.text = value;
                locationObject.SetActive(!string.IsNullOrEmpty(locationUI.text));
            }
        }
        string instanceType;
        /// <summary>
        /// 房间类型
        /// </summary>
        public string InstanceType
        {
            get => instanceType;
            set
            {
                instanceType = value;
                instanceTypeUI.text = _(instanceType);
                instanceTypeObject.SetActive(!string.IsNullOrEmpty(instanceTypeUI.text));
            }
        }
        /// <summary>
        /// 平台
        /// </summary>
        string[] platform;
        /// <summary>
        /// 平台
        /// </summary>
        public string[] Platform
        {
            get => platform;
            set
            {
                platform = value;
                if (platform != null)
                {
                    var platformLength = platform.Length;
                    var tPlatform = new string[platformLength];
                    for (int i = 0; i < platformLength; i++)
                    {
                        tPlatform.SetValue(_(platform[i]), i);
                    }
                    platformUI.text = string.Join(_(", "), tPlatform);
                }
                platformObject.SetActive(!string.IsNullOrEmpty(platformUI.text));
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        string[] tags;
        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags
        {
            get => tags;
            set
            {
                tags = value;
                if (tags != null)
                {
                    var tagsLength = tags.Length;
                    var tTags = new string[tagsLength];
                    for (int i = 0; i < tagsLength; i++)
                    {
                        var tTag = _i18n(tags[i]);
                        tTags.SetValue(tTag, i);
                    }
                    tagsUI.text = string.Join(_(", "), tTags);
                }
                tagsObject.SetActive(!string.IsNullOrEmpty(tagsUI.text));
            }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime startTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                if (startTime != default && endTime != default)
                {
                    var ectime = _("EC Time:{0} {1} {2} - {3}");
                    if (ectime.StartsWith("EC Time:")) ectime = ectime.Substring(8);
                    timeUI.text = string.Format(ectime, startTime.ToString(_("yyyy-MM-dd")), _(startTime.DayOfWeek.ToString()), startTime.ToString(_("HH:mm")), endTime.ToString(_("HH:mm")));
                }
                else if (startTime != default)
                {
                    var ectime = _("EC Time:{0} {1} {2}");
                    if (ectime.StartsWith("EC Time:")) ectime = ectime.Substring(8);
                    timeUI.text = string.Format(ectime, startTime.ToString(_("yyyy-MM-dd")), _(startTime.DayOfWeek.ToString()), startTime.ToString(_("HH:mm")));
                }
            }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get => endTime;
            set
            {
                endTime = value;
                if (startTime != default && endTime != default)
                {
                    timeUI.text = string.Format(_("EC Time:{0} {1} {2} - {3}"), startTime.ToString(_("yyyy-MM-dd")), _(startTime.DayOfWeek.ToString()), startTime.ToString(_("HH:mm")), endTime.ToString(_("HH:mm")));
                }
            }
        }
        /// <summary>
        /// 要求
        /// </summary>
        public string Require
        {
            get => requireUI.text;
            set
            {
                requireUI.text = value;
                requireObject.SetActive(!string.IsNullOrEmpty(requireUI.text));
            }
        }
        /// <summary>
        /// 加入方式
        /// </summary>
        public string Join
        {
            get => joinUI.text;
            set
            {
                joinUI.text = value;
                joinObject.SetActive(!string.IsNullOrEmpty(joinUI.text));
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get => noteUI.text;
            set
            {
                noteUI.text = value;
                noteObject.SetActive(!string.IsNullOrEmpty(noteUI.text));
            }
        }
        /// <summary>
        /// 原始语言
        /// </summary>
        public string Language;
        /// <summary>
        /// 多语言
        /// </summary>
        DataDictionary multilinguals = new DataDictionary();
        #endregion
        public void UpdateData(EventListItem eventListItem)
        {
            if (eventListItem == null) { return; }
            ClearContent();
            Title = eventListItem.Title;
            Author = eventListItem.Author;
            StartTime = eventListItem.StartTime;
            EndTime = eventListItem.EndTime;
            if (eventListItem.multilinguals != null) { multilinguals = eventListItem.multilinguals; }
            Description = eventListItem.Description;
            GroupName = eventListItem.GroupName;
            GroupID = eventListItem.GroupID;
            Location = eventListItem.Location;
            InstanceType = eventListItem.InstanceType;
            Platform = eventListItem.Platform;
            Tags = eventListItem.Tags;
            Require = eventListItem.Require;
            Join = eventListItem.Join;
            Note = eventListItem.Note;
        }
        public void ClearContent()
        {
            Title = "";
            Author = "";
            StartTime = default;
            EndTime = default;
            multilinguals = new DataDictionary();
            Description = "";
            GroupName = "";
            GroupID = "";
            Location = "";
            Platform = new string[0];
            Tags = new string[0];
            Require = "";
            Join = "";
            Note = "";
        }
        public void OpenEventCard()
        {
            animator.SetBool("Show", true);
            scrollView.verticalNormalizedPosition = 1;
        }
        public void CloseEventCard()
        {
            animator.SetBool("Show", false);
            foreach (var item in eventCalendar.EventListItems)
            {
                item.Toggle.SetIsOnWithoutNotify(false);
            }
        }
        public void OpenGroupPage()
        {
            if (string.IsNullOrEmpty(groupID) || !groupID.StartsWith("grp_")) { return; }
            if (!Networking.LocalPlayer.isLocal) { return; }
            VRC.Economy.Store.OpenGroupPage(groupID);
        }
        #region 翻译
        public string _(string text) => eventCalendar.Translate.GetText(text);
        public string _i18n(string text) => eventCalendar._i18n(text);
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
