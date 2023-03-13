using Microsoft.AspNetCore.Html;        
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersManager.Models;
using System.Text;

namespace HtmlHelper
{
    public static class ListHelper
    {
        public static IHtmlContent Table(this IHtmlHelper helper, List<OrderModel> data, List<string> headers)
        {
            //Tags
            var table = new TagBuilder("table");
            var tr = new TagBuilder("tr");

            //Add headers
            foreach (var s in headers)
            {
                var th = new TagBuilder("th");
                th.InnerHtml.Append(s);
                tr.InnerHtml.AppendHtml(th);
            }
            table.InnerHtml.AppendHtml(tr);

            //Add data
            foreach (var d in data)
            {
                tr = new TagBuilder("tr");
                foreach (var h in headers)
                {
                    var td = new TagBuilder("td");
                    td.InnerHtml.Append(d.ToString());
                    tr.InnerHtml.AppendHtml(td);
                }

                table.InnerHtml.AppendHtml(tr);
            }

            return table;
        }
    }   
}
