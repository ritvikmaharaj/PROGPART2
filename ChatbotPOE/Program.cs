using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Speech.Synthesis;

namespace ChatbotPOE
{
    internal class Program
    {

        static List<string> chatHistory = new List<string>();

        static SpeechSynthesizer synth = new SpeechSynthesizer
        {
            Volume = 100,
            Rate = 0
        };

        static Random random = new Random();




        static void Main()
        {
            // Audio plays welcome message
            PlayGreetingAudio("greeting.wav");

            Console.Title = "Cybersecurity chatbot";
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@"

     _____ _   _ ____  _____ ____  _____  ______ _____  _   _ _____ _______ __     __
  / ____| \ | |  _ \| ____/ __ \|  __ \|  ____|  __ \| \ | |_   _|__   __|\ \   / /
 | |    |  \| | |_) | |__| |  | | |__) | |__  | |__) |  \| | | |    | |    \ \_/ / 
 | |    | .  |  _ <|  __| |  | |  _  /|  __| |  _  /| .  | | |    | |     \   /  
 | |____| |\  | |_) | |___ |__| | | \ \| |____| | \ \| |\  |_| |_   | |      | |   
  \_____|_| \_|____/|______\____/|_|  \_\______|_|  \_\_| \_|_____|  |_|      |_|        

               ");

            Console.Title = "Cybersecurity Assistant Chatbot";
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" Welcome to your Cybersecurity Assistant Chatbot!");
            Console.Write(" What’s your name? ");
            Console.ForegroundColor = ConsoleColor.White;
            string userName = Console.ReadLine();


            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"Hello, {userName}! I’m here to bring awareness around cybersecurity");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{userName}: ");
                string userInput = Console.ReadLine()?.ToLower().Trim();


                if (string.IsNullOrEmpty(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Chatbot: Please enter a valid question.");
                    continue;
                }

                if (userInput == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Chatbot: Stay safe and alert! See you next time! ");
                    break;
                }
                HandleUserQuery(userInput, userName);

            }
        }





        // METHOD TO PLAY GREETING AUDIO
        static void PlayGreetingAudio(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);  // Gets the full path

                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.PlaySync();  // Play the audio synchronously
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: The file '{filePath}' was not found at the specified location.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }




        static List<string> userInterests = new List<string>();
        static string GetInterest(string input)
        {
            string[] interest = { "i'm interested in ", "i am interested in ", "my interest is ", "i like " };

            foreach (var phrase in interest)
            {
                if (input.StartsWith(phrase))
                {
                    string userInterest = input.Substring(phrase.Length).Trim();
                    return userInterest;
                }
            }

            return null;
        }

        static string lastUserInput = "";




        // DICTIONARY
        static void HandleUserQuery(string input, string userName)
        {
            string newInterest = GetInterest(input);

            if (newInterest != null)
            {
                if (!userInterests.Contains(newInterest))
                {
                    userInterests.Add(newInterest);
                    RespondWithSpeech($"Great! I'll remember that you're interested in {newInterest}. It's an important topic!");
                    return;
                }
                else
                {
                    RespondWithSpeech($"You already told me you're interested in {newInterest}.");
                    return;
                }
            }
            // Sentiment keywords to detect
            string[] sentimentKeywords = { "worried", "frustrated", "curious" };

            // Check if user input contains any sentiment keywords
            if (sentimentKeywords.Any(word => input.Contains(word)))
            {
                lastUserInput = input;
                RespondWithSpeech("I sense you're feeling " +
                    sentimentKeywords.First(word => input.Contains(word)) +
                    ". Would you like me to share some tips on cybersecurity?");
                return;  // Wait for user to respond before continuing
            }
            if (input == "yes")
            {
                SentimentTips();
                return;
            }
            else if (input == "no")
            {
                RespondWithSpeech("No problem! Let me know if you want tips later.");
                return;
            }



            Dictionary<string, string> responses = new Dictionary<string, string>
            {
                {"what can i ask you about", "You can ask about phishing scams, safe password practice, recognising suspicious links, two factor authentication and device security"  },
                {"how are you", "I'm great! I'm here to help you stay safe online" },
                {"what is your purpose", "My purpose is to help you stay informed and safe when it comes to cybersecurity" },
                {  "help", "You can ask about: 'phishing scams', 'safe password practice', 'recognising suspicious links', 'two factor authentication', 'device security'. You can also ask me to 'suggest tips' on cybersecurity topics." },

                { "safe password practice", " Make use of various numbers, letters and symbols. Never includes things such as your name or birthdate." +
                "Use different passwords for all your accounts that are each strong and unique" },
                { "phishing scams", "Phishing scams trick you into giving away personal information such as passwords or bank details." +
                "It may occur in the form of  email or text messages that appears to be from legitimate organisations but are actually not." +
                "Always look out for suspicious links and check the senders email address" },
                { "recognising suspicious links", " A suspicious link is one that may appear legitimate but is actually designed to redirect you to a site where a hacker can access sensitive information." +
                "A common way in which this is achieved is by replacing certain characters from the legitimate website that look similar to others." +
                "For example, www.paypal.com, the hacker may replace the L with 1: www.paypa1.com" },
                { "two factor authentication", " Two factor authentication or 2FA is the process of adding an additional layer of secuirty to your accounts." +
                "So after entering your password, you'll be required to show your face or give your fingerprint." +
                "This additional layer of secuirty is beneficial as someone may have your password but will not be given access." },
                { "device security", " Keep your software and apps updated, make use of antivirus sotware and avoid using public Wi-Fi without VPN." },
                {"what is my name", "Your name is " + (userName) }
            };






            // KEY WORD RESPONSE
            Dictionary<string, List<string>> keywordGroups = new Dictionary<string, List<string>>()
    {
        { "safe password practice", new List<string> { "password", "strong password", "secure password", "password tips" } },
        { "phishing scams", new List<string> { "scam","phishing", "phishing emails", "scam emails", "fake messages" } },
        { "recognising suspicious links", new List<string> { "suspicious links", "fake links", "phishing links", "dangerous websites" } },
        { "two factor authentication", new List<string> { "2fa", "two factor", "authentication", "multi-factor" } },
        { "device security", new List<string> { "device security", "secure my device", "phone safety", "antivirus", "vpn", "software updates" } },
        { "what is my name", new List<string> { "name", "my name", "whats my name", "who am i" } },
    };
            bool foundResponse = false;


            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Chat log: {userName}: {input}]");

            if (input.Contains("suggest tips"))
            {
                SuggestTips();
                foundResponse = true;
                return;
            }



            foreach (var entry in responses)
            {
                if (input.Contains(entry.Key))
                {
                    RespondWithSpeech(entry.Value);
                    foundResponse = true;
                    break;
                }
            }


            // Match synonyms
            if (!foundResponse)
            {
                foreach (var group in keywordGroups)
                {
                    foreach (var synonym in group.Value)
                    {
                        if (input.Contains(synonym))
                        {
                            RespondWithSpeech(responses[group.Key]);
                            foundResponse = true;
                            break;
                        }
                    }
                    if (foundResponse) break;
                }
            }

            // No match found
            if (!foundResponse)
            {
                RespondWithSpeech("Sorry, I didn't understand. Type 'help' to see what you can ask.");
            }
        }




        // SUGGEST TIPS METHOD
        static void SuggestTips()
        {
            // Arrays of tips for each category
            string[] personalProtectionTips = {
            "Make use of strong, unique passwords.",
            "Use multifactor authentication.",
            "Limit what you share online.",
            "Make use of a VPN while on public Wi-Fi."
        };

            string[] deviceSecurityTips = {
            "Stay updated with the latest software.",
            "Ensure your device has protection like a password or fingerprint lock.",
            "Don't download apps from suspicious sites.",
            "Use reputable antivirus software."
        };

            string[] scamAwarenessTips = {
            "Stay away from those that demand money or payment of some sort.",
            "Look out for offers that are too good to be true – they probably are.",
            "Always double-check who the information is coming from.",
            "Don’t click on links from unknown sources."
        };

            string matchedInterest = userInterests.FirstOrDefault(i =>
    i.Contains("protection") || i.Contains("device") || i.Contains("account") ||
    i.Contains("scams") || i.Contains("phishing")
);

            if (matchedInterest != null)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"As someone interested in {matchedInterest}, I have some tips that might help you.");
            }

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" Chatbot: What would you like to know about? (personal protection/device and account security/scam awareness)");
            Console.ForegroundColor = ConsoleColor.White;
            string goal = Console.ReadLine()?.ToLower().Trim();

            // Create Random instance
            Random rand = new Random();

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (goal == "personal protection")
            {
                string tip = personalProtectionTips[rand.Next(personalProtectionTips.Length)];
                Console.WriteLine(" Keep your personal information and identity safe:\n- " + tip);
            }
            else if (goal == "device and account security")
            {
                string tip = deviceSecurityTips[rand.Next(deviceSecurityTips.Length)];
                Console.WriteLine(" Protect your devices and accounts from threats:\n- " + tip);
            }
            else if (goal == "scam awareness")
            {
                string tip = scamAwarenessTips[rand.Next(scamAwarenessTips.Length)];
                Console.WriteLine(" Be aware of scams:\n- " + tip);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Chatbot: That’s not a recognized goal. Try 'personal protection', 'device and account security', or 'scam awareness'.");
            }
        }

        static void SentimentTips()
        {
            if (lastUserInput.Contains("im worried about scams"))
            {
                RespondWithSpeech("Scammers can be very convincing, therefore its important to verify who you are dealing with");
            }
            else if (lastUserInput.Contains("password"))
            {
                RespondWithSpeech("Strong passwords are key to ensuring no one can access your accounts");
            }
            else if (lastUserInput.Contains("device") || lastUserInput.Contains("security") || lastUserInput.Contains("vpn"))
            {
                RespondWithSpeech("Device security is important  because it protects your personal data, prevents unauthorized access, and ensures your digital activities remain private and safe from cyber threats");
            }
            else
            {
                RespondWithSpeech("Here are some general cybersecurity tips: Combine uppercase, lowercase, numbers, and symbols, avoid using personal info (like birthdates or names), use a password manager to keep track.");
            }
        }






        // CHATBOT VOICE
        static void RespondWithSpeech(string response)
        {
            LoadingEffect();

            Console.ForegroundColor = ConsoleColor.Blue;
            TypingEffect($"ChatBot: {response}\n");

            try
            {
                synth.Speak(response); // Reliable speech output
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"TTS Error: {ex.Message}");
            }

            chatHistory.Add($"ChatBot: {response}");
        }

        // LOADING EFFECT METHOD
        static void LoadingEffect()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("ChatBot");

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(400);
                Console.Write(". ");
            }

            Console.WriteLine();
        }


        // TYPING EFFECT METHOD
        static void TypingEffect(string message, int delay = 30)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
        }
        static void SaveChatHistory(string[] chatHistory)
        {
            string path = "chat_history.txt";

            File.WriteAllLines(path, chatHistory);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Chat history saved to {path}");
        }

        // Method to save users name and favorite topic
        static void SaveUserData(string name, string topic)
        {
            string path = "data.txt";
            string[] lines = { name, topic };
            File.WriteAllLines(path, lines);
        }

        static void LoadUserData(out string name, out string topic)
        {
            string path = "data.txt";
            name = "";
            topic = "";

            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Length >= 2)
                {
                    name = lines[0];
                    topic = lines[1];
                }
            }
        }


    }
}


