using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HtmlAgilityPack;

namespace ConsoleApplication2
{
    class Program
    {
        private static readonly Dictionary<string, string> kinderusCatalogs = new Dictionary<string, string>()
        {
            {"toys","./image/data/Kinderus/KidsToys"},
            {"mebel","./image/data/Kinderus/ChildrensFurnitureAndInterior"},
            {"kolyaski","./image/data/Kinderus/StrollersAndCarSeats"},
            {"green","./image/data/Kinderus/ClothesForBabies"},
            {"purple","./image/data/Kinderus/ClothingForGirls"},
            {"blue","./image/data/Kinderus/ClothesForBoys"},
            {"mom","./image/data/Kinderus/ClothesForMoms"},
            {"newborn","./image/data/Kinderus/ChildrensClothingForKids"},
            {"gigiena","./image/data/Kinderus/DiapersAndHygiene"},
            {"sport","./image/data/Kinderus/SportsAndRecreation"},
            {"school","./image/data/Kinderus/GoodsForStudents"},
        
        };

        
        [STAThread]
        static void Main(string[] args)
        {
            var list = new List<Product>();
            var doc = new HtmlDocument();
            doc.Load(@"C:\exp\ibabbies\Детские игрушки\Детские игрушки_ интернет-магазин Kinderus.html",Encoding.UTF8);
//            doc.Load(@"C:\exp\ibabbies\Детская мебель и интерьер\Детская мебель и интерьер_ интернет-магазин Kinderus.html", Encoding.UTF8);

            var catalogs = doc.DocumentNode.SelectNodes("//div[@id='catalog']");
            var catalog = catalogs[0].Attributes[1].Value;
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='prod']");
            for (var i=0; i<nodes.Count;i++)
            {
                try
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

                        var files = products_link.Value.Split('/');
                        var file = kinderusCatalogs[catalog] + "/" + files[files.Length - 1];

                        var id = int.Parse(products_link.Key.Split(':')[0]);
                        var name = p_header.Key;
                        var articul = articul_i_block.Value;
                        var stuff = stuff_i_block.Value;
                        var size = sizes_i_block.Value;
                        var alt = products_link.Key.Split(':')[1];
                        var pricies = price.Key.Split(':');
                        Decimal priceCurrent = Decimal.Parse(pricies.Length > 1 ? pricies[1] : pricies[0]);
                        Decimal priceOld = pricies.Length > 1 ? Decimal.Parse(pricies[0]) : Decimal.Zero;


                        var result =

                            string.Format("Id={0}\n", id) +
                            string.Format("Catalog={0}\n", catalog) +
                            string.Format("Наименование: {0}\n", name) +
                            string.Format("{0} {1}\n", articul_i_block.Key, articul) +
                            string.Format("{0} {1}\n", stuff_i_block.Key, stuff) +
                            string.Format("{0} {1}\n", sizes_i_block.Key, size) +
                            string.Format("Альтернатива картинке: {0}\n", alt) +
                            string.Format("Файл картинки: {0}\n", file) +
                            string.Format("Брэнд: {0}\n", brand.Key) +

                            string.Format("Цена текущая: {0} {1}\n", priceCurrent, price.Value) +
                            ((priceOld != Decimal.Zero)
                                ? string.Format("Цена старая: {0} {1}\n", priceOld, price.Value)
                                : "") + "\n";

                        //Console.Write(result);
                        list.Add(new Product(id, catalog, name, articul, stuff, size, alt, file, brand.Key, priceCurrent,
                            priceOld, price.Value));
                    }

                }
                catch (Exception e)
                {
                    //
                }
            }
            foreach(var rez in list.GroupBy(value => value.Brand).OrderBy(value=>value.Key))
            {
                 Console.WriteLine(rez.Key);
               
            }
            foreach (var product in list)
            {
                Console.Write(product);
            }
        }

        private static KeyValuePair<string, string> GetDivValue(HtmlNode node, string mask)
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
                                if (b.ChildNodes.Count > 1)
                                {
                                    foreach (var c in b.ChildNodes)
                                    {
                                        if ((c.Name == "span"))
                                        {
                                            if (c.ChildNodes.Count > 1)
                                            {
                                                return
                                                    new KeyValuePair<string, string>(
                                                        c.ChildNodes[0].InnerHtml.Trim(new[] { '\n', ' ' })+":"+
                                                        c.ChildNodes[2].ChildNodes[0].InnerHtml.Trim(new[] { '\n', ' ' }),
                                                        c.ChildNodes[2].ChildNodes[1].InnerHtml.Trim(new[] { '\n', ' ' }));
                                            }
                                            return
                                                new KeyValuePair<string, string>(
                                                    c.PreviousSibling.InnerHtml.Trim(new[] {'\n', ' '}),
                                                    c.InnerHtml.Trim(new[] {'\n', ' '}));
                                        }
                                        if ((c.Name == "img"))
                                        {
                                            var split = c.ParentNode.Attributes[1].Value.Split('/');
                                            var id = split[split.Length - 1];
                                            return
                                                new KeyValuePair<string, string>(id+":"+
                                                    c.Attributes[1].Value.Trim(new[] { '\n', ' ' }),
                                                    c.Attributes[0].Value.Trim(new[] { '\n', ' ' }));
                                        }
                                    }

                                }
                                return new KeyValuePair<string, string>(b.InnerHtml.Trim(new[] { '\n', ' ' }), "");
                            }
                        }
                    }
                    }
                }
            return new KeyValuePair<string, string>("","") ;
        }
    }
}
