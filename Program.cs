using System;
using System.Data;
using System.Xml;
using Microsoft.Data.Sqlite;

class Program {
    public static string pathDB = "Data Source=mydatabase.db";
    static void Main() {
        if (!File.Exists("mydatabase.db")) {
            Console.WriteLine("Database tidak ditemukan, Membuat database...");
            buatDb();
        }
        Lobby();
    }
    static void Lobby() {
        String UID = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
        Console.WriteLine("======= STATISTIKA SAMPAH KECAMATAN =======");
        Console.Write("1. Input\n2. Hapus\n3. Edit\n4. Search\n5. Filter\nMasukan Pilihan: ");
        int x = int.Parse(Console.ReadLine());
        if(x == 1) {
            Console.WriteLine("\n======= INPUT =======");
            Console.Write("Masukan berat sampah: ");
            double beratSampah = double.Parse(Console.ReadLine());
            Console.Write("Masukan kecamatan: ");
            string namaKecamatan = Console.ReadLine();
            Console.Write("Masukan bulan: ");
            int bulanTanggal = int.Parse(Console.ReadLine());
            Console.Write("\n"); 
            TulisDB(bulanTanggal, beratSampah, UID, namaKecamatan);
        } else if(x == 2) {
            Console.WriteLine();
            displayDb();
            Console.WriteLine();
            Console.WriteLine("\n======= HAPUS =======");
            Console.Write("1. Hapus dengan Bulan\n2. Hapus dengan UID\n3. Hapus dengan Kecamatan\n");
            Console.Write("Masukan pilihan: ");
            int y = int.Parse(Console.ReadLine());
            int intPilihan = 0;
            string stringPilihan = "";
            
            switch (y) {
                case 1:
                    Console.Write("Masukan angka bulan: ");
                    intPilihan = int.Parse(Console.ReadLine());
                    hapusDb("bulanTanggal", intPilihan);
                    break;
                case 2:
                    Console.Write("Masukan UID: ");
                    stringPilihan = Console.ReadLine();
                    hapusDb("UID", stringPilihan);
                    break;
                case 3:
                    Console.Write("Masukan Kecamatan: ");
                    stringPilihan = Console.ReadLine();
                    hapusDb("namaKecamatan", stringPilihan);
                    break;
                default:
                    break;
            }
        }
    }

    public static void displayDb(){
        var connection = new SqliteConnection(pathDB);
        connection.Open();
        string query = "SELECT Id, bulanTanggal,beratSampah, UID, namaKecamatan FROM Users";
        var commText = new SqliteCommand(query, connection);
        using (var reader = commText.ExecuteReader()) {
            Console.WriteLine("Id\tBulan\tberatSampah\tUID\tKecamatan");
            while (reader.Read()) {
                int id = reader.GetInt32(0); // Column 0: Id
                int bulanVal = reader.GetInt32(1); // Column 1: Name
                double beratVal = reader.GetDouble(2);
                string UID = reader.GetString(3); // Column 2: Age
                string kecamatanStr = reader.GetString(4);

                Console.WriteLine($"{id}\t{bulanVal}\t{beratVal}\t\t{UID}\t{kecamatanStr}");
        }
    }
    }

    public static void hapusDb(dynamic namaKolum, dynamic valueRow){
        using (var connection = new SqliteConnection(pathDB)) {
            connection.Open();
            SqliteCommand commText;
            if(valueRow is string){
                commText = new SqliteCommand($"DELETE FROM Users WHERE {namaKolum} = '{valueRow}'", connection);
            } else {
                commText = new SqliteCommand($"DELETE FROM Users WHERE {namaKolum} = {valueRow}", connection);
            }
            commText.ExecuteNonQuery();
            Console.WriteLine($"Data  {namaKolum} : {valueRow} berhasil dihapus!");
            Lobby();
        }
    }

    public static void buatDb(){
        using (var connection = new SqliteConnection(pathDB)) {
            connection.Open();
            Console.WriteLine("Database Telah Dibuat!");

            var command = connection.CreateCommand(); //tanggalInt, beratSampah, UID, Kecamatan
            command.CommandText = @" 
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    bulanTanggal INTEGER,
                    beratSampah REAL,
                    UID TEXT NOT NULL,
                    namaKecamatan TEXT NOT NULL
                )";

            command.ExecuteNonQuery();

           /* command.CommandText = "SELECT * FROM Users";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Age: {reader.GetInt32(2)}");
                }
            }*/
        }
    }

    public static void TulisDB(int bulanTanggal, double beratSampah, String UID, String namaKecamatan) {
        using (var connection = new SqliteConnection(pathDB)) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (bulanTanggal, beratSampah, UID, namaKecamatan)
                VALUES (@bulanTanggal, @beratSampah, @UID, @namaKecamatan
            )";

            command.Parameters.Add("@bulanTanggal", SqliteType.Integer);
            command.Parameters["@bulanTanggal"].Value = bulanTanggal;

            command.Parameters.Add("@beratSampah", SqliteType.Real);
            command.Parameters["@beratSampah"].Value = beratSampah;

            command.Parameters.Add("@UID", SqliteType.Text);
            command.Parameters["@UID"].Value = UID;

            command.Parameters.Add("@namaKecamatan", SqliteType.Text);
            command.Parameters["@namaKecamatan"].Value = namaKecamatan;

            command.ExecuteNonQuery();
            Console.WriteLine($"Data Telah Disimpan Dengan UID: {UID}");
            Lobby();
        }
    }
}