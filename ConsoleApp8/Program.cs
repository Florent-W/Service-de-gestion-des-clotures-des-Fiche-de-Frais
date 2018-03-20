﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;

using System.Threading.Tasks;

using MySql.Data.MySqlClient;
                 
      
namespace ConsoleApp8 
{
    /// <summary>
    /// Classe d'accès aux données et de gestion des dates
    /// </summary>
    public class Form2
    {
        MySqlConnection connexion = new MySqlConnection("database=gsb_frais; server=localhost; user id=root"); // Connexion à la base de données

        /// <summary>
        /// Cette méthode permet de sélectionner toutes les fiches de frais et de les écrire dans un fichier de sortie présent dans le répertoire de l'application
        /// </summary>
        public void selectionFiche()
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = connexion;

            string pathCommandes = AppDomain.CurrentDomain.BaseDirectory + "commandes.txt";



            // tentative de connexion à la base de données
            try
            {
                System.IO.File.AppendAllText(pathCommandes, "Connexion à MySQL... \n");

                    connexion.Open();

                    command.CommandText = "SELECT * FROM fichefrais";

                    MySqlDataReader reader = command.ExecuteReader();

               
                /* Ecriture de l'id du visiteur et de la date présente dans la fiche de frais dans un fichier de log. */
                while (reader.Read())
                {
                    string textReadId = reader["idvisiteur"].ToString();
                    string textReadMois = reader["mois"].ToString();
                    System.IO.File.AppendAllText(pathCommandes, textReadId + " ");
                    System.IO.File.AppendAllText(pathCommandes, textReadMois + "\n");
                }
                reader.Close();
                }
                catch (Exception ex)
                {
                System.IO.File.AppendAllText(pathCommandes, ex.ToString() + "\n");
            }

            connexion.Close();
        }

        /// <summary>
        /// Cette méthode permet de sélectionner les fiches de frais du mois précédent et de les écrire dans un fichier de sortie présent dans le répertoire de l'application
        /// </summary>
        public void selectionFicheMoisPrecedent()
            {
                
            MySqlCommand command = new MySqlCommand();
            command.Connection = connexion;

            string pathCommandes = AppDomain.CurrentDomain.BaseDirectory + "commandes.txt";

            // tentative de connexion à la base de données
            try
            {
                System.IO.File.AppendAllText(pathCommandes, "Connexion à MySQL... \n");

                connexion.Open();

                MySqlCommand commandSelectFiche = new MySqlCommand();
                string moisPrecedent = getMoisPrecedent();
                string annee = getAnnee();

                string moisSelectionner = annee + moisPrecedent;

                commandSelectFiche.Connection = connexion;
                commandSelectFiche.CommandText = "SELECT * FROM fichefrais where mois='" + moisSelectionner + "'";


                MySqlDataReader reader = commandSelectFiche.ExecuteReader();

                /* Ecriture de l'id du visiteur et de la date présente dans la fiche de frais dans un fichier de log. */
                while (reader.Read())
                {
                    string textReadId = reader["idvisiteur"].ToString();
                    string textReadMois = reader["mois"].ToString();
                    System.IO.File.AppendAllText(pathCommandes, textReadId + " ");
                    System.IO.File.AppendAllText(pathCommandes, textReadMois + "\n"); 
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(pathCommandes, ex.ToString() + "\n");
            }

            connexion.Close();

        }

        /// <summary>
        /// Cette méthode permet de mettre à jour les fiches de frais du mois précédent en les mettant à l'état "CL" (validé)
        /// </summary>
        public void miseAJourFicheValidation()
            {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connexion;

            string pathCommandes = AppDomain.CurrentDomain.BaseDirectory + "commandes.txt";

            // tentative de connexion à la base de données
            try
            {
                System.IO.File.AppendAllText(pathCommandes, "Connexion à MySQL... \n");
                connexion.Open();

                MySqlCommand commandSelectFiche = new MySqlCommand();
                string moisPrecedent = getMoisPrecedent();
                string annee = getAnnee();

                string moisSelectionner = annee + moisPrecedent;

                commandSelectFiche.Connection = connexion;

                // Mise à jour des fiches de frais du mois précédent
                commandSelectFiche.CommandText = "UPDATE fichefrais SET idEtat='CL' WHERE mois='" + moisSelectionner + "'";

                commandSelectFiche.ExecuteNonQuery();

            }

            catch (Exception ex)
            {
                System.IO.File.AppendAllText(pathCommandes, ex.ToString() + "\n"); 
            }

            connexion.Close();

        }

        /// <summary>
        /// Cette méthode permet de mettre à jour les fiches de frais validées et du mois précédent en les mettant à l'état "RB" (remboursé)
        /// </summary>
        public void miseAJourFicheRemboursement()
            {
            MySqlCommand command = new MySqlCommand();
            command.Connection = connexion;

            string pathCommandes = AppDomain.CurrentDomain.BaseDirectory + "commandes.txt";

            try
            {
                // tentative de connexion à la base de données
                System.IO.File.AppendAllText(@pathCommandes, "Connexion à MySQL... \n");
                connexion.Open();

                MySqlCommand commandSelectFiche = new MySqlCommand();
                string moisPrecedent = getMoisPrecedent();
                string annee = getAnnee();

                string moisSelectionner = annee + moisPrecedent;

                commandSelectFiche.Connection = connexion;

                // Mise à jour des fiches de frais du mois précédent en les passant en remboursé
                commandSelectFiche.CommandText = "UPDATE fichefrais SET idEtat='RB' WHERE mois='" + moisSelectionner + "' AND idEtat='VA'";

                commandSelectFiche.ExecuteNonQuery();
            }

            // Exception
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(pathCommandes, ex.ToString() + "\n");
            }

            connexion.Close();
        }

