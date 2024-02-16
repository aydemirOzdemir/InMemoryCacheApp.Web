using InMemoryCacheApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCacheApp.Web.Controllers;

public class ProductController : Controller
{
    private readonly IMemoryCache memoryCache;

    public ProductController(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }
    public IActionResult Index()
    {
        if (String.IsNullOrEmpty(memoryCache.Get<string>("zaman")))
        {
            memoryCache.Set<string>("zaman", DateTime.Now.ToString());
        }


        if(!memoryCache.TryGetValue("zaman",out string zamanCache))
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration=DateTime.Now.AddSeconds(30),
                Priority=CacheItemPriority.High,
                
            };
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                memoryCache.Set<string>("calback", $"{key}=>{value} sebep:{state}");
            });


            memoryCache.Set<string>("zaman", DateTime.Now.ToString(),options);
        }
        
        
        return View();
    }




    public IActionResult ProductMemory()
    {
        Product product = new Product
        {
            Id = 1,
            Name = "Kalem",
            Price = 100
        };
        memoryCache.Set("product1",product);
        return View();
    }


    public IActionResult Show()
    {


        memoryCache.Remove("zaman");
        memoryCache.GetOrCreate<string>("zaman", entry =>
        {
          return  DateTime.Now.ToString();
        });
        ViewBag.zaman = memoryCache.Get<string>("zaman");


        ViewBag.product = memoryCache.Get<Product>("product1");
        return View();
    }
}
