using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Ilaro.Admin.Commons.Paging
{
    public class Pager
    {
        private readonly ViewContext _viewContext;
        private readonly int _pageSize;
        private readonly int _currentPage;
        private readonly int _totalItemCount;
        private readonly RouteValueDictionary _linkWithoutPageValuesDictionary;
        private readonly AjaxOptions _ajaxOptions;

        public Pager(
            ViewContext viewContext,
            int pageSize,
            int currentPage,
            int totalItemCount,
            RouteValueDictionary valuesDictionary,
            AjaxOptions ajaxOptions)
        {
            _viewContext = viewContext;
            _pageSize = pageSize;
            _currentPage = currentPage;
            _totalItemCount = totalItemCount;
            _linkWithoutPageValuesDictionary = valuesDictionary;
            _ajaxOptions = ajaxOptions;
        }

        public HtmlString RenderHtml(string url)
        {
            var pageCount =
                (int)Math.Ceiling(_totalItemCount / (double)_pageSize);

            if (pageCount <= 1)
            {
                return new MvcHtmlString(String.Empty);
            }

            const int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            // Previous
            sb.Append(_currentPage > 1 ?
                GeneratePageLink("«", _currentPage - 1, url, "prev") :
                "<li class=\"disabled\"><span>«</span></li>");

            var start = 1;
            var end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (_currentPage - middle);
                var above = (_currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1, url));
                sb.Append(GeneratePageLink("2", 2, url));
                sb.Append("<li class=\"disabled\"><span>...</span></li>");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == _currentPage || (_currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat(GeneratePageLink(i.ToString(), i, url, "", "active"));
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i, url));
                }
            }
            if (end < (pageCount - 3))
            {
                sb.Append("<li class=\"disabled\"><span>...</span></li>");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1, url));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount, url));
            }

            // Next
            sb.Append(_currentPage < pageCount ?
                GeneratePageLink("»", (_currentPage + 1), url, "next") :
                "<li class=\"disabled\"><span>»</span></li>");

            return new HtmlString(
                "<div class=\"pagination\"><ul>" +
                sb +
                "</ul></div>");
        }

        public HtmlString RenderHtml()
        {
            var pageCount = (int)Math.Ceiling(_totalItemCount / (double)_pageSize);
            const int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            // Previous
            sb.Append(_currentPage > 1 ?
                GeneratePageLink("&lt;", _currentPage - 1, "previous") :
                "<span class=\"disabled\">&lt;</span>");

            var start = 1;
            var end = pageCount;

            if (pageCount > nrOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                var below = (_currentPage - middle);
                var above = (_currentPage + middle);

                if (below < 4)
                {
                    above = nrOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - nrOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append(GeneratePageLink("2", 2));
                sb.Append("<div class='more'>...</div>");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == _currentPage || (_currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                }
                else
                {
                    sb.Append(GeneratePageLink(i.ToString(), i));
                }
            }
            if (end < (pageCount - 3))
            {
                sb.Append("<div class='more'>...</div>");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }

            // Next
            sb.Append(_currentPage < pageCount ?
                GeneratePageLink("&gt;", (_currentPage + 1), "next") :
                "<span class=\"disabled\">&gt;</span>");

            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var routeDataValues = _viewContext.RequestContext.RouteData.Values;
            RouteValueDictionary pageLinkValueDictionary;
            // Avoid canonical errors when page count is equal to 1.
            if (pageNumber == 1)
            {
                pageLinkValueDictionary =
                    new RouteValueDictionary(_linkWithoutPageValuesDictionary);
                if (routeDataValues.ContainsKey("page"))
                {
                    routeDataValues.Remove("page");
                }
            }
            else
            {
                pageLinkValueDictionary =
                    new RouteValueDictionary(_linkWithoutPageValuesDictionary)
                    {
                        { "page", pageNumber }
                    };
            }

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") &&
                routeDataValues.ContainsKey("controller"))
            {
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);
            }
            if (!pageLinkValueDictionary.ContainsKey("action") &&
                routeDataValues.ContainsKey("action"))
            {
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);
            }

            // 'Render' virtual path.
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(
                _viewContext.RequestContext,
                pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<a");

            if (_ajaxOptions != null)
                foreach (var ajaxOption in _ajaxOptions.ToUnobtrusiveHtmlAttributes())
                    stringBuilder.AppendFormat(
                        " {0}=\"{1}\"",
                        ajaxOption.Key,
                        ajaxOption.Value);

            stringBuilder.AppendFormat(
                " href=\"{0}\">{1}</a>",
                virtualPathForArea.VirtualPath,
                linkText);

            return stringBuilder.ToString();
        }

        private string GeneratePageLink(
            string linkText,
            int pageNumber,
            string url,
            string rel = "",
            string @class = "")
        {
            var href = string.Format(url, pageNumber);
            var stringBuilder = new StringBuilder("<a");

            //if (ajaxOptions != null)
            //    foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())
            //        stringBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);

            if (!string.IsNullOrEmpty(rel))
            {
                stringBuilder.AppendFormat(" rel=\"{0}\"", rel);
            }

            stringBuilder.AppendFormat(" href=\"{0}\">{1}</a>", href, linkText);


            if (!string.IsNullOrEmpty(@class))
            {
                return string.Format("<li class=\"{1}\">{0}</li>", stringBuilder, @class);
            }
            return string.Format("<li>{0}</li>", stringBuilder);
        }
    }
}