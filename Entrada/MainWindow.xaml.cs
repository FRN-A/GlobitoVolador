using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

using System.Windows.Threading;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;

namespace Entrada
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn;
        DispatcherTimer timer;
        Stopwatch cronometro;
        Stopwatch cronometroElementos;

        float bottomGloboFrecuencia;
        float bottomGlobo;
        float frecuenciaActual = 0;
        float frecuenciaAnterior;

        float tiempoActual = 0.0f;
        float tiempoDiferencial = 0.0f;
        float tiempoAnterior = 0.0f;
        float velocidadEnemigo = 0.8f;
        bool contVueltas = false;
        int score = 0;

        List<Elementos>[] elementos = new List<Elementos>[2];

        List<Elementos> nubes = new List<Elementos>();
		
		public MainWindow()
        {
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Tick += Timer_Tick;


            elementos[0] = new List<Elementos>();
            elementos[1] = new List<Elementos>();

            cronometro = new Stopwatch();
            cronometroElementos = new Stopwatch();
			InitializeComponent();
			llenarListaElementos();

		}

		private void llenarListaElementos()
        {
            //Escenario 1
            elementos[0].Add(new Elementos(imgPino1, Canvas.GetLeft(imgPino1)));
            elementos[0].Add(new Elementos(imgPino2, Canvas.GetLeft(imgPino2)));
            elementos[0].Add(new Elementos(imgOvni, Canvas.GetLeft(imgOvni)));
            elementos[0].Add(new Elementos(imgPino3, Canvas.GetLeft(imgPino3)));
            elementos[0].Add(new Elementos(imgPino4, Canvas.GetLeft(imgPino4)));

            //Esecanrio 2
            elementos[1].Add(new Elementos(imgEdificio1, Canvas.GetLeft(imgEdificio1)));
            elementos[1].Add(new Elementos(imgEdificio2, Canvas.GetLeft(imgEdificio2)));
            elementos[1].Add(new Elementos(imgBatman, Canvas.GetLeft(imgBatman)));
            elementos[1].Add(new Elementos(imgEdificio3, Canvas.GetLeft(imgEdificio3)));
            elementos[1].Add(new Elementos(imgEdificio4, Canvas.GetLeft(imgEdificio4)));

            //Nubes
            nubes.Add(new Elementos(imgNube, Canvas.GetLeft(imgNube)));
            nubes.Add(new Elementos(imgNube_Copy, Canvas.GetLeft(imgNube_Copy)));
            nubes.Add(new Elementos(imgNube_Copy1, Canvas.GetLeft(imgNube_Copy1)));
            nubes.Add(new Elementos(imgNube_Copy10, Canvas.GetLeft(imgNube_Copy10)));
            nubes.Add(new Elementos(imgNube_Copy11, Canvas.GetLeft(imgNube_Copy11)));
            nubes.Add(new Elementos(imgNube_Copy12, Canvas.GetLeft(imgNube_Copy12)));
            nubes.Add(new Elementos(imgNube_Copy13, Canvas.GetLeft(imgNube_Copy13)));
            nubes.Add(new Elementos(imgNube_Copy14, Canvas.GetLeft(imgNube_Copy14)));
            nubes.Add(new Elementos(imgNube_Copy15, Canvas.GetLeft(imgNube_Copy15)));
            nubes.Add(new Elementos(imgNube_Copy16, Canvas.GetLeft(imgNube_Copy16)));
            nubes.Add(new Elementos(imgNube_Copy17, Canvas.GetLeft(imgNube_Copy17)));
            nubes.Add(new Elementos(imgNube_Copy18, Canvas.GetLeft(imgNube_Copy18)));
            nubes.Add(new Elementos(imgNube_Copy19, Canvas.GetLeft(imgNube_Copy19)));
            nubes.Add(new Elementos(imgNube_Copy20, Canvas.GetLeft(imgNube_Copy20)));
            nubes.Add(new Elementos(imgNube_Copy21, Canvas.GetLeft(imgNube_Copy21)));
            nubes.Add(new Elementos(imgNube_Copy22, Canvas.GetLeft(imgNube_Copy22)));
            nubes.Add(new Elementos(imgNube_Copy23, Canvas.GetLeft(imgNube_Copy23)));
            nubes.Add(new Elementos(imgNube_Copy24, Canvas.GetLeft(imgNube_Copy24)));
            nubes.Add(new Elementos(imgNube_Copy25, Canvas.GetLeft(imgNube_Copy25)));
            nubes.Add(new Elementos(imgNube_Copy26, Canvas.GetLeft(imgNube_Copy26)));
            nubes.Add(new Elementos(imgNube_Copy2, Canvas.GetLeft(imgNube_Copy2)));
            nubes.Add(new Elementos(imgNube_Copy3, Canvas.GetLeft(imgNube_Copy3)));
            nubes.Add(new Elementos(imgNube_Copy4, Canvas.GetLeft(imgNube_Copy4)));
            nubes.Add(new Elementos(imgNube_Copy5, Canvas.GetLeft(imgNube_Copy5)));
            nubes.Add(new Elementos(imgNube_Copy6, Canvas.GetLeft(imgNube_Copy6)));
            nubes.Add(new Elementos(imgNube_Copy7, Canvas.GetLeft(imgNube_Copy7)));
            nubes.Add(new Elementos(imgNube_Copy8, Canvas.GetLeft(imgNube_Copy8)));
            nubes.Add(new Elementos(imgNube_Copy9, Canvas.GetLeft(imgNube_Copy9)));
        }

        void resetLeftElementos(int i)
        {
            foreach (Elementos elemento in elementos[i])
            {
                Canvas.SetLeft(elemento.Imagen, elemento.Left);
                elemento.punto = true;
            }  
        }

        private void moverElementos()
        {
            tiempoActual = cronometroElementos.ElapsedMilliseconds;
            tiempoDiferencial = tiempoActual - tiempoAnterior;
            tiempoAnterior = tiempoActual;

            foreach (Elementos elemento in elementos[0])
            {
                var leftElemento = Canvas.GetLeft(elemento.Imagen);
                Canvas.SetLeft(elemento.Imagen, (leftElemento -= (velocidadEnemigo * tiempoDiferencial) * 0.1));

                if (leftElemento < -150 && elemento.punto){
                    score++;
                    elemento.punto = false;
                }

            }

            if (Canvas.GetLeft(elementos[0][3].Imagen) < 300 || contVueltas)
            {
                foreach (Elementos elemento in elementos[1])
                {
                    var leftElemento = Canvas.GetLeft(elemento.Imagen);
                    Canvas.SetLeft(elemento.Imagen, (leftElemento -= (velocidadEnemigo * tiempoDiferencial) * 0.1));

                    if (leftElemento < -150 && elemento.punto)
                    {
                        score++;
                        elemento.punto = false;
                    }
                }

            }

            if (Canvas.GetLeft(elementos[0][elementos[0].Count - 1].Imagen) < -300 && Canvas.GetLeft(elementos[1][3].Imagen) < 300)
            {
                resetLeftElementos(0);
                contVueltas = true;
            }

            if (Canvas.GetLeft(elementos[1][elementos[1].Count - 1].Imagen) < -300 && Canvas.GetLeft(elementos[0][3].Imagen) < 300)
            {
                resetLeftElementos(1);
                contVueltas = false;
            }
        }

        private bool colisiones(Image imagen1, Image imagen2)
        {
            bool colision = (Canvas.GetLeft(imagen1) + imagen1.ActualWidth > Canvas.GetLeft(imagen2) && Canvas.GetLeft(imagen1) < Canvas.GetLeft(imagen2) + imagen2.ActualWidth) &&
            (Canvas.GetTop(imagen1) < Canvas.GetTop(imagen2) + imagen2.ActualHeight && Canvas.GetTop(imagen1) + imagen1.ActualHeight > Canvas.GetTop(imagen2));

            return colision;
        }

        private void Timer_Tick(object sender, EventArgs e)
		{
			lblPuntos.Text = score.ToString();
            moverElementos();
            moverNubes();


            //Game Over
            foreach (Elementos elemento in elementos[0])
            {
                if (colisiones(Globo, elemento.Imagen))
                {
                    timer.Stop();
					cronometro.Stop();
					waveIn.StopRecording();
					btnIniciar.Content = "Volver a empezar";
                    lblScore.Text = "Score: " + score.ToString();
					btnIniciar.Visibility = Visibility.Visible;
					Fondo.Visibility = Visibility.Visible;
                    lblScore.Visibility = Visibility.Visible;
                }
             
            }

            if (bottomGlobo < 500) 
			{
				Canvas.SetTop(Globo, bottomGlobo++);
			}
			

			//Carro
			//top 500	
			if (frecuenciaActual>=frecuenciaAnterior-25.0f && frecuenciaActual<=frecuenciaAnterior+25.0f)
			{
				//evaluar si ya paso un segundo
				if (cronometro.ElapsedMilliseconds >= 250)
				{
					Canvas.SetTop(Globo, bottomGloboFrecuencia);
					bottomGlobo = bottomGloboFrecuencia;
					cronometro.Restart();
				}


			}
			else
			{
				cronometro.Restart();
			}
            
        }

        private void resetNubes()
        {
            foreach (Elementos elemento in nubes)
            {
                Canvas.SetLeft(elemento.Imagen, elemento.Left);
            }
        }

        private void moverNubes()
        {

            foreach (Elementos elemento in nubes)
            {
                if (Canvas.GetLeft(elemento.Imagen) < -160)
                {
                    Canvas.SetLeft(elemento.Imagen, 1300);
                }
                var leftElemento = Canvas.GetLeft(elemento.Imagen);
                Canvas.SetLeft(elemento.Imagen, (leftElemento -= (velocidadEnemigo * tiempoDiferencial) * 0.1));

            }
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            resetNubes();
			resetLeftElementos(0);
            resetLeftElementos(1);
            btnIniciar.Visibility = Visibility.Hidden;
			Fondo.Visibility = Visibility.Hidden;
            lblScore.Visibility = Visibility.Hidden;

            timer.Start();
            cronometroElementos.Start();
            waveIn = new WaveIn();
            //Formato de audio
            waveIn.WaveFormat =
                new WaveFormat(44100, 16, 1);
            //Buffer
            waveIn.BufferMilliseconds =
                250;
            //¿Que hacer cuando hay muestras disponibles?
            waveIn.DataAvailable += WaveIn_DataAvailable;

            waveIn.StartRecording();

            
			cronometro.Start();
		
			bottomGlobo = (float)Canvas.GetTop(Globo);
		}


		private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
		{
			byte[] buffer = e.Buffer;
			int bytesGrabados = e.BytesRecorded;
			float acumulador = 0.0f;

			double numeroDeMuestras =
				bytesGrabados / 2;
			int exponente = 1;
			int numeroDeMuestrasComplejas = 0;
			int bitsMaximos = 0;

			do
			{
				bitsMaximos = (int)Math.Pow(2, exponente);
				exponente++;
			} while (bitsMaximos < numeroDeMuestras);

			numeroDeMuestrasComplejas = bitsMaximos / 2;
			exponente -= 2;

			Complex[] señalCompleja = new Complex[numeroDeMuestrasComplejas];

			for (int i = 0; i < bytesGrabados; i += 2)
			{
				//Transformando 2 bytes separados
				//en una muestra de 16 bits
				//1.- Toma el segundo byte y el antepone
				//     8 0's al principio
				//2.- Hace un OR con el primer byte, al cual
				//   automaticamente se le llenan 8 0's al final
				short muestra =
					(short)(buffer[i + 1] << 8 | buffer[i]);
				float muestra32bits =
					(float)muestra / 32768.0f;
				acumulador += Math.Abs(muestra32bits);

				if (i / 2 < numeroDeMuestrasComplejas)
				{
					señalCompleja[i / 2].X =
						muestra32bits;
				}

			}
			float promedio = acumulador /
				(bytesGrabados / 2.0f);

			if (promedio > 0)
			{
				FastFourierTransform.FFT(true, exponente, señalCompleja);
				float[] valoresAbsolutos = new float[señalCompleja.Length];
				for (int i = 0; i < señalCompleja.Length; i++)
				{
					valoresAbsolutos[i] = (float)Math.Sqrt((señalCompleja[i].X * señalCompleja[i].X) + (señalCompleja[i].Y * señalCompleja[i].Y));
				}

				int indiceSeñalConMasPresencia = valoresAbsolutos.ToList().IndexOf(valoresAbsolutos.Max());

				float frecuenciaFundamental = (float)(indiceSeñalConMasPresencia * waveIn.WaveFormat.SampleRate) / (float)valoresAbsolutos.Length;


				if (frecuenciaFundamental < 1200 && frecuenciaFundamental > 100)
				{
					bottomGloboFrecuencia = Math.Abs((frecuenciaFundamental - 200) * 2 - 600);
				}

				if (bottomGloboFrecuencia < 0)
				{
					bottomGloboFrecuencia = 0;
				} else if (bottomGloboFrecuencia > 500)
				{
					bottomGloboFrecuencia = 500;
				}


				frecuenciaAnterior = frecuenciaActual;

				frecuenciaActual = frecuenciaFundamental;
			}
		}

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
        }
    }
}
