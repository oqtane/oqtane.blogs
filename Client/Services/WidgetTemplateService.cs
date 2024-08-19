using Microsoft.Extensions.Localization;
using Oqtane.Blogs.Models;
using Oqtane.Blogs.Shared;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Oqtane.Blogs.Services
{
    public interface IWidgetTemplateService
    {
        string GetWidgetTemplate(Dictionary<string, string> settings, string widgetName);
    }

    public class WidgetTemplateService : ServiceBase, IWidgetTemplateService, IService
    {
        private const string ResourceType = "Oqtane.Blogs.Widgets.Index, Oqtane.Blogs.Client.Oqtane";
        private readonly ISettingService _settingService;
        private readonly IDictionary<string, string> _defaultTemplates;

        public WidgetTemplateService(HttpClient http, ISettingService settingService, IStringLocalizerFactory stringLocalizerFactory) : base(http)
        {
            _settingService = settingService;

            var localizer = stringLocalizerFactory.Create(ResourceType);
            _defaultTemplates = new Dictionary<string, string>();
            foreach(var widget in BlogConstants.AvailableWidgets)
            {
                _defaultTemplates.Add(widget, localizer[$"{widget}.Template"]);
            }
        }

        public string GetWidgetTemplate(Dictionary<string, string> settings, string widgetName)
        {
            var settingName = $"WidgetTemplate";
            if (settings.ContainsKey(settingName))
            {
                return _settingService.GetSetting(settings, settingName, string.Empty);
            }
            
            if (_defaultTemplates.ContainsKey(widgetName))
            {
                return _defaultTemplates[widgetName] ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
