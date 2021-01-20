using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShoppingCart.Data;
using ShoppingCart.Helper;
using ShoppingCart.Models;
using System.Net.Http.Formatting;

namespace ShoppingCart.Controllers
{
    [Authorize]

    public class ProductsController : Controller
    {
        ProductsAPI _API = new ProductsAPI();
        string _apiDomain = string.Empty;
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            return View(products);
        }


        [HttpGet]
        public async Task<IActionResult> Index(string ProductSearch)
        {
            ViewData["GetProductDetails"] = ProductSearch;
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            var sql = from a in products select a;
            if (!string.IsNullOrEmpty(ProductSearch))
            {
                sql = sql.Where(a => a.ProductName.StartsWith(ProductSearch)/*||a.LastName.Contains(PersonSearch)|| a.Email.Contains(PersonSearch)*/);
            }
            return View(sql.ToList());
        }

        public async Task<IActionResult> Shop()
        {
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            var vie = products.OrderBy(d => d.ProductName).ToList();
            return View(vie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(IFormCollection form)
        {
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            var qty = int.Parse(form["Qty"]);
            var productId = int.Parse(form["ProductId"]);
            var price = decimal.Parse(form["Price"]);
            IList<Cart> carts = new List<Cart>();
            var data = products.Where(d => d.ProductId == productId).FirstOrDefault();
            if (data != null)
            {
                data.ProductQuantity = data.ProductQuantity - qty;
                _context.SaveChanges();
            }
            carts = GetCart();
            carts.Add(new Cart()
            {
                Price = price,
                ProductId = productId,
                Qty = qty
            });
            var cartString = JsonConvert.SerializeObject(carts);
            HttpContext.Session.SetString("MyCart", cartString);
            return RedirectToAction("Shop");
        }

        private IList<Cart> GetCart()
        {
            var myCartString = HttpContext.Session.GetString("MyCart");
            IList<Cart> carts = new List<Cart>();
            if (myCartString != null)
            {
                carts = (IList<Cart>)JsonConvert.DeserializeObject(myCartString, typeof(IList<Cart>));

            }
            return carts;
        }
        public async Task<IActionResult> Cart()
        {
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            var myCart = GetCart();
            if (myCart.Count > 0)
            {
                foreach (var item in myCart)
                {
                    var thisProduct = products.Find(u => u.ProductId == item.ProductId);
                    item.ProductName = thisProduct.ProductName;
                    item.FileImage = thisProduct.FileImage;
                }
            }
            return View(myCart);
        }
        public IActionResult Checkout()
        {
            var myCart = GetCart();
            var orderAmount = myCart.Sum(d => (d.Price * d.Qty));
            Order order = new Order()
            {
                OrderAmount = orderAmount,
                OrderDate = DateTime.Now,
                UserID = User.Identity.Name
            };
            _context.Add(order);
            _context.SaveChanges();

            foreach (var item in myCart)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    OrderId = order.OrderId,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Qty
                };
            }
            HttpContext.Session.Remove("MyCart");
            return View(order);
        }
        public IActionResult MyOrders()
        {
            var order = _context.Order.Where(d => d.UserID == User.Identity.Name).ToList();
            return View(order);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _context.Order.Find(id);
            var orderDetails = _context.orderDetails.Include(d => d.product).Where(d => d.OrderId == id).ToList();
            ViewBag.OrderDetails = orderDetails;
            return View(order);
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            List<Product> products = new List<Product>();
            HttpClient client = _API.Initial();
            HttpResponseMessage message = await client.GetAsync("api/Products");
            if (message.IsSuccessStatusCode)
            {
                var results = message.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(results);
            }
            if (id == null)
            {
                return NotFound();
            }

            var product =  products.FirstOrDefault(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //// GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductQuantity,ProductPrice,FileImage")] Product product, IFormFile FileImage)
        {
            HttpClient client = _API.Initial();
            var post = client.PostAsJsonAsync<Product>("api/Products",product);
            post.Wait();

            var result = post.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            //if (ModelState.IsValid)
            //{
            //    if (FileImage.Length > 0)
            //    {

            //        string path = Environment.CurrentDirectory;
            //        string fullName = Path.Combine(path, "wwwroot", "Images", FileImage.FileName);

            //        using (var stream = System.IO.File.Create(fullName))
            //        {
            //            await FileImage.CopyToAsync(stream);
            //        }
            //        product.FileImage = FileImage.FileName;
            //        _context.Add(product);
            //    }
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return View();
        }

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Product.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductQuantity,ProductPrice,FileImage")] Product product)
        //{
        //    if (id != product.ProductId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.ProductId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        //// GET: Products/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Product
        //        .FirstOrDefaultAsync(m => m.ProductId == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Product.FindAsync(id);
        //    _context.Product.Remove(product);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ProductExists(int id)
        //{
        //    return _context.Product.Any(e => e.ProductId == id);
        //}
    }
}
