using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ilaro.Admin.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-if")]
    public class IfTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-if")]
        public bool Predicate { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Predicate)
            {
                output.SuppressOutput();
            }
        }
    }
}
