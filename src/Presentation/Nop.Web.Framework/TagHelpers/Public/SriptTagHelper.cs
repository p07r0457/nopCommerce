using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Microsoft.AspNetCore.Html;
using Nop.Core.Domain.Seo;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("script", Attributes = LocationAttributeName)]
    public class SriptTagHelper : TagHelper
    {
        private const string LocationAttributeName = "asp-location";
        private readonly IHtmlHelper _htmlHelper;
        private readonly SeoSettings _seoSettings;

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LocationAttributeName)]
        public ResourceLocation Location { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public SriptTagHelper(IHtmlHelper htmlHelper, SeoSettings seoSettings)
        {
            this._htmlHelper = htmlHelper;
            this._seoSettings = seoSettings;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!_seoSettings.MoveJsToFooter)
            {
                return;
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //get JavaScript
            var script = output.GetChildContentAsync().Result.GetContent();

            //build script tag
            var scriptTag = new TagBuilder("script");
            scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

            //merge attributes
            foreach (var attribute in context.AllAttributes)
                if (!attribute.Name.ToString().StartsWith("asp-"))
                    scriptTag.Attributes.Add(attribute.Name, attribute.Value.ToString());

            _htmlHelper.AppendInlineScriptParts(Location, scriptTag.RenderHtmlContent());

            //generate nothing
            output.SuppressOutput();
        }
    }

}