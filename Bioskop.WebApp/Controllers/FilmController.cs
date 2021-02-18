using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bioskop.Domen;
using Bioskop.Podaci.UnitOfWork;
using Bioskop.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bioskop.WebApp.Controllers
{
    public class FilmController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public FilmController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Film
        public ActionResult Index()
        {
            List<Film> filmovi = unitOfWork.Film.VratiSve();
            return View("Index",filmovi);
        }
        public ActionResult DetaljiFilma()
        {
            return View();
        }

        // eksplicitno naglasavanje da dolazi iz rute.. [FromRoute] 
        public ActionResult Details([FromRoute] int id) {

            Film model = unitOfWork.Film.NadjiPoId(id);
            return View(model);
        }
        [HttpGet]
        public ActionResult Create() {
            //ViewData["Zanr"] = unitOfWork.Zanr.GetAll(); za uvodjenje dodatnih modela
            List<Sala> list = unitOfWork.Sala.VratiSve();
            List<SelectListItem> selectList = list.Select(s => new SelectListItem { Text = s.NazivSale, Value = s.SalaId.ToString() }).ToList();
            CreateFilmViewModel model = new CreateFilmViewModel
            {
                Sale = selectList
            };

            return View(model);
        }

        // da posmatra kao podatke sa forme
        // za JSON [FromBody
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm]Film film)
        {
            if (!ModelState.IsValid) {
                return View("Create");
            }


            bool exist = unitOfWork.Film.Search(f => f.Naziv == film.Naziv).Any();

            if (exist)
            {
                ModelState.AddModelError("FilmName", "Film sa ovim nazivom vec postoji");
                return View("Create");
            }
            unitOfWork.Film.Dodaj(film);
            unitOfWork.Commit();
            return Index();
        }
    }
}