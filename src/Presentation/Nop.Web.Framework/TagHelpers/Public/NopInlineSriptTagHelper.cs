using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using System;
using System.Collections.Generic;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// Represents a tag helper which is used for placing javascript (instead of usual <script></script>)
    /// </summary>
    [HtmlTargetElement("nop-inline-script", Attributes = LocationAttributeName)]
    public class NopInlineSriptTagHelper : TagHelper
    {
        private const string LocationAttributeName = "asp-location";
        private const string ScriptContextName = "scriptContext";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LocationAttributeName)]
        public InlineScriptLocationEnum Location { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopInlineSriptTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
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

            //get script context from ViewData
            //we use ViewData (not TagHelperContext) because a parent tag helper  
            //doesn't share TagHelperContext if a child is not in the same view
            List<InlineScriptItem> scripts;
            if (_htmlHelper.ViewData.ContainsKey(ScriptContextName))
            {
                //get script context if exists
                scripts = (List<InlineScriptItem>)_htmlHelper.ViewData[ScriptContextName];
            }
            else
            {
                //or create a new one if doesn't
                scripts = new List<InlineScriptItem>();
                _htmlHelper.ViewData.Add(ScriptContextName, scripts);
            }

            //add scrip to ViewData to have access later
            scripts.Add(new InlineScriptItem()
            {
                Script = script,
                Location = Location
            });

            //generate nothing
            output.SuppressOutput();
        }
    }




    /// <summary>
    /// Represents a script item
    /// </summary>
    public class InlineScriptItem
    {
        public string Script { get; set; }

        public InlineScriptLocationEnum Location { get; set; }
    }

    /// <summary>
    /// Script location enum
    /// </summary>
    public enum InlineScriptLocationEnum
    {
        Head,
        Footer
    }




    /// <summary>
    /// Represents a tag helper which manages scripts on a page
    /// </summary>
    [HtmlTargetElement("nop-inline-script-manager")]
    public class NopInlineSriptManagerTagHelper : TagHelper
    {
        private const string ScriptContextName = "scriptContext";
        private const string LocationContextName = "locationContext";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopInlineSriptManagerTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
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

            //execute child tag helpers and get the content
            var content = output.GetChildContentAsync().Result.GetContent();

            //we use ViewData (not TagHelperContext) because a parent tag helper  
            //doesn't share TagHelperContext if a child is not in the same view
            var scriptContext = (List<InlineScriptItem>)_htmlHelper.ViewData[ScriptContextName];
            var locationContext = (List<InlineScriptLocationItem>)_htmlHelper.ViewData[LocationContextName];



            //render nothing because this tag helpere is used just to manage it's childs
            output.TagName = "";
        }
    }




    /// <summary>
    /// Represents a tag helper which renders scripts in the appropriate location
    /// </summary>
    [HtmlTargetElement("nop-inline-script-location", Attributes = LocationAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class NopInlineSriptLocationTagHelper : TagHelper
    {
        private const string LocationAttributeName = "asp-name";
        private const string LocationContextName = "locationContext";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LocationAttributeName)]
        public InlineScriptLocationEnum Location { set; get; }
        
        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopInlineSriptLocationTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
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

            var random = CommonHelper.GenerateRandomInteger();
            var inlineScriptLocationId = $"inlineScriptLocation{random}";

            //render a script tag with an appropriate id
            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("id", inlineScriptLocationId);

            //get javascript
            var script = output.GetChildContentAsync().Result.GetContent();

            //get location context from ViewData
            //we use ViewData (not TagHelperContext) because a parent tag helper  
            //doesn't share TagHelperContext if a child is not in the same view
            List<InlineScriptLocationItem> scripts;
            if (_htmlHelper.ViewData.ContainsKey(LocationContextName))
            {
                //get script context if exists
                scripts = (List<InlineScriptLocationItem>)_htmlHelper.ViewData[LocationContextName];
            }
            else
            {
                //or create a new one if doesn't
                scripts = new List<InlineScriptLocationItem>();
                _htmlHelper.ViewData.Add(LocationContextName, scripts);
            }

            //add scrip to ViewData to have access later
            scripts.Add(new InlineScriptLocationItem()
            {
                Id = inlineScriptLocationId,
                Location = Location
            });
        }
    }

    public class InlineScriptLocationItem
    {
        public string Id { get; set; }

        public InlineScriptLocationEnum Location { get; set; }
    }
}