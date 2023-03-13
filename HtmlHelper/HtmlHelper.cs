using Microsoft.AspNetCore.Html;        
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersManager.Models;
using System.Text;

namespace HtmlHelper
{
    public static class ListHelper
    {
        public static IHtmlContent Table(this IHtmlHelper helper, List<OrderModel> data, List<(string propertyName, string colName)> headers)
        {
            //Tags
            var table = new TagBuilder("table");
            var tr = new TagBuilder("tr");

            //Add headers
            foreach (var s in headers)
            {
                var th = new TagBuilder("th");
                th.InnerHtml.Append(s.colName);
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
                    td.InnerHtml.Append(d.GetType().GetProperty(h.propertyName).GetValue(d, null)?.ToString());
                    tr.InnerHtml.AppendHtml(td);
                }

                table.InnerHtml.AppendHtml(tr);
            }

            return table;
        }
    }   
}
