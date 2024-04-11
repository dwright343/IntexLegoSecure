using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using IntexLegoSecure.Models.ViewModels;

namespace IntexLegoSecure.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        public PaginationTagHelper(IUrlHelperFactory temp)
        {
            _urlHelperFactory = temp;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        public string? PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();
        public PaginationInfo PageModel { get; set; }
        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; } = string.Empty;
        public string PageClassNormal { get; set; } = string.Empty;
        public string PageClassSelected { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

                TagBuilder result = new TagBuilder("div");

                for (int i = 1; i <= PageModel.TotalNumPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues["pageNum"] = i;
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);

                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal); // if we're on current page, use pageclass selected, otherwise use pageclass normal
                    }

                    tag.InnerHtml.Append(i.ToString());

                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }
}