        /// <summary>
        /// La méthode va servir à récupérer l'année du mois précédent
        /// </summary>
        /// <returns>Une chaîne de caractère représentant l'année du mois précédent</returns>
        public static string getAnnee()
            {
                string annee;

                // Si le mois actuel n'est pas Janvier, on récupère l'année
                if (DateTime.Now.ToString("MM") != "01")
                {
                    annee = DateTime.Now.ToString("yyyy");
                }

                // Si le mois actuel est Janvier, on récupère l'année précédente 
                else
                {
                    annee = DateTime.Now.AddMonths(-1).ToString("yyyy");
                }

                return annee;

            }

        /// <summary>
        /// La méthode retourne le mois précédent
        /// </summary>
        /// <returns>Une chaîne de caractère représentant le mois précédent sous forme de deux caractères</returns>
        public static string getMoisPrecedent()
            {
                // Le mois actuel est récupéré et soustrait de un mois pour obtenir le mois précédent
                string moisPrecedent = DateTime.Now.AddMonths(-1).ToString("MM"); 

                return moisPrecedent;
            }

        /// <summary>
        /// La méthode retourne le mois précédent à l'aide d'un objet de type datetime
        /// </summary>
        /// <param name="date">Objet de type datetime</param>
        /// <returns>Une chaîne de caractère représentant le mois précédent sous forme de deux caractères</returns>
        public static string getMoisPrecedent(DateTime date)
            {
            // Le date renseignée est récupérée est soustraite de un mois pour obtenir le mois précédent
            string moisPrecedent = date.AddMonths(-1).ToString("MM");
                return moisPrecedent;
            }

        /// <summary>
        /// La méthode retourne le mois suivant
        /// </summary>
        /// <returns>Une chaîne de caractère représentant le mois suivant sous forme de deux caractères</returns>
        public static string getMoisSuivant()
            {
            // Le date renseignée est récupérée est augmentée de un mois pour obtenir le mois suivant
            string moisSuivant = DateTime.Now.AddMonths(+1).ToString("MM");

                return moisSuivant;
            }

        /// <summary>
        /// La méthode retourne le mois suivant à l'aide d'un objet de type datetime
        /// </summary>
        /// <param name="date">Objet de type datetime</param>
        /// <returns>Une chaîne de caractère représentant le mois suivant sous forme de deux caractères</returns>
        public static string getMoisSuivant(DateTime date)
            {
            // Le date renseignée est récupérée est augmentée de un mois pour obtenir le mois suivant
            string moisSuivant = date.AddMonths(+1).ToString("MM");
                return moisSuivant;
            }

