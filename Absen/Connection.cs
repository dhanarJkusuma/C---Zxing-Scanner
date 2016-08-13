using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Absen
{
    public class Connection
    {
        public static String DirectoryLocation = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        public Connection()
        {
            try
            {
                alamat = "Provider=Microsoft.ace.Oledb.12.0; Data Source=" + DirectoryLocation + "\\_Database\\absen.accdb";
                koneksi = new OleDbConnection(alamat);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Console.WriteLine(e.ToString());
            }
        }

        private string alamat;
        private OleDbConnection koneksi;
        private OleDbCommand perintah;
        private OleDbDataAdapter adapter;
        private DataSet ds;
        private static Connection connection = null;

        public static Connection GetInstance()
        {
            if (Connection.connection == null)
            {
                Connection.connection = new Connection();
            }
            return Connection.connection;
        }

        private DataTable GetData(string query)
        {
            try
            {
                
                ds = new DataSet();
                DataTable dt = new DataTable();
                perintah = new OleDbCommand(query, koneksi);
                koneksi.Open();
                adapter = new OleDbDataAdapter(perintah);
                adapter.Fill(dt);
                koneksi.Close();
                return dt;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public String Scan(String qrcode) 
        {
            string name = "";
            bool isFound = false;
            try
            {
                string queryString = "select nama from pengunjung where qrcode = '" + qrcode + "' AND isScanned=False;";
                OleDbCommand command = new OleDbCommand(queryString, koneksi);
                koneksi.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        name = reader.GetValue(0).ToString();
                    }
                    isFound = true;            
                }
                reader.Close();
                koneksi.Close();
                if (isFound) {
                    processScan(qrcode);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return name;
        }

        private int Manipulate(string query)
        {
            try
            {
                int res = 0;
                koneksi.Open();
                perintah = new OleDbCommand(query, koneksi);
                adapter = new OleDbDataAdapter(perintah);
                res = perintah.ExecuteNonQuery();
                koneksi.Close();
                return res;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public DataTable GetAllPeople() 
        {
            return GetData("select * from pengunjung order by nomor asc;");
        }

        public DataTable GetScannedPeople()
        {
            return GetData("select * from pengunjung where isScanned=True order by nomor asc;");
        }

        public DataTable GetScannedPerson(string qrcode)
        {
            return GetData("select * from pengunjung where qrcode = '" + qrcode + "';");
        }

        public int NewPerson(string qrcode, string nama, bool isScanned)
        {
            int scan = (isScanned) ? 1 : 0;
            return Manipulate("insert into pengunjung(qrcode,nama,isScanned) values ('" + qrcode + "','" + nama + "','" +  scan.ToString() + "');");
        }

        public int processScan(string qrcode)
        {
            return Manipulate("update pengunjung set isScanned = '1' where qrcode = '" + qrcode + "';");
        }

        public int resetAll() 
        {
            return Manipulate("update pengunjung set isScanned = '0';");
        }

        public int getCountScanned() 
        {
            int count = 0;
            try
            {
                string queryString = "select count(*) from pengunjung where isScanned=True;";
                OleDbCommand command = new OleDbCommand(queryString, koneksi);
                koneksi.Open();
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        count = Int32.Parse(reader.GetValue(0).ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Data tidak ditemukan.");
                }


                reader.Close();
                koneksi.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return count;
        }
    }  
}
