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

        List<Image> elementos = new List<Image>();
        
		

		public MainWindow()
        {
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Tick += Timer_Tick;

			
			cronometro = new Stopwatch();
            cronometroElementos = new Stopwatch();
			InitializeComponent();
        }

        private void llenarListaElementos()
        {
            elementos.Add(imgBatman);
            elementos.Add(imgEdificio);
            elementos.Add(imgNube);
            elementos.Add(imgOvni);
            elementos.Add(imgPutin);
           
        }
        private void moverElementos()
        {
            tiempoActual = cronometroElementos.ElapsedMilliseconds;
            tiempoDiferencial = tiempoActual - tiempoAnterior;
            tiempoAnterior = tiempoActual;

            foreach (Image imagen in elementos)
            {
                var leftElemento = Canvas.GetLeft(imagen);
                Canvas.SetLeft(imagen, (leftElemento -= (velocidadEnemigo * tiempoDiferencial) * 0.1));
                
            }
        }
        private bool colisiones(Image imagen1, Image imagen2)
        {
            
            return
                (Canvas.GetLeft(imagen1) + imagen1.ActualWidth > Canvas.GetLeft(imagen2) && Canvas.GetLeft(imagen1) < Canvas.GetLeft(imagen2) + imagen2.ActualWidth) &&
                (Canvas.GetBottom(imagen1) < Canvas.GetBottom(imagen2) + imagen2.ActualHeight && Canvas.GetBottom(imagen1) + imagen1.ActualHeight > Canvas.GetBottom(imagen2))
           
                ;
        }

        private void Timer_Tick(object sender, EventArgs e)
		{
			lblFrecuencia.Text = frecuenciaActual.ToString("f");

            if (colisiones(Globo,elementos[0]))
            {
                lbl_frecuencia200.Text = "colision";
               
            }
            lbl_frecuencia500.Text = Canvas.GetRight(Globo).ToString();
            lbl_frecuencia355.Text = Canvas.GetRight(elementos[0]).ToString();
            if (bottomGlobo > 0)
			{
				//Canvas.SetBottom(Globo, bottomGlobo--);
			}
			

			//Carro
			//top 500	
			if (frecuenciaActual>=frecuenciaAnterior-25.0f && frecuenciaActual<=frecuenciaAnterior+25.0f)
			{
				//evaluar si ya paso un segundo
				if(cronometro.ElapsedMilliseconds >= 500)
				{
					Canvas.SetBottom(Globo, bottomGloboFrecuencia);
					bottomGlobo = bottomGloboFrecuencia;
					cronometro.Restart();
				}


			}
			else
			{
				cronometro.Restart();
			}
            moverElementos();
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
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

            llenarListaElementos();

			cronometro.Start();
		
			bottomGlobo = (float)Canvas.GetBottom(Globo);
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


				if (frecuenciaFundamental < 700 && frecuenciaFundamental > 100)
				{
					bottomGloboFrecuencia = (frecuenciaFundamental - 200) * 2;
				}

				if (bottomGloboFrecuencia < 0)
				{
					bottomGloboFrecuencia = 0;
				} else if (bottomGloboFrecuencia > 600)
				{
					bottomGloboFrecuencia = 600;
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
