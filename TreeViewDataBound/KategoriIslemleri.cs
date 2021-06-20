using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeViewDataBound
{
    public partial class KategoriIslemleri : Form
    {
        public KategoriIslemleri()
        {
            InitializeComponent();
        }

        private void KategoriIslemleri_Load(object sender, EventArgs e)
        {
            Doldur();
        }

        private void btn_ekle_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=Asililar_DB;Integrated Security=True"))
            {
                try
                {
                    SqlCommand cmd = con.CreateCommand();

                    if (treeView1.SelectedNode == null)
                    {
                        cmd.CommandText = "INSERT INTO Kategoriler(Isim) VALUES(@a)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@a", tb_kategori.Text);
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO Kategoriler(Isim, UstKategori_ID) VALUES(@a,@b)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@a", tb_kategori.Text);
                        cmd.Parameters.AddWithValue("@b", IDGetir(treeView1.SelectedNode.Text));
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Ekleme işlemi başarılı", "Kategori Eklendi");
                }
                catch
                {
                    MessageBox.Show("Ekleme işleminde hata oluştu", "Hata Var.");
                }
            }
            Doldur();
        }

        /// <summary>
        /// Kategori Adına karşılık gelen id değerini getirir
        /// </summary>
        /// <param name="isim">Veritabanında aranacak kategori adı</param>
        /// <returns>Aranan kategorinin ID si</returns>
        public int IDGetir(string isim)
        {

            using (SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=Asililar_DB;Integrated Security=True"))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID FROM Kategoriler WHERE Isim = @a";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@a", isim);
                con.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                return id;
            }
        }
        public List<Kategori> KategorileriGetir()
        {
            List<Kategori> kategoriler = new List<Kategori>();

            using (SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=Asililar_DB;Integrated Security=True"))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Kategoriler";
                cmd.Parameters.Clear();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Kategori k = new Kategori()
                    {
                        ID = reader.GetInt32(0),
                        UstKategori_ID = reader.IsDBNull(1) ? Convert.ToInt32("-1") : reader.GetInt32(1),
                        Isim = reader.GetString(2)
                    };
                    kategoriler.Add(k);
                }
            }
            return kategoriler;
        }


        public void Doldur()
        {
            treeView1.Nodes.Clear();
            List<Kategori> tumkategoriler = KategorileriGetir();

            foreach (Kategori kategori in tumkategoriler)
            {
                TreeNode node = new TreeNode
                {
                    Name = kategori.ID.ToString(),
                    Text = kategori.Isim,
                };
                if (kategori.UstKategori_ID != -1)
                {
                    string parentId = kategori.UstKategori_ID.ToString();
                    TreeNode parentNode = treeView1.Nodes.Find(parentId, true)[0];
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    treeView1.Nodes.Add(node);
                }
            }
        }
    }
}
