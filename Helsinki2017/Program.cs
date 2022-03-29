using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helsinki2017
{
    class Program
    {
        static readonly List<Versenyzo> eredmenyek = new List<Versenyzo>();
        static void Main()
        {
            //1. feladat
            FelDolgoz(FajlBeolvas("rovidprogram.csv"), Tipus.Rovid);
            FelDolgoz(FajlBeolvas("donto.csv"), Tipus.Donto);

            //2. feladat
            Console.WriteLine("2. feladat");
            Console.WriteLine("\tA rövidprogramban {0} induló volt", eredmenyek.Count(v=>v.tipus==Tipus.Rovid));

            //3. feladat
            Console.WriteLine("3. feladat");
            Console.WriteLine("\tA magyar versenyző {0} a kűrbe.", (eredmenyek.Any(v => v.tipus==Tipus.Donto && v.orszagkod == "HUN") ? "bejutott" : "nem jutott be"));

            //5. feladat
            Console.WriteLine("5. feladat");
            Console.Write("\tKérem a versenyző nevét: ");
            string nev = Console.ReadLine();
            bool letezo = true;
            if (eredmenyek.Count(v => v.nev == nev) == 0) {
                Console.WriteLine("\tIlyen nevű induló nem volt!");
                letezo = false;
            }

            //6. feladat
            if (letezo)
            {
                Console.WriteLine("6. feladat");
                Console.WriteLine("\tA versenyző összpontszáma: {0}", OsszPontszam(nev));
            }

            //7. feladat
            Console.WriteLine("7. feladat");
            eredmenyek.Where(v => v.tipus == Tipus.Donto).Select(v => v.orszagkod).Distinct().ToList().ForEach(o => {
                int indulo = eredmenyek.Where(v => v.tipus == Tipus.Donto && v.orszagkod == o).Count();
                if (indulo > 1)
                {
                    Console.WriteLine("\t{0}: {1} versenyző",o,indulo);
                }
            });

            //8. feladat
            List<Versenyzo> uj = new List<Versenyzo>();
            eredmenyek.Where(v => v.tipus == Tipus.Donto).ToList().ForEach(v =>
            {
                v.osszPontszam = OsszPontszam(v.nev);
                uj.Add(v);
            });
            using(var f = File.Open("vegeredmeny.csv",FileMode.Create))
            using(var sw = new StreamWriter(f))
            {
                int helyezes = 1;
                uj.Where(v => v.tipus == Tipus.Donto).OrderByDescending(v=>v.osszPontszam).ToList().ForEach(v =>
                {
                    sw.WriteLine($"{helyezes++};{v.nev};{v.orszagkod};{v.osszPontszam}");
                });
            }

            Console.ReadKey();
        }
        static List<string[]> FajlBeolvas(string fajlnev)
        {
            List<string[]> rtn = new List<string[]>();
            foreach (var sor in File.ReadAllLines(fajlnev).Skip(1))
            {
                rtn.Add(sor.Split(';'));
            }
            return rtn;
        }
        static void FelDolgoz(List<string[]>  adatok,Tipus tipus)
        {
            foreach (string[] adat in adatok)
            {

                eredmenyek.Add(new Versenyzo
                {
                    tipus = tipus,
                    nev = adat[0],
                    orszagkod = adat[1],
                    tpont = decimal.Parse(adat[2].Replace('.', ',')),
                    kpont = decimal.Parse(adat[3].Replace('.', ',')),
                    levonas = decimal.Parse(adat[4].Replace('.', ','))
                });
            }
        }
        //4. feladat
        static decimal OsszPontszam(string nev)
        {
            return eredmenyek.Where(v => v.nev == nev).Aggregate((decimal)0,(ossz,v) => ossz+=v.VersenyszamOsszPontszam);
        }
    }
    struct Versenyzo
    {
        public Tipus tipus;
        public string nev;
        public string orszagkod;
        public decimal tpont;
        public decimal kpont;
        public decimal levonas;
        public decimal osszPontszam;
        public decimal VersenyszamOsszPontszam
        { 
            get
            {
                return this.tpont + this.kpont - this.levonas;
            }
        }
    }
    enum Tipus
    {
        Rovid = 1,
        Donto = 2
    }
}