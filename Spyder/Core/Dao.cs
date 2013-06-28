using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Core {
    public class Dao {
        private static Dao instance = null;

        public AVSyncerDataContext dataContext { get; set; }

        public Dao () {
            this.dataContext = new AVSyncerDataContext();
        }

        public static Dao getInstance () {
            if (instance == null) {
                instance = new Dao();
            }
            return instance;
        }

        internal Label QueryOrInsert (Label lab) {
            var q = (from label in dataContext.Label
                     where label.name == lab.name
                     select label).Take(1);
            if (q.Count() == 0) {
                System.Console.WriteLine("insert Label -- " + lab.name);
                dataContext.Label.InsertOnSubmit(lab);
                dataContext.SubmitChanges();
                return lab;
            } else {
                return q.First();
            }
        }

        internal Movie findMovieByMvid (string mvid) {
            var q = (from movie in dataContext.Movie
                     where movie.mvid == mvid
                     select movie).Take(1);
            if (q.Count() == 0) {
                return null;
            } else {
                return q.First();
            }
        }

        internal Actress QueryOrInsert (Actress act) {
            var q = (from actress in dataContext.Actress
                     where actress.name == act.name
                     select actress).Take(1);
            if (q.Count() == 0) {
                System.Console.WriteLine("insert Actress ++++++++ " + act.name);
                dataContext.Actress.InsertOnSubmit(act);
                dataContext.SubmitChanges();
                return act;
            } else {
                System.Console.WriteLine("query Actress -- " + act.name);
                return q.First();
            }
        }

        internal Series QueryOrInsert (Series ser) {
            var q = (from series in dataContext.Series
                     where series.name == ser.name
                     select series).Take(1);
            if (q.Count() == 0) {
                System.Console.WriteLine("insert Series -- " + ser.name);
                dataContext.Series.InsertOnSubmit(ser);
                dataContext.SubmitChanges();
                return ser;
            } else {
                return q.First();
            }
        }

        internal Maker QueryOrInsert (Maker mak) {
            var q = (from maker in dataContext.Maker
                     where maker.name == mak.name
                     select maker).Take(1);
            if (q.Count() == 0) {
                System.Console.WriteLine("insert Maker -- " + mak.name);
                dataContext.Maker.InsertOnSubmit(mak);
                dataContext.SubmitChanges();
                return mak;
            } else {
                System.Console.WriteLine("query Maker -- " + mak.name);
                return q.First();
            }
        }

        internal Movie QueryOrInsert (Movie mv) {
            var q = (from movie in dataContext.Movie
                     where movie.mvid == mv.mvid
                     select movie).Take(1);
            if (q.Count() == 0) {
                System.Console.WriteLine("insert Movie -- " + mv.mvid);
                dataContext.Movie.InsertOnSubmit(mv);
                dataContext.SubmitChanges();
                return mv;
            } else {
                System.Console.WriteLine("query Movie -- " + mv.mvid);
                return q.First();
            }
        }

        internal ActressMovie insertOrUpdate (ActressMovie amv) {
            ActressMovie result;
            var q = (from actressMovie in dataContext.ActressMovie
                     where actressMovie.movieId == amv.movieId
                     && actressMovie.actressId == amv.actressId
                     select actressMovie).Take(1);
            if (q.Count() == 0) {
                dataContext.ActressMovie.InsertOnSubmit(amv);
                result = amv;
            } else {
                result = q.First();
                result.actressOrder = amv.actressOrder;
            }
            dataContext.SubmitChanges();
            return result;
        }

        internal void deleteByQuery (Movie mv, string mvid) {
            var q = (from movie in dataContext.Movie
                     where movie.mvid == mvid
                     select movie).Take(1);
            if (q.Count() != 0) {
                System.Console.WriteLine("delete Movie -- " + mvid);
                mv = q.First();
                dataContext.ActressMovie.DeleteAllOnSubmit(mv.ActressMovie);
                dataContext.Movie.DeleteOnSubmit(mv);
                dataContext.SubmitChanges();
            }
        }
    }

    /*
delete from ActressMovie dbcc checkident('ActressMovie', RESEED, 0);
delete from Movie dbcc checkident('Movie', RESEED, 0);
delete from Label dbcc checkident('Label', RESEED, 0);
delete from Series dbcc checkident('Series', RESEED, 0);
delete from Actress dbcc checkident('Actress', RESEED, 0);
     */
}
