using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Trivial_pursuit_LPR_30_novembre
{
    public partial class BonneReponse : Form
    {
        static public SqlConnection connSql = new SqlConnection();
        public BonneReponse()
        {
            InitializeComponent();
        }

        private void Connecter_Click(object sender, EventArgs e)
        {
            string dataSource = "LAPTOP-FD8UUTIE\\SQLEXPRESS";
            string UserId = "trivial";
            string motDePasse = "12345";
            string bd = "dbdev";

            string chaineConnection = $"Data source ={dataSource}; " +
                $"User Id ={UserId} " +
                $";password ={motDePasse}"
               + $";initial catalog ={bd}";
            
            try
            {
                connSql.ConnectionString = chaineConnection;
                connSql.Open();
                MessageBox.Show($"status de connection: {connSql.State}", "Connected");
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"status de connection: {connSql.State}", "Connection Impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        static bool VérifierJoueur(string alias, string password)
        {
            SqlCommand cmd = new SqlCommand("ValiderIdentité", connSql);
            cmd.CommandText = "ValiderIdentité";
            cmd.CommandType = CommandType.StoredProcedure;


            string param = "palias";
            string param2 = "pmotDePasse";
            SqlParameter aliasParam = new SqlParameter(param, SqlDbType.VarChar, 20);
            SqlParameter psswdParam = new SqlParameter(param2, SqlDbType.VarChar, 30);
            aliasParam.Direction = ParameterDirection.Input;
            psswdParam.Direction = ParameterDirection.Input;


            aliasParam.Value = alias;
            psswdParam.Value = password;

            SqlParameter resultat = new SqlParameter("@valide", SqlDbType.Int);
            resultat.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(aliasParam);
            cmd.Parameters.Add(psswdParam);
            cmd.Parameters.Add(resultat);
            cmd.ExecuteScalar();

            string valide = resultat.Value.ToString();

            return valide == "0";
        }
        private void InitialiserCategorie()
        {

            string query = $"select nomCategorie from Categories";

            SqlCommand sqlCommdSelect = new SqlCommand(query, connSql);
            SqlDataReader sqlRead = sqlCommdSelect.ExecuteReader();

            while (sqlRead.Read())
            {
                Categories.Items.Add(sqlRead.GetString(0));
            }
            sqlRead.Close();
            sqlRead.Dispose();
        }
        private void Valider_Click(object sender, EventArgs e)
        {
            string alias = Alias.Text;
            string passwordJoueur = password.Text;
            if (VérifierJoueur(alias, passwordJoueur))
            {
                MessageBox.Show("Joueur Valider !");
                InitialiserCategorie();
                Ajouter.Visible = true;
            }
            else
            {
                MessageBox.Show("joueur invalide , veuillez ressayer !");
            }
        }

        private void Ajouter_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("AjouterQuestionRéponses", connSql);
            cmd.CommandText = "AjouterQuestionRéponses";
            cmd.CommandType = CommandType.StoredProcedure;

            string param = "@enonce";
            string param2 = "@idcategorie";
            string param3 = "@difficulte";
            string param4 = "@rep1";
            string param5 = "@estBonne";
            string param6 = "@rep2";
            string param7 = "@estBonneB";
            string param8 = "@rep3";
            string param9 = "@estBonneC";
            string param10 = "@rep4";
            string param11 = "@estBonneD";

            SqlParameter enonceParam = new SqlParameter(param, SqlDbType.VarChar, 60);
            SqlParameter idcatParam = new SqlParameter(param2, SqlDbType.Char, 1);
            SqlParameter difficulteParam = new SqlParameter(param3, SqlDbType.Char, 1);

            SqlParameter rep1Param = new SqlParameter(param4, SqlDbType.VarChar, 60);
            SqlParameter estBonne1Param = new SqlParameter(param5, SqlDbType.Char, 1);

            SqlParameter rep2Param = new SqlParameter(param6, SqlDbType.VarChar, 60);
            SqlParameter estBonne2Param = new SqlParameter(param7, SqlDbType.Char, 1);

            SqlParameter rep3Param = new SqlParameter(param8, SqlDbType.VarChar, 60);
            SqlParameter estBonne3Param = new SqlParameter(param9, SqlDbType.Char, 1);

            SqlParameter rep4Param = new SqlParameter(param10, SqlDbType.VarChar, 60);
            SqlParameter estBonne4Param = new SqlParameter(param11, SqlDbType.Char, 1);



            enonceParam.Direction = ParameterDirection.Input;
            idcatParam.Direction = ParameterDirection.Input;
            difficulteParam.Direction = ParameterDirection.Input;
            rep1Param.Direction = ParameterDirection.Input;
            estBonne1Param.Direction = ParameterDirection.Input;
            rep2Param.Direction = ParameterDirection.Input;
            estBonne2Param.Direction = ParameterDirection.Input;
            rep3Param.Direction = ParameterDirection.Input;
            estBonne3Param.Direction = ParameterDirection.Input;
            rep4Param.Direction = ParameterDirection.Input;
            estBonne4Param.Direction = ParameterDirection.Input;

            TrouverBonneRéponse(estBonne1Param, estBonne2Param, estBonne3Param, estBonne4Param);

            enonceParam.Value = question.Text;
            idcatParam.Value = TrouverBonCharCategorie(Categories.SelectedItem.ToString());

            difficulteParam.Value = TrouverDifficulter();
            rep1Param.Value = rep1.Text;
            rep2Param.Value = rep2.Text;
            rep3Param.Value = rep3.Text;
            rep4Param.Value = rep4.Text;

            cmd.Parameters.Add(enonceParam);
            cmd.Parameters.Add(idcatParam);
            cmd.Parameters.Add(difficulteParam);

            cmd.Parameters.Add(rep1Param);
            cmd.Parameters.Add(estBonne1Param);

            cmd.Parameters.Add(rep2Param);
            cmd.Parameters.Add(estBonne2Param);

            cmd.Parameters.Add(rep3Param);
            cmd.Parameters.Add(estBonne3Param);

            cmd.Parameters.Add(rep4Param);
            cmd.Parameters.Add(estBonne4Param);

            try
            {
                int nbligne =cmd.ExecuteNonQuery();
                MessageBox.Show("nb de ligne affecté :" + nbligne);
            }
            catch(Exception)
            {
                MessageBox.Show("erreur");
            }






        }
        private char TrouverDifficulter()
        {
            switch (Difficulté.SelectedItem.ToString())
            {

                case "Facile":
                    return '1';
                case "Moyen":
                    return '2';
                case "Difficile":
                    return '3';
                default: 
                    return '0';
            }
        }
        private char TrouverBonCharCategorie(string categorie)
        {
            switch (categorie)
            {
                case "art et lettres":
                    return 'a';
                case "général":
                    return 'g';
                case "histoire et géographie":
                    return 'h';
                case "science":
                    return 's';
                default:
                    return '0';
            }
        }
        private void TrouverBonneRéponse(SqlParameter estbonne1, SqlParameter estbonne2, SqlParameter estbonne3, SqlParameter estbonne4)
        {
            string bonneQuestion = RéponseBonne.SelectedItem.ToString();
            List<SqlParameter> lstParam = new List<SqlParameter>();
            lstParam.Add(estbonne1);
            lstParam.Add(estbonne2);
            lstParam.Add(estbonne3);
            lstParam.Add(estbonne4);
            switch (bonneQuestion)
            {
                case "1":
                    lstParam[0].Value = 'o';
                    lstParam.RemoveAt(0);
                    break;
                case "2":
                    lstParam[1].Value = 'o';
                    lstParam.RemoveAt(1);
                    break;
                case "3":
                    lstParam[2].Value = 'o';
                    lstParam.RemoveAt(2);
                    break;
                case "4":
                    lstParam[3].Value = 'o';
                    lstParam.RemoveAt(3);
                    break;
                default:
                    MessageBox.Show("Erreur , pas trouvé bonne réponse");
                    break;

            }
            foreach(SqlParameter p in lstParam)
            {
                p.Value = 'n';
            }
        }
    }
}
