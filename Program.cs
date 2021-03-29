using System;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace SampleRecognition
{
    class Program
    {
        private static SpeechRecognitionEngine recognizer;
        public static void Main(string[] args)
        {
            
            // Initialize an in-process speech recognition engine and set its input.
            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();

            // Add a handler for the LoadGrammarCompleted event.
            recognizer.LoadGrammarCompleted +=
              new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);

            // Add a handler for the SpeechRecognized event.
            recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            // Create the "yesno" grammar.
            Choices yesChoices = new Choices(new string[] { "yes", "yup", "yeah" });
            SemanticResultValue yesValue =
                new SemanticResultValue(yesChoices, (bool)true);
            Choices noChoices = new Choices(new string[] { "no", "nope", "neah" });
            SemanticResultValue noValue =
                new SemanticResultValue(noChoices, (bool)false);
            SemanticResultKey yesNoKey =
                new SemanticResultKey("yesno", new Choices(new GrammarBuilder[] { yesValue, noValue }));
            Grammar yesnoGrammar = new Grammar(yesNoKey);
            yesnoGrammar.Name = "yesNo";

            // Create the "done" grammar.
            Grammar doneGrammar =
              new Grammar(new Choices(new string[] { "open chrome", "close chrome", "facebook", 
                  "what is the time now", "whats the date today", "what is your name" }));
            doneGrammar.Name = "Done";

            // Create a dictation grammar.
            //Grammar dictation = new DictationGrammar();
            //dictation.Name = "Dictation";

            // Load grammars to the recognizer.
            recognizer.LoadGrammarAsync(yesnoGrammar);
            recognizer.LoadGrammarAsync(doneGrammar);
            //recognizer.LoadGrammarAsync(dictation);

            // Start asynchronous, continuous recognition.
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            // Keep the console window open.
            Console.ReadLine();
        }

        // Handle the LoadGrammarCompleted event. 
        static void recognizer_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            string grammarName = e.Grammar.Name;
            bool grammarLoaded = e.Grammar.Loaded;

            if (e.Error != null)
            {
                Console.WriteLine("LoadGrammar for {0} failed with a {1}.",
                grammarName, e.Error.GetType().Name);

                // Add exception handling code here.
            }

            Console.WriteLine("Grammar {0} {1} loaded.",
            grammarName, (grammarLoaded) ? "is" : "is not");
        }

        // Handle the SpeechRecognized event.
        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            
            Console.WriteLine("Grammar({0}): {1}", e.Result.Grammar.Name, e.Result.Text);
            
            if (e.Result.Text == "open chrome")
            {
                synth.Speak("chrome opened");
                Process.Start("chrome.exe");
            }

            if (e.Result.Text == "close closed")
            {
                synth.Speak("chrome opened");
                foreach (var process in Process.GetProcessesByName("notepad"))
                {
                    process.Kill();
                };
            }

            if (e.Result.Text == "facebook")
            {
                synth.Speak("facebook");
                Process.Start("chrome.exe","http://www.facebook.com");
                
            }

            if (e.Result.Text == "what is the time now")
            {
                synth.Speak("the time now is" + DateTime.Now.ToShortTimeString());
                

            }

            if (e.Result.Text == "whats the date today")
            {
                synth.Speak("Todays date is" + DateTime.Today.ToString("dd-MM-yyyy"));


            }

            if (e.Result.Text == "what is your name")
            {
                synth.Speak("My name is John");


            }

            
            // Add event handler code here.
        }
    }
}