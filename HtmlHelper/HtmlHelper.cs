using Microsoft.AspNetCore.Html;        
using Microsoft.AspNetCore.Mvc.Rendering;
using OrdersManager.Components;
using OrdersManager.Models;

using System.Text;
using System.Text.Encodings.Web;

namespace HtmlHelper
{
    public static class ListHelper
    {

        public static IHtmlContent TableOrderItems(this IHtmlHelper helper, List<OrderItemModel> listOfItems, List<string> headers)
        {
            //Tags
            var table = new TagBuilder("table");
            var tr = new TagBuilder("tr");

            //Add headers
            foreach (var header in headers)
            {
                var th = new TagBuilder("th");
                th.InnerHtml.Append(header);
                tr.InnerHtml.AppendHtml(th);
            }

            table.InnerHtml.AppendHtml(tr);
            //Add data
            foreach (var item in listOfItems)
            {                
                tr = new TagBuilder("tr");

                var td1 = new TagBuilder("td");
                td1.InnerHtml.Append(item.Name);
                tr.InnerHtml.AppendHtml(td1);

                var td2 = new TagBuilder("td");
                td2.InnerHtml.Append(item.Quantity.ToString());
                tr.InnerHtml.AppendHtml(td2);

                var td3 = new TagBuilder("td");
                td3.InnerHtml.Append(item.Unit);
                tr.InnerHtml.AppendHtml(td3);

                table.InnerHtml.AppendHtml(tr);
            }

            return table;
        }

    }
}