        /// <summary>
        /// La méthode teste et renvoie vrai si le jour actuel est entre les deux jours mis en paramètres sinon faux  
        /// </summary>
        /// <param name="jour1">Entier qui sera le premier jour de l'intervalle</param>
        /// <param name="jour2">Entier qui sera le dernier jour de l'intervalle</param>
        /// <returns>Un booléen : Renvoie vrai si le jour actuel est entre les deux jours mis en paramètres sinon faux</returns>
        public static Boolean entre(int jour1, int jour2)
            {
                DateTime dateActuel = DateTime.Now;
                int jour = dateActuel.Day;

                if (jour >= jour1 && jour <= jour2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        /// <summary>
        /// La méthode teste et renvoie vrai si le jour de la date mis en paramètre est entre les deux jours de l'intervalle mis en paramètres sinon renvoie faux  
        /// </summary>
        /// <param name="jour1">Entier qui sera le premier jour de l'intervalle</param>
        /// <param name="jour2">Entier qui sera le dernier jour de l'intervalle</param>
        /// <param name="dateTest">Date qui sera comparé à l'intervalle</param>
        /// <returns>Un booléen : Renvoie vrai si le jour de la date mis en paramètre est entre les deux jours mis en paramètres sinon renvoie faux</returns>
        public static Boolean entre(int jour1, int jour2, DateTime dateTest)
            {
                int jour = dateTest.Day;

                if (jour >= jour1 && jour <= jour2)
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }

        // <summary>
        /// Cette méthode s'occupe de quasiment tous les traitements : mise à jour des fiches, logs pour bien indiquer à quels dates le traitement à été effectuée
        /// </summary>
        public static void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {



            string pathCommandes = AppDomain.CurrentDomain.BaseDirectory + "commandes.txt";

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Ecriture de la date du traitement dans un fichier situé dans le répertoire du programme
            System.IO.File.AppendAllText(@pathCommandes, "Date de verification : " + date + "\n");

            // Accès aux fonctions de manipulations des fiches et de gestions des dates
            Form2 fiche = new Form2();

           
            // Si le jour actuel est entre 1 et 10, les fiches sont mises à jour en validation dans les conditions de la méthode (pour comparer à la version d'avant, les fiches sont écrite dans le fichier de logs)
            if (entre(1, 10))
            {
                fiche.selectionFiche();
                fiche.miseAJourFicheValidation();
                System.IO.File.AppendAllText(@pathCommandes, "Le jour est entre \n \n");

            }

            // Si le jour actuel est entre 20 et 31, les fiches sont mises à jour en remboursement dans les conditions de la méthode (pour comparer à la version d'avant, les fiches sont écrite dans le fichier de logs)
            else if (entre(20, 31))
            {
                fiche.selectionFiche();
                fiche.miseAJourFicheRemboursement();
                 System.IO.File.AppendAllText(@pathCommandes, "Remboursé \n \n");
            }

            // Sinon, on séléctionne juste les fiches et on écrit dans le fichier qu'il n'y a pas d'action qui sont effectué ce jour-là sur l'état des fiches par le programme
            else
            {
                fiche.selectionFiche();
                System.IO.File.AppendAllText(@pathCommandes, "Le jour n'est pas entre \n \n");
            }
   }
        // <summary>
        /// Cette méthode va permettre de déclencher le programme selon un intervalle donné (Si le timer est à 5000, le programme va recommencer toutes les cinq secondes et le service essayera de mettre à jour les fiches de frais toutes les cinq secondes) 
        /// </summary>
        public void Action()
        {
           
               // System.IO.File.AppendAllText(@"H:\" + "texte.txt", "b");
                Timer myTimer = new Timer();
                myTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
                myTimer.Interval = 5000; // Les 5 secondes ont été choisies pour les conditions d'examen, dans des conditions d'entreprises, il n'y aurait besoin de ne vérifier les fiches que toutes les 24 heures puisqu'on regarde le jour actuel pour mettre à jour les fiches, une journée équivaut à 86400000 millisecondes) 
            myTimer.Start();
           
            
        }

        static void Main(string[] args)
        {
          
            


        }
        }
      
    
}
