﻿using Bioskop.Domen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bioskop.Podaci.Implementacija
{
    public class RepositoryProjekcija : IRepositoryProjekcija
    {
        private BioskopContext context;

        public RepositoryProjekcija(BioskopContext context)
        {
            this.context = context;
        }
        public void Commit()
        {
            context.SaveChanges();
        }

        public void Delete(Projekcija s)
        {
            context.Projekcija.Remove(s);
        }

        public void Dodaj(Projekcija s)
        {
            context.Projekcija.Add(s);
        }

        public Projekcija NadjiPoId(int id)
        {
            return context.Projekcija.Find(id);
        }

        public void Update(Projekcija s)
        {
            throw new NotImplementedException();
        }

        public List<Projekcija> VratiSve()
        {
            return context.Projekcija.ToList();
        }
    }
}
