using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        public ViewResult Index()//Index aksiyonu listelemek için kullanılır genelde,ve biz ViewResult yaptık Actiondı
        {
            //var model = db.Movies.ToList(); // satırı Getlist yapınca kapadık
            var model = GetList();
            //if (model == null) eğer filmlertablosu boşsa hata verirse ki tolist vermiyor aşağıyı uygula
            //{
            //    model = new List<Movie>();//Bu da nullsa boş bir liste göndercek
            //}
            ViewBag.count = model.Count;  //ındex cshmtle gönderir
            //ViewData["count"] = model.Count; yukarı ile aynı sonuç verir
            return View(model);//view ile döndürme yapılıyor,parametere gönderilmesi herzaman zorunlu değil
            //controllerden viewa veri taşımanın ilk yolu view içine model yazmak,model ile

        }
        [NonAction]//url üzerinden getlist aksiyonunu çağırmamak için
        public List<Movie> GetList(bool removeSession=true)
        {//session short yapmada ve sayfalama işlemlerinde kullanılmalı amaç performans artırmak için yedekleme için değil
            List<Movie> entities;
            if (removeSession)
            {
                Session.Remove("movies");
            }
            if (Session["movie"]==null || removeSession)
            {//session ise git verileri veri tabanından al
                entities = db.Movies.ToList();
                Session["movie"] = entities;//bu veriyide sessiona atmam lazım,memoryde tutarız sessiondan da almak için aynı zamanda
            }
            else
            {//değilse (sessiontemizle değilse) verileri git sessiondan al
                entities = Session["movies"] as List<Movie>;//casding işlemi yaptık list olması gerekiyor
            }
           
            return entities;
        }

        public ActionResult GetMoviesFromSession()//moviesleri memoryden getirme
        {
            var model = GetList(false);//sessiontemizleme olmasın çünkü verileri ondan alcaz
            ViewBag.count = model.Count;
            return View("Index", model);//tek bir viewde iki farklı aksiyomu kullandık
        }


        [HttpGet]//bunların adları actionmethodselector:hangi metot olduğunu belirtir
        //httpget yerine accept.Verbs içine get post vb de yazılır
        public ActionResult Add()//Addviewde göstercek bu get işlemini
        {
            ViewBag.Message = "Please enter movie information";//controllerden viewe bu yazıyı gönderiyoruz,ekleme olmadığı için post işlemi değil,viewbag ile taşırız
            //return View();  ya daaa;
            return new ViewResult();
        }


        [HttpPost]//ekleme kısmı postta yapılır,veritabanına ekleme yapar
        public RedirectToRouteResult Add(string Name,int ProductionYear,string BoxOfficeReturn)//formda eklenen veriler
        {
            var entity = new Movie()//movieye ekliyorum bu verileri
            {
                Name = Name,
                ProductionYear = ProductionYear.ToString(),//productionyear string tanımlamıştık
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",","."),CultureInfo.InvariantCulture)//paradaki , ü . ile değiştir
            };//noktaya çevirmemizin nedeni sunucu tarafında sıkıntı olmasın diye,biz nokta görmüyoruz ama.Sunucu kısmında 
            db.Movies.Add(entity);
            db.SaveChanges();
            TempData["Info"]= "Record successfully saved to database";//mesajı listelenen yerde yani indexte görcem
            //Temodata kullanma sebebi viewbag yada data yapınca göstermedi çünkü burda return view yerine yönlenidirme yaptık add'den indexe gittiği için gözükmedi
            return RedirectToAction("Index");//ekleyince Indexe yönlendiriyor listenin güncelini görmek için istek
        }

        [HttpGet]//yazmasamda yine get bu,bunun post olanını yazcaz add metodundki gibi
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)//id göndermeyi unuttuk yada silindi diyelim garantiye aldık
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Id is required!");//id yoksa notfound 401 gibi bunu döneriz
            }
            var model = db.Movies.Find(id.Value);
            List<SelectListItem> years = new List<SelectListItem>();//dropdowna yılları çekmek için
            SelectListItem year;
            for(int i = DateTime.Now.Year; i >= 2000; i--)
            {
                year = new SelectListItem() { Value= i.ToString(), Text= i.ToString() };//dropdownın value ve texti var
                years.Add(year);//dropdowmliste ekledik bunuda viewbag ile dropdowndan çekip göndercez
            }
            //ProductionYear ile seçili idnin satırının yılı dolu geliyor
            ViewBag.Years = new SelectList(years,"Value","Text",model.ProductionYear);//edit cshtmle gönderir,selectlist dropdownlisti beslicek
            return View(model);
        }

        [HttpPost]//kaydetme veritabanına
        public ActionResult Edit([Bind(Include="Id,Name,ProductionYear")]Movie movie,string BoxOfficeReturn)//movie nin propları gelcek name,yıl vb. tek tek yazmadık
        {//boxofficereturnı gönderdik çünkü değeri noktalı yazınca null atıyor,metotta bind kullanmasamda olurdu zaten 3 alan movie ile gelcekti
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);//where yerine
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            entity.BoxOfficeReturn =Convert.ToDouble(BoxOfficeReturn.Replace(",","."),CultureInfo.InvariantCulture);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToRoute(new { controller = "Movies", action = "Index" });
        }

        [HttpGet]//bişey döncez post değil ekleme kayıt yok
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            var model = db.Movies.FirstOrDefault(e => e.Id == id.Value);
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = db.Movies.Find(id);
            db.Movies.Remove(entity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
