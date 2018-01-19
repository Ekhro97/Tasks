using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{
		//Valors constants
		#region Constants
		const int nCaixeres = 3;

		#endregion
		/* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

		public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

		public static void Main(string[] args)
		{
			var clock = new Stopwatch();
			var threads = new List<Task>();

			string filePath= @"..\..\..\clients.csv";
			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
    					sr.ReadLine();
    					while (sr.Peek() != -1)
    					{
       						string line = sr.ReadLine();
        					var values = line.Split(',');
                    				var tmp = new Client() { nom = values[0], carretCompra = Int32.Parse(values[1]) };
                    				cua.Enqueue(tmp);

       
    					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}

			clock.Start();


            // Instanciar les caixeres i afegir el task creat a la llista de tasks
            Caixera caixera1 = new Caixera { idCaixera = 1 };
            Caixera caixera2 = new Caixera { idCaixera = 2 };
            Caixera caixera3 = new Caixera { idCaixera = 3 };

            threads.Add(Task.Run(() => { caixera1.ProcessarCua(); }));
            threads.Add(Task.Run(() => { caixera2.ProcessarCua(); }));
            threads.Add(Task.Run(() => { caixera3.ProcessarCua(); }));

            // Procediment per esperar que acabin tots els threads abans d'acabar

            Task.WaitAll(threads.ToArray());

			// Parem el rellotge i mostrem el temps que triga
			clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}
	#region ClassCaixera
	public class Caixera
	{
		public int idCaixera
		{
			get;
			set;
		}

		public void ProcessarCua()
		{
            // Llegirem la cua extreient l'element
            // cridem al mètode ProcesarCompra passant-li el client
            Client client;

            while(MainClass.cua.Count > 0)
            {
                bool Completat = MainClass.cua.TryDequeue(out client);

                if (Completat)
                {
                    this.ProcesarCompra(client);
                }
            }
        }


		private void ProcesarCompra(Client client)
		{

			Console.WriteLine("La caixera " + this.idCaixera + " comença amb el client " + client.nom + " que té " + client.carretCompra + " productes");

			for (int i = 0; i < client.carretCompra; i++)
			{
				this.ProcessaProducte();

			}

			Console.WriteLine(">>>>>> La caixera " + this.idCaixera + " ha acabat amb el client " + client.nom);
		}


		private void ProcessaProducte()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
		}
	}


	#endregion

	#region ClassClient

	public class Client
	{
		public string nom
		{
			get;
			set;
		}


		public int carretCompra
		{
			get;
			set;
		}


	}

	#endregion
}
