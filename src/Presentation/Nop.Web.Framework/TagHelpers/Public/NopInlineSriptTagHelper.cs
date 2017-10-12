using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Microsoft.AspNetCore.Html;
using Nop.Core.Domain.Seo;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// Represents a tag helper which is used for placing javascript (instead of usual <script></script>)
    /// </summary>
    [HtmlTargetElement("nop-inline-script", Attributes = LocationAttributeName)]
    public class NopInlineSriptTagHelper : TagHelper
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

        public NopInlineSriptTagHelper(IHtmlHelper htmlHelper, SeoSettings seoSettings)
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

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //get JavaScript
            var script = output.GetChildContentAsync().Result.GetContent();

            if (!_seoSettings.MoveInlineJsToFooter)
            {
                output.SuppressOutput();
                output.Content.SetHtmlContent(new HtmlString(script));
                return;
            }

            _htmlHelper.AppendInlineScriptParts(Location, script);

            //generate nothing
            output.SuppressOutput();
        }
    }
}