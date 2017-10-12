using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
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

        public NopInlineSriptTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
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

            //get javascript
            var script = output.GetChildContentAsync().Result.GetContent();

            _htmlHelper.AppendInlineScriptParts(Location, script);

            //generate nothing
            output.SuppressOutput();
        }
    }
}