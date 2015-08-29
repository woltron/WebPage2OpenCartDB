using System;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace ConsoleApplication2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var doc = new HtmlDocument();
            doc.Load(@"C:\exp\ibabbies\Детские игрушки\Детские игрушки_ интернет-магазин Kinderus.html",Encoding.UTF8);
//            doc.Load(@"C:\exp\ibabbies\Детская мебель и интерьер\Детская мебель и интерьер_ интернет-магазин Kinderus.html", Encoding.UTF8);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='prod']");
            for (var i=0; i<nodes.Count;i++)
            {
                var node = nodes[i];
                if (node.HasAttributes)
                {
                    var p_header = GetDivValue(node, "p_header");
                    var articul_i_block = GetDivValue(node, "articul i_block");
                    var stuff_i_block = GetDivValue(node, "stuff i_block");
                    var sizes_i_block = GetDivValue(node, "sizes i_block");
                    var products_link = GetDivValue(node, "products-link");
                    var brand = GetDivValue(node, "brand");
                    var price = GetDivValue(node, "price");

                    var result = 
                        p_header + "\n"+
                        articul_i_block + "\n" +
                        stuff_i_block + "\n" +
                        sizes_i_block + "\n"+
                        products_link+"\n"+
                        brand+"\n"+
                        price+"\n"
                        ;
                    Console.Write(result);
                }
            }
        }

        private static string GetDivValue(HtmlNode node, string mask)
        {
            foreach (var a in node.ChildNodes)
            {
                if ((a != null) && (a.Attributes.Count>0))
                {
                    foreach (var b in a.ChildNodes)
                    {
                        if ((b != null) && (b.Attributes.Count > 0))
                        {
                            var attr = b.Attributes[0];
                            if ((attr.Name == "class") && (attr.Value == mask))
                            {
                                return b.InnerHtml.Trim(new[] { '\n', ' ' });
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
