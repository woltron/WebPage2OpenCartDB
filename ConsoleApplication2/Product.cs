using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    public class Product
    {
        public int Id { get; private set; }
        public string Catalog { get; private set; }
        public string Name { get; private set; }
        public string Articul { get; private set; }
        public string Stuff { get; private set; }
        public string Size { get; private set; }
        public string Alt { get; private set; }
        public string Pic { get; private set; }
        public string Brand { get; private set; }
        public decimal Price { get; private set; }
        public decimal OldPrice { get; private set; }
        public string ValutaName { get; private set; }
        public int CatalogId { get; private set; }

        public Product(int id, string catalog, string name, string articul, string stuff, string size, string alt, string pic, string brand, decimal price, decimal oldPrice, string valutaName, int catalog_id)
        {
            Id = id;
            Catalog = catalog;
            Name = name;
            Articul = articul;
            Stuff = stuff;
            Size = size;
            Alt = alt;
            Pic = pic;
            Brand = brand;
            Price = price;
            OldPrice = oldPrice;
            ValutaName = valutaName;
            CatalogId = catalog_id;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
