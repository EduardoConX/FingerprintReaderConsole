using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerprintReaderConsoleNet
{
    internal class Program: Capturer
    {
        public delegate void OnTemplateEventHandler(DPFP.Template template);
        private DPFP.Template Template;
        private DPFP.Processing.Enrollment Enroller;
        public string encoded;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Init();
        }

        protected override void Init()
        {
            base.Init();
            Enroller = new DPFP.Processing.Enrollment();            // Create an enrollment.
            UpdateStatus();
            base.Start();
            Console.ReadKey();
        }

        private void OnTemplate(DPFP.Template template)
        {
            Template = template;
            if (Template != null)
            {
                Console.WriteLine("La huella fue registrada exitosamente");
                byte[] streamHuella = Template.Bytes;

                encoded = Convert.ToBase64String(streamHuella);
                Console.WriteLine(encoded);
            }
            else
            {
                Console.WriteLine("El registro de huela digital falló");
            }
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);

            // Process the sample and create a feature set for the enrollment purpose.
            DPFP.FeatureSet features = base.ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) try
                {
                    Console.WriteLine("Se creó el set de huellas dactilares.");
                    Enroller.AddFeatures(features);     // Add feature set to template.
                }
                finally
                {
                    UpdateStatus();

                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:   // report success and stop capturing
                            OnTemplate(Enroller.Template);            
                            Stop();
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:  // report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            UpdateStatus();
                            OnTemplate(null);
                            base.Start();
                            break;
                    }
                }
        }

        private void UpdateStatus()
        {
            // Show number of samples needed.
            Console.WriteLine(String.Format("Muestras faltantes: {0}", Enroller.FeaturesNeeded));
        }
    }
}
