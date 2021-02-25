﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bioskop.Domen;
using Bioskop.Podaci.UnitOfWork;
using Bioskop.WebApp.Filters;
using Bioskop.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Web.Helpers;

namespace Bioskop.WebApp.Controllers
{
  // moze da se stavi na njivou kontrolera
    public class FilmController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public FilmController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Film
        [NotLoggedIn]
        public ActionResult Index()
        {
            List<Film> filmovi = unitOfWork.Film.VratiSve();
            int? userid = HttpContext.Session.GetInt32("userid");
            if (userid != null)
            {
                ViewBag.IsLoggedIn = true;
                ViewBag.Username = HttpContext.Session.GetString("username");
                //byte[] userBy = HttpContext.Session.Get("user"); // user je key
                //Korisnik k = JsonSerializer.Deserialize<Korisnik>(userBy); nece 22:43
                // za slucaj korpe je pozeljno koristiti serijalizaciju/deserijalizaciju objekta

            }
            else
            {
                //return RedirectToAction("Login", "Korisnik");
                return View("Index", filmovi);
            }
            return View("Index", filmovi);
        }

        // eksplicitno naglasavanje da dolazi iz rute.. [FromRoute] 
        [NotLoggedIn]
        public ActionResult Details([FromRoute] int id) {

            Film model = unitOfWork.Film.NadjiPoId(id);
            ViewBag.IsLoggedIn = true;
            ViewBag.Username = HttpContext.Session.GetString("username");
            //model.PutanjaBackPostera = model.PutanjaBackPostera.Replace("//")
            return View(model);
        }
        [LoggedInKorisnik]
        [HttpGet]
        public ActionResult Create() {
            //ViewData["Zanr"] = unitOfWork.Zanr.GetAll(); za uvodjenje dodatnih modela
            List<Sala> list = unitOfWork.Sala.VratiSve();
            ViewBag.IsLoggedIn = true;
            ViewBag.Username = HttpContext.Session.GetString("username");
            List<SelectListItem> selectList = list.Select(s => new SelectListItem { Text = s.NazivSale, Value = s.SalaId.ToString() }).ToList();
            CreateFilmViewModel model = new CreateFilmViewModel
            {
                Sale = selectList
            };

            return View(model);
        }

        // da posmatra kao podatke sa forme
        // za JSON [FromBody
        [LoggedInKorisnik]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateFilmViewModel viewModel)
        {
            try
            {
                //string fileName1 = Path.GetFileNameWithoutExtension(viewModel.Film.ImageFilePoster.FileName);
                //string fileName2 = Path.GetFileNameWithoutExtension(viewModel.Film.ImageFileBackPoster.FileName);
                //string extension1 = Path.GetExtension(viewModel.Film.ImageFilePoster.FileName);
                //string extension2 = Path.GetExtension(viewModel.Film.ImageFileBackPoster.FileName);
                //fileName1 = fileName1 + DateTime.Now.ToString() + extension1;
                //fileName2 = fileName2 + DateTime.Now.ToString() + extension2;
                //viewModel.Film.PutanjaPostera = "../img/upload-img/" + fileName1;
                //viewModel.Film.PutanjaBackPostera = "../img/upload-img/" + fileName2;
                //fileupload.SaveAs(Path.Combine(Server.MapPath("~/Media"), fileuploadPimage.FileName);
                //viewModel.Film.ImageFilePoster.SaveAs(Server.MapPath("../img/upload-img/"), fileName1);
                if (viewModel.Film.Naziv == null || viewModel.Film.OpisFilma == null || viewModel.Film.PutanjaBackPostera == null
                    || viewModel.Film.PutanjaPostera == null || viewModel.Film.Trajanje == 0) throw new Exception();
                unitOfWork.Film.Dodaj(viewModel.Film);
                unitOfWork.Commit();
                return RedirectToAction("Index", "Film");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Morate unit sva polja");
                return RedirectToAction("Create");
            }


            //if (!ModelState.IsValid) {
            //    return View("Create");
            //}


            //bool exist = unitOfWork.Film.Search(f => f.Naziv == film.Naziv).Any();

            //if (exist)
            //{
            //    ModelState.AddModelError("FilmName", "Film sa ovim nazivom vec postoji");
            //    return View("Create");
            //}
            //unitOfWork.Film.Dodaj(film);
            //unitOfWork.Commit();
            //return Index();
        }

        [LoggedInKorisnik]
        [HttpPost]
        public ActionResult AddSala(ProjekcijaViewModel request) {
            Sala s = unitOfWork.Sala.NadjiPoId(request.SalaId);
            ProjekcijaViewModel model = new ProjekcijaViewModel
            {
                Num = request.Num,
                SalaId = request.SalaId,
                NazivSale = s.NazivSale
            };
            return PartialView("ProjekcijaPartial",model);
        }

        [NotLoggedIn]
        /*  public ActionResult Delete([FromRoute] int id)
          {
              Film model = unitOfWork.Film.NadjiPoId(id);
              ViewBag.IsLoggedIn = true;
              ViewBag.Username = HttpContext.Session.GetString("username");
              List<Projekcija> projekcije = new List<Projekcija>();
              projekcije = unitOfWork.Projekcija.VratiSve();
              foreach (Projekcija p in projekcije)
              {
                  if (p.FilmId == id)
                  {
                      unitOfWork.Projekcija.Delete(p);
                  }
              }

              unitOfWork.Film.Delete(model);
              unitOfWork.Commit();
              return RedirectToAction("Index");
          }*/
        public ActionResult Delete([FromRoute] int id)
        {
            Film model = unitOfWork.Film.NadjiPoId(id);
            ViewBag.IsLoggedIn = true;
            ViewBag.Username = HttpContext.Session.GetString("username");

            List<Projekcija> projekcije = new List<Projekcija>();

            projekcije = unitOfWork.Projekcija.VratiSve();
            unitOfWork.Projekcija.izbrisiSvePFilm(id, projekcije);


            unitOfWork.Film.Delete(model);
            unitOfWork.Commit();
            return RedirectToAction("Index");
        }

    }
}