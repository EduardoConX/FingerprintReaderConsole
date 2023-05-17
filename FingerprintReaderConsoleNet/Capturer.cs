using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerprintReaderConsoleNet
{
    public partial class Capturer : DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture _capturer;
        private DPFP.Processing.Enrollment _enroller;
        protected virtual void Init()
        {
            try
            {
                _capturer = new DPFP.Capture.Capture();               // Create a capture operation.

                if (null != _capturer)
                {
                    _capturer.EventHandler = this;                    // Subscribe for capturing events.
                    Console.WriteLine("Operación de captura inicializada correctamente");
                }
                else
                {
                    Console.WriteLine("¡No se pudó inicializar la operación de captura!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("¡No se pudó inicializar la operación de captura!");
                Console.WriteLine(ex.Message);
            }
        }

        protected virtual void Process(DPFP.Sample Sample)
        {
            // Draw fingerprint sample image.
            Console.WriteLine("Dibujando");
        }

        protected void Start()
        {
            if (null != _capturer)
            {
                try
                {
                    _capturer.StartCapture();
                    Console.WriteLine("Escanea tu huella usando el lector");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se puede iniciar la captura");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected void Stop()
        {
            if (null != _capturer)
            {
                try
                {
                    _capturer.StopCapture();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("No se puede terminar la captura");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            Console.WriteLine("La muestra de huellas dactilares fue capturada.");
            Console.WriteLine("Escanea la misma huella dactilar de nuevo.");
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            Console.WriteLine("El dedo fue retirado del lector de huellas dactilares..");
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            Console.WriteLine("El lector de huellas digitales fue tocado.");
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            Console.WriteLine("El lector de huellas digitales fue conectado.");
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            Console.WriteLine("El lector de huellas digitales estaba desconectado.");
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            if (CaptureFeedback == DPFP.Capture.CaptureFeedback.Good)
                Console.WriteLine("La calidad de la muestra de huellas dactilares es buena.");
            else
                Console.WriteLine("La calidad de la muestra de huellas dactilares es mala.");
        }
        #endregion

        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();  // Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);            // TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }
    }
}
