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
        private static readonly Dictionary<string, string[]> kinderusCatalogs = new Dictionary<string, string[]>()
        {
            {"toys",new[]{"data/Kinderus/KidsToys","1",@"C:\exp\ibabbies\Детские игрушки\Детские игрушки_ интернет-магазин Kinderus.html" }},
            {"mebel",new[]{"data/Kinderus/ChildrensFurnitureAndInterior","2",@"C:\exp\ibabbies\Детская мебель и интерьер\Детская мебель и интерьер_ интернет-магазин Kinderus.html" }},
            {"kolyaski",new[]{"data/Kinderus/StrollersAndCarSeats","3",@"C:\exp\ibabbies\Коляски и автокресла\Коляски и автокресла_ интернет-магазин Kinderus.html" }},
            {"green",new[]{"data/Kinderus/ClothesForBabies","4",@"C:\exp\ibabbies\Купить детскую одежду для малышей\Купить детскую одежду для малышей в интернет-магазине Санкт-Петербурга._ интернет-магазин Kinderus.html" }},
            {"purple",new[]{"data/Kinderus/ClothingForGirls","5",@"C:\exp\ibabbies\Одежда для девочек\Одежда для девочек_ интернет-магазин Kinderus.html" }},
            {"blue",new[]{"data/Kinderus/ClothesForBoys","6",@"C:\exp\ibabbies\Одежда для мальчиков\Одежда для мальчиков_ интернет-магазин Kinderus.html" }},
            {"mom",new[]{"data/Kinderus/ClothesForMoms","7",@"C:\exp\ibabbies\Одежда для мам\Одежда для мам_ интернет-магазин Kinderus.html" }},
            {"newborn",new[]{"data/Kinderus/ChildrensClothingForKids","8",@"C:\exp\ibabbies\Одежда для новорожденных\Одежда для новорожденных_ интернет-магазин Kinderus.html" }},
            {"gigiena",new[]{"data/Kinderus/DiapersAndHygiene","9",@"C:\exp\ibabbies\Подгузники и гигиена\Подгузники и гигиена_ интернет-магазин Kinderus.html" }},
            {"sport",new[]{"data/Kinderus/SportsAndRecreation","10",@"C:\exp\ibabbies\Спорт и отдых\Спорт и отдых_ интернет-магазин Kinderus.html" }},
            {"school",new[]{"data/Kinderus/GoodsForStudents","11",@"C:\exp\ibabbies\Товары для школьников\Товары для школьников_ интернет-магазин Kinderus.html" }},
        
        };
        private static readonly Dictionary<string, string[]> kinderus = new Dictionary<string, string[]>()
        {
            {"toys",new[]{"Игрушки","Toys"}},
            {"mebel",new[]{"Мебель","Furniture"}},
            {"kolyaski",new[]{"Коляски","Stroller"}},
            {"green",new[]{"Малыши","Babies"}},
            {"purple",new[]{"Девочки","Girls"}},
            {"blue",new[]{"Мальчики","Boys"}},
            {"mom",new[]{"Мамы","Mom"}},
            {"newborn",new[]{"Новорожденные","Newborn"}},
            {"gigiena",new[]{"Гигиена","Hygiene"}},
            {"sport",new[]{"Спорт","Sport"}},
            {"school",new[]{"Школьники","Student"}},
        };

        
        [STAThread]
        static void Main(string[] args)
        {
            var list = new List<Product>();
            var doc = new HtmlDocument();
            var storeId = 0;//номер магазина
            //doc.Load(@"C:\exp\ibabbies\Детские игрушки\Детские игрушки_ интернет-магазин Kinderus.html",Encoding.UTF8);
            //doc.Load(@"C:\exp\ibabbies\Детская мебель и интерьер\Детская мебель и интерьер_ интернет-магазин Kinderus.html", Encoding.UTF8);
            var fileStream = new StreamWriter("C:/exp/1.sql", false);
            fileStream.WriteLine();
            fileStream.WriteLine("SET @row_number = 0;");
            fileStream.WriteLine("SET @cnt = 0;");

            fileStream.WriteLine("--SQL-загрузка недостающих производителей");
            fileStream.WriteLine("delete from oc_manufacturer;");

            foreach (var cats in kinderusCatalogs.Values)
            {
                doc.Load(cats[2], Encoding.UTF8);

                #region загрузка продуктов

                var catalogs = doc.DocumentNode.SelectNodes("//div[@id='catalog']");
                var catalog = catalogs[0].Attributes[1].Value;
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='prod']");
                for (var i = 0; i < nodes.Count; i++)
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
                            var file = kinderusCatalogs[catalog][0] + "/" + files[files.Length - 1];

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
                            list.Add(new Product(id, catalog, name, articul, stuff, size, alt, file, brand.Key,
                                priceCurrent,
                                priceOld, price.Value, int.Parse(cats[1])));
                        }

                    }
                    catch (Exception e)
                    {
                        //
                    }
                }

                #endregion

            }

            #region SQL-загрузка недостающих производителей

                foreach (var rez in list.GroupBy(value => value.Brand).OrderBy(value => value.Key))
                {
                    fileStream.WriteLine(
                        string.Format(
                            "INSERT INTO oc_manufacturer (`manufacturer_id`, `name`, `image`, `sort_order`) SELECT (select max(manufacturer_id)+1 from oc_manufacturer), '{0}', '', 0 FROM DUAL WHERE (select count(*) from oc_manufacturer where name ='{0}')=0;",
                            rez.Key.Replace("'", "`")));
                }

                #endregion

            #region SQL-загрузка недостающих категорий

            fileStream.WriteLine();
            fileStream.WriteLine("--SQL-загрузка недостающих категорий");
            //fileStream.WriteLine("delete from oc_category_description;");
            //fileStream.WriteLine("delete from oc_category_path;");
            //fileStream.WriteLine("delete from oc_category_to_store;");
            //fileStream.WriteLine("delete from oc_category;");
            int ii = 0;
            foreach (var cat in kinderus)
            {
                ii++;
                //fileStream.WriteLine(string.Format("INSERT INTO oc_category (`category_id`, `image`, `parent_id`, `top`, `column`, `sort_order`, `status`, `date_added`, `date_modified`) SELECT {0}, '', 0, 1, 1,{0}, 1,'2015-09-01','2015-09-01'  FROM DUAL;", ii));
                //fileStream.WriteLine(string.Format("INSERT INTO oc_category_to_store (`category_id`, `store_id`) SELECT {0}, 0 FROM DUAL;", ii));
                //fileStream.WriteLine(string.Format("INSERT INTO oc_category_path (`category_id`, `path_id`, `level`) SELECT {0}, {0}, 1 FROM DUAL;", ii));
                //fileStream.WriteLine(string.Format("INSERT INTO oc_category_description (`category_id`, `language_id`, `name`, `description`, `meta_description`, `meta_keyword`, `seo_title`, `seo_h1`) SELECT {0}, 1, '{1}', '', '','','',''  FROM DUAL;", ii, cat.Value[0].Replace("'", "`")));
                //fileStream.WriteLine(string.Format("INSERT INTO oc_category_description (`category_id`, `language_id`, `name`, `description`, `meta_description`, `meta_keyword`, `seo_title`, `seo_h1`) SELECT {0}, 2, '{1}', '', '','','',''  FROM DUAL;", ii, cat.Value[1].Replace("'", "`")));
            }

            #endregion

            #region product

            fileStream.WriteLine();
            fileStream.WriteLine("--SQL-загрузка недостающих продуктов");
            fileStream.WriteLine("SET @manufacturer_id=0;");
            //fileStream.WriteLine("delete from oc_product_description;");
            //fileStream.WriteLine("delete from oc_product;");
            //fileStream.WriteLine("delete from oc_product_to_category;");
            //fileStream.WriteLine("delete from oc_product_to_store;");
            foreach (var pr in list)
            {
                fileStream.WriteLine(
                    string.Format(
                        "select @manufacturer_id:=manufacturer_id from oc_manufacturer where name = '{0}';",
                        pr.Brand.Replace("'", "`")));
                fileStream.WriteLine(
                    string.Format(
                        "INSERT INTO oc_product_to_category (`product_id`,`category_id`, `main_category`) SELECT {0}, {1}, {1}  FROM DUAL where not exists(select * from oc_product_to_category where product_id={0} and category_id={1});",
                        pr.Id, pr.CatalogId));
                fileStream.WriteLine(
                    string.Format(
                        "INSERT INTO oc_product_to_store (`product_id`,`store_id`) SELECT {0}, {1}  FROM DUAL where not exists(select * from oc_product_to_store where product_id={0} and store_id={1});",
                        pr.Id, storeId));
                fileStream.WriteLine(
                    "INSERT INTO `oc_product_description` (`product_id`, `language_id`, `name`, `description`, `meta_description`, `meta_keyword`, `seo_title`, `seo_h1`, `tag`) " +
                    string.Format(
                        "select '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}' from dual where not exists(select * from oc_product_description where product_id={0} and language_id={1});",
                        pr.Id, 1, pr.Name.Replace("'", "`"), "", "", "", "", "", ""));
                fileStream.WriteLine(
                    "INSERT INTO `oc_product_description` (`product_id`, `language_id`, `name`, `description`, `meta_description`, `meta_keyword`, `seo_title`, `seo_h1`, `tag`) " +
                    string.Format(
                        "select '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}' from dual where not exists(select * from oc_product_description where product_id={0} and language_id={1});",
                        pr.Id, 2, pr.Name.Replace("'", "`"), "", "", "", "", "", ""));
                fileStream.WriteLine(
                    "INSERT INTO `oc_product` (`product_id`, `model`, `sku`, `upc`, `ean`, `jan`, `isbn`, `mpn`, `location`, `quantity`, `stock_status_id`, `image`, `manufacturer_id`, `shipping`, `price`, `points`, `tax_class_id`, `date_available`, `weight`, `weight_class_id`, `length`, `width`, `height`, `length_class_id`, `subtract`, `minimum`, `sort_order`, `status`, `date_added`, `date_modified`, `viewed`)" +
                    string.Format(
                        "select  '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}' , {9}, {10}, '{11}', {12}, {13}, {14}, '{15}', '{16}', '{17}', '{18}', {19}, '{20}', '{21}', '{22}', {23}, '{24}', '{25}', '{26}', {27}, '{28}', '{29}', '{30}' from dual where not exists(select * from oc_product where product_id={0});",
                        pr.Id, kinderus[pr.Catalog][0].Replace("'", "`"), pr.Catalog.Replace("'", "`"), "", "", "",
                        "", "", "", "1", "7", pr.Pic, "@manufacturer_id", "1", pr.Price, "", "", "2015-09-01", "",
                        "1", "", "", "", "1", "", "", "", "1", "2015-09-01", "2015-09-01", ""));

                //Console.Write(product);
            }

             #endregion
            
            fileStream.Flush();
           
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
