using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newbrewers
{
    class BrewerDB
    {
//        public enum RegistInfo { prefecture,taxoffice, licenceDate, registDate, name, place, classification, item, type};
        private System.Data.SQLite.SQLiteConnection db;
        private bool closed;
//        private int limit = 10000;

        public BrewerDB()
        {
            string mydir = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string mydbpath = string.Format("{0}\\brewer.db", mydir);
            db = new System.Data.SQLite.SQLiteConnection(string.Format("Data Source={0}", mydbpath));
            if (!System.IO.File.Exists(mydbpath))
            {
                createDB();
            }
            closed = true;
        }

        public void open()
        {
            if (closed)
            {
                db.Open();
                closed = false;
            }
        }

        public void close()
        {
            if (!closed)
            {
                db.Close();
                closed = true;
            }
        }

        private void createDB()
        {
            db.Open();
            var com = db.CreateCommand();
            com.CommandText = "CREATE TABLE brewers (id integer primary key AUTOINCREMENT, prefecture text, taxoffice text, licencedate text, registdate text, name text, place text, classification text, item text, type text)";
            com.ExecuteNonQuery();
            db.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        public void regist(Brewer b)
        {
            if (isRegisted(b))
            {
                return;//登録済みなら何もしない
            }

            var com = db.CreateCommand();

            com.CommandText = "insert into brewers(prefecture,taxoffice,licencedate,registdate,name,place,classification,item,type) values(@1,@2,@3,@4,@5,@6,@7,@8,@9);";
            com.Parameters.AddWithValue("@1", b.getParam(Brewer.RegistInfo.prefecture));
            com.Parameters.AddWithValue("@2", b.getParam(Brewer.RegistInfo.taxoffice));
            com.Parameters.AddWithValue("@3", b.getParam(Brewer.RegistInfo.licenceDate));
            com.Parameters.AddWithValue("@4", b.getParam(Brewer.RegistInfo.registDate));
            com.Parameters.AddWithValue("@5", b.getParam(Brewer.RegistInfo.name));
            com.Parameters.AddWithValue("@6", b.getParam(Brewer.RegistInfo.place));
            com.Parameters.AddWithValue("@7", b.getParam(Brewer.RegistInfo.classification));
            com.Parameters.AddWithValue("@8", b.getParam(Brewer.RegistInfo.item));
            com.Parameters.AddWithValue("@9", b.getParam(Brewer.RegistInfo.type));
            com.ExecuteNonQuery();
        }

        /// <summary>
        /// 既に登録済みか確認
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool isRegisted(Brewer b)
        {
            var com = db.CreateCommand();
            com.CommandText = "select id from brewers where licencedate = @1 and name = @2 and item = @3";
            com.Parameters.AddWithValue("@1", b.getParam(Brewer.RegistInfo.licenceDate));
            com.Parameters.AddWithValue("@2", b.getParam(Brewer.RegistInfo.name));
            com.Parameters.AddWithValue("@3", b.getParam(Brewer.RegistInfo.item));
            var r = com.ExecuteReader();
            return r.Read();

        }

        public List<Brewer> searchBrewer(string str)
        {

            var list = new List<Brewer>();

            var com = db.CreateCommand();
            com.CommandText = "select prefecture,taxoffice,licencedate,registdate,name,place,classification,item,type from brewers where (prefecture like @1 or name like @1 or item like @1)";
            com.Parameters.AddWithValue("@1", string.Format("%{0}%", str));
            var reader = com.ExecuteReader();
            while (reader.Read())
            {
                var pref = reader.GetString((int)Brewer.RegistInfo.prefecture);
                string[] info = { reader.GetString((int)Brewer.RegistInfo.taxoffice), reader.GetString((int)Brewer.RegistInfo.licenceDate), reader.GetString((int)Brewer.RegistInfo.registDate), reader.GetString((int)Brewer.RegistInfo.name), reader.GetString((int)Brewer.RegistInfo.place), reader.GetString((int)Brewer.RegistInfo.classification), reader.GetString((int)Brewer.RegistInfo.item), reader.GetString((int)Brewer.RegistInfo.type) };
                list.Add(new Brewer(pref, info));
            }
            return list;
        }
    }
}
