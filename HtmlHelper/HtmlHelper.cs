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

        public static IHtmlContent Table<T>(this IHtmlHelper helper, List<T> data, List<(string propertyName, string colName)> headers, IHtmlContent content = null)
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
            int id = 0;
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
                if(content != null)
                {
                    var tdButton = new TagBuilder("td");
                    TagBuilder form = new TagBuilder("form");
                    form.Attributes.Add("action", "DeleteOrderItem");
                    form.Attributes.Add("method", "post");

                    TagBuilder input = new TagBuilder("input");
                    input.Attributes.Add("type", "submit");
                    input.Attributes.Add("value", "Удалить");

                    TagBuilder inputId = new TagBuilder("input");
                    inputId.Attributes.Add("type", "hidden");
                    inputId.Attributes.Add("name", "Id");
                    inputId.Attributes.Add("value", id.ToString());
                    
                    form.InnerHtml.AppendHtml(input);
                    form.InnerHtml.AppendHtml(inputId);

                    form.InnerHtml.AppendHtml(content);

                    tdButton.InnerHtml.AppendHtml(form);
                    tr.InnerHtml.AppendHtml(tdButton); 
                }

                id++;
                table.InnerHtml.AppendHtml(tr);
            }

            return table;
        }

    }
}
