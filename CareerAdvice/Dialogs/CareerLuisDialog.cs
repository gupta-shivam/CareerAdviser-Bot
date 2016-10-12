using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using QC = System.Data.SqlClient;
using DT = System.Data;
using System.Web.Script.Serialization;
using System.Globalization;

namespace CareerAdvice.Dialogs
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
    public class Value
    {
        public List<string> ColumnNames { get; set; }
        public List<string> ColumnTypes { get; set; }
        public List<List<string>> Values { get; set; }
    }

    public class Output1
    {
        public string type { get; set; }
        public Value value { get; set; }
    }

    public class Results
    {
        public Output1 output1 { get; set; }
    }

    public class RootObject
    {
        public Results Results { get; set; }
    }

    public class Document
    {
        public double score { get; set; }
        public string id { get; set; }
    }

    public class RootObject_Senti
    {
        public List<Document> documents { get; set; }
        public List<object> errors { get; set; }
    }

    [LuisModel("d45f9973-e944-4056-959e-c6f6a1978095", "8c49b30c27044a7ab894d3ee9be02f78")]
    [Serializable]
    public class CareerLuisDialog : LuisDialog<object>
    {
        public string[] greetings = {"Hi!",
                    "Hello There!",
                    "Hey How do you do?",
                    "At Your Service...",
                    "Hello,How’s it going? ",
                    "Hi,How are you doing?",
                    "Hi,Good to see you ",
                    "Hi,Nice to see you"
                };

        public string[] help = {"Ask me Career Advice, Tell me about Yourself :)",
                    "I give Career Help, Ask me Anything",
                    "I can asist you in finding correct Career choice",
                    "I would be glad to help you in finding a perfect career",
                    "I am good in predecting right Career choice.wanna try?",
                    "i am a self learning bot to predict Career"
                };

        public string[] endings = {"Bye!",
                    "Good Bye",
                    "Thanks,See you later",
                    "I am glad we got to talk",
                    "Thank you very much for your time",
                    "Nice to meet you.Have a nice day!",
                    "Nice to meet you.see you soon"
                };

        public string[] fail = { "Sorry I could not Understand What you are Saying.",
                        "I don’t understand this. Can you please explain it?" ,
                        "Can you please repeat that?" ,
                        "Sorry for that,would you please repeat",
                        "I could not get you,would you please repeat"
        };

        public string[] feedback = { "We would love to have your Feedback",
                        "Did you find my advice Satisfactory?" ,
                        "Do you have any feedback for us?"
        };

        public string[] info = { "subject",
            "education",
            "interests"
        };

        static public string[] getedu = { "are you a postgrad or undergrad?",
                    "So what all have you done in terms of Academics till now?",
                    "tell me something about your degrees",
                    "have you done masters?"
                };
        static public string[] getinterests = { "What are your interests?",
                    "What are some of the things that you like to do?" ,
                    "Could you tell me about your interests?",
                    "what are the thigs you like to do?",
                    "what are the interests you want to pursue?"
                };
        static public string[] getsubjects = { "What Subjects did you take in Academics?",
                    "What all Subjects do you know?" ,
                    "what all subjects do you learn in academics?",
                    "Could you tell me about your subjects?",
                    "what are the subjects you study?"
                };

        public Dictionary<string, string[]> ques = new Dictionary<string, string[]>(){
                    { "education" , getedu },
                    { "interests" , getinterests},
                    { "subject" , getsubjects }
                };
        // enrich this list
        static public string[] underGrad = { "BTech","B Tech","B.sc","B Sc","B.Sc","B A","B.A",
           "B.Com","B Com","Undergrad","MBBS","Under Graduate","under grad","Undergraduate"};

        static public string[,] subs = {
            {"CSE","Computer Science","computer","computerscience" },
            {"MNC","Maths","mathematics","Maths and computing"},
            {"ECE","electronics","electronics and communication","communication" },
            { "EEE","electrical","electrical","electrical"},
            { "Civil","Civil engineering","architect","Civil"},
            { "Mech","Mechanical engineering","mechanical","me"},
            { "ep","physics","engineering physics","e p"},
            { "Bio tech","biotech","biotechnology","bio technology"},
            { "Chemical","Chemical engineering","chem","chemist"},
            { "Eco","economics","hss","economics"},
            { "BS","business","business studies","bs"},
            { "MBA","management","master of business administration","administration"},
            { "ca","chartered accountant","accountant","commerce"},
            { "Medical","doctor","medicine","medicine"},
            { "MBBS","MB BS","M B B S","mbbs"},
            { "dentist","dental","dentist","dentist"},
            { "vetnary","vet","vet","vet"},
            { "litr","literature","reading","author"},
            { "Cinema","film","film industry","movie"},
            { "hss","social science","history","humanities"}

        };

        static public string[,] inters = {
            {"Coding","Code","competitive coding","coding" },
            {"study","researching","studying","research"},
            {"machine learning","ml","m l","machinelearning" },
            { "development","developing","dev","develop"},
            { "puzzle","solving puzzle","solve puzzle","puzzle"},
            { "architect","architecture","architect","architecture"},
            { "read","to read","reading","read"},
            { "cars","auto indusrty","automobiles","automobile"},
            { "teach","teaching","teacher","to teach"},
            { "management","managing","manager","manage"},
            { "enterprenuer","business","startup","company"},
            { "to act","actor","acting","drama"},
            { "photo","photography","photographer","photo"},
            { "reporter","journal","journalist","journalism"},
            {"chef","cooking","cook","chef" },
            { "movie","films","film","movies"},
            { "shares","stock market","stock","stocks"},
            { "interact","interacting","to interact","interact"},
            { "music","songs","instrument","play"},
            { "surgeon","operation","surgeory","surgeon"}

        };

        //remove 2 careers
        static public string[] careers =
        {
          "archaelogy",
          "archaeology",
          "architect",
          "autoIndustry",
          "consultancyServices",
          "Critic",
          "cryptography",
          "Docotr",
          "Doctor",
          "filmIndustry",
          "Finance",
          "higherStudies",
          "hotelManagement",
          "investmentBanking",
          "IT",
          "journalist",
          "MBA",
          "Proffesor",
          "robotics",
          "startUp"
        };

        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(fail);
            await context.PostAsync(fail[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("delete")]
        public async Task Delete(IDialogContext context, LuisResult result)
        {
            context.ConversationData.Clear();
            await context.PostAsync("Data Successfully Deleted!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("greeting")]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(greetings);
            await context.PostAsync(greetings[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("end")]
        public async Task EndConv(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(endings);
            await context.PostAsync(endings[idx]);
            context.Wait(MessageReceived);
        }

        [LuisIntent("help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            int idx = getRandomString(help);
            await context.PostAsync(help[idx]);
            await context.PostAsync("To Delete User Data and Start Afresh just type: delete user data");
            context.Wait(MessageReceived);
        }

        [LuisIntent("info")]
        public async Task Process(IDialogContext context, LuisResult result)
        {
            string entity_kind = "";
            string entity = "";

            for (int i = 0; i < result.Entities.Count; i++)
            {
                entity_kind = result.Entities[i].Type;
                entity = result.Entities[i].Entity;
                context.ConversationData.SetValue<string>(entity_kind, entity);
            }

            string s = getMessage(context, result);

            if (s == "")
            {
                await context.PostAsync("Let me See What I can Find for You! :)");
                await onComplete(context, result);
            }
            else
            {
                int idx = getRandomString(ques[s]);
                await context.PostAsync(ques[s][idx]);
                context.Wait(MessageReceived);
            }
        }

        public string getMessage(IDialogContext context, LuisResult result)
        {
            //Returns the Required Entities or "" if all the Entities have been acquired
            string a = "";
            string entity = "";
            for (int i = 0; i < info.Length; i++)
            {
                if (!context.ConversationData.TryGetValue(info[i], out a))
                {
                    entity = info[i];
                    break;
                }
            }
            return entity;
        }

        [LuisIntent("needanswer")]
        public async Task onComplete(IDialogContext context, LuisResult result)
        {
            //-> Tockenize entities 
            //-> Form Feature Vector
            //-> Get Response from ML API
            //-> Update choiceGiven Var

            //Tockenize and Form Feature Vector
            int[] fv = await getBoolVector(context,result);
            if (fv == null) {
                await Process(context, result);
                context.Wait(MessageReceived);
            }

            //Query ML API
            var client = new HttpClient();

            var scoreRequest = new
            {
                Inputs = new Dictionary<string, StringTable>() {
                    {
                        "input1",
                        new StringTable()
                        {
                            ColumnNames = new string[] {"underGrad", "postGrad", "CSE", "MNC", "ECE", "EEE", "Civil", "Mech", "EP", "Biotech", "Chem", "Eco", "BS", "MBA", "Accounts", "Medical", "MBBS", "Dentist", "Vet", "Litr", "Cinema", "HSS", "Coding", "Research", "ML", "Dev", "Puzzle", "Arch", "Reading", "Autom", "Teach", "Management", "Entrepreneur", "Drama", "Photo", "Journal", "Chef", "movie", "Stocks", "interact", "music", "surgeon", "Result"},
                            Values = new string[,] {  { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value" },
                                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "value" }
                                                    }

                        }
                    },
                },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };

            string[] ML = new string[2] { "0", "1" };
            for (int i = 0; i < 2; i++){
                for (int j = 0; j < 42; j++){
                    scoreRequest.Inputs["input1"].Values[i, j] = ML[fv[j]];
                }
            }

            const string apiKey = "qmt16vqLEHioB/UvEMGmuvj6eJtUf48f0fiKujXiicQPDLXcHbXSpvp9wVqxunDASsoA5iFAbsmNPLC4WVhkgA=="; // Replace this with the API key for the web service
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.BaseAddress = new Uri("https://asiasoutheast.services.azureml.net/workspaces/257eee5d631f454797f6146f812c0f48/services/835a9e1a32be469dad38570a87ffbffa/execute?api-version=2.0&details=true");
            HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

            if (response.IsSuccessStatusCode)
            {
                string result2 = await response.Content.ReadAsStringAsync();
                //await context.PostAsync("Result: " + result2);

                JavaScriptSerializer ser = new JavaScriptSerializer();

                // This deserialization will only work if the corresponding C# classes are defined for JSON.
                RootObject res = ser.Deserialize<RootObject>(result2);

                //await context.PostAsync("JSON CHECK: " + res.Results.output1.type);

                // here goes code for getting top 5
                // CALL FEEDBACK

                List<float> Scores = new List<float>();
                for (int i = 43; i < 63; i++) // make 61 if 2 carrers removed
                {
                    string sc = res.Results.output1.value.Values[0][i];
                    //await context.PostAsync("You have " + sc);
                    float score = float.Parse(sc, CultureInfo.InvariantCulture.NumberFormat);
                    //await context.PostAsync("You have float " + score);

                    Scores.Add(score);
                }
                var sorted = Scores
                            .Select((x, i) => new KeyValuePair<float, int>(x, i))
                            .OrderBy(x => x.Key)
                            .ToList();

                List<float> sortedSc = sorted.Select(x => x.Key).ToList();
                List<int> sortedIdx = sorted.Select(x => x.Value).ToList();

                await context.PostAsync("Check out the following fields");
                string cg = "";
                cg = careers[sortedSc.Count - 1];
                for (var i = sortedSc.Count - 1; i >= sortedSc.Count - 3; i--)
                {
                    var curSc = sortedSc[i];
                    var curIdx = sortedIdx[i];
                    //await context.PostAsync("SORT val " + curSc);
                    //await context.PostAsync("SORT idx " + sortedIdx[i]);
                    await context.PostAsync(careers[curIdx]);
                }
                context.ConversationData.SetValue<string>("choiceGiven",cg);

                //REMOVE AT LAST un wanted
                /* var jsres = JsonConvert.SerializeObject(response.Content);
                 dynamic dynJson = JsonConvert.DeserializeObject(jsres);
                */
                //dynamic dynJson2 = JsonConvert.DeserializeObject(dynJson.Results);
                /*dynamic dynJson3 = JsonConvert.DeserializeObject(dynJson2.output1);*/
                /*
                 dynamic res = JsonConvert.DeserializeObject(response.Content.ToString());
                 await context.PostAsync("JSON CHECK: " + res.Results.output1.type);*/
                //JavaScriptSerializer ser = new JavaScriptSerializer();
                //dynamic res = JsonConvert.DeserializeObject(result2);
                // This deserialization will only work if the corresponding C# classes are defined for JSON.
                /* dynamic myresults = ser.Deserialize<dynamic>(result2);*/
                // await context.PostAsync("JSON CHECK: " + res.Results.output1.type);

                int idx = getRandomString(feedback);
                await context.PostAsync(feedback[idx]);
            }
            else
            {
                await context.PostAsync("Oops! Some error occured");

                // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                /*  Console.WriteLine(response.Headers.ToString());

                  string responseContent = await response.Content.ReadAsStringAsync();
                  Console.WriteLine(responseContent);*/
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("feedback")]
        public async Task ProcessFeedback(IDialogContext context, LuisResult result)
        {
            int res = await getSentiment(result.Query, context, result);
            if (res == 1)
            {
                int[] r = await getBoolVector(context, result);//Only includes the columns apart from Career Prediction
                if (r != null)
                {
                    string row = "";
                    for (int i = 0; i < r.Length; i++)
                    {
                        row += r[i].ToString() + ",";
                    }
                    string cg;
                    if (context.ConversationData.TryGetValue("choiceGiven", out cg))
                    {
                        row += "'" + cg + "'";//add value for career prediction column
                        using (var connection = new QC.SqlConnection(
                                "Server = tcp:careerpredicitor.database.windows.net,1433; Initial Catalog = testCarrer; Persist Security Info = False; User ID=Shivram; Password=code#123; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"
                        ))
                        {
                            connection.Open();
                            Console.WriteLine("Connected successfully.");
                            InsertRows(row, connection);
                            Console.WriteLine("Press any key to finish...");
                        }
                    }
                }
            }
            await context.PostAsync("Thank you for your valuable Feedback");
            context.Wait(MessageReceived);
        }

        public int getRandomString(string[] a)
        {
            Random rnd = new Random();
            int idx = rnd.Next(0, a.Length);
            return idx;
        }

        static async Task<int> getSentiment(string body, IDialogContext context, LuisResult result, int t = 0)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "178cce3645ad47ab9dd771267be479f9");
            
            var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"documents\":[" +
                    "{\"id\":\"1\",\"text\":\""+body+"\"},");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/json");
                try
                {
                    response = await client.PostAsync(uri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string res_string = await response.Content.ReadAsStringAsync();
                        JavaScriptSerializer ser = new JavaScriptSerializer();
                        RootObject_Senti res = ser.Deserialize<RootObject_Senti>(res_string);
                        double score = res.documents[0].score;

                        if (score > 0.5)
                            return 1;
                    }
                }
                catch(HttpRequestException e){
                    if(t<3)
                        await getSentiment(body, context, result,t+1);
                    //Try to connect to API 3 times if fails then no can do.
                }
            }
            return 0;
        }

        //forms the boolean vector
        async Task<int[]> getBoolVector(IDialogContext context, LuisResult result, bool verbose = false) {
            string s = getMessage(context,result);
            if (s != "") {
                return null;
            }
            int[] fv = new int[42];
            for (int i = 0; i < fv.Length; i++)
            {
                fv[i] = 0;
            }

            string key = "education";
            string edu = context.ConversationData.Get<string>(key);
            Boolean checkUnder = underGrad.Contains(edu, StringComparer.OrdinalIgnoreCase);

            if (checkUnder)
            {
                if(verbose)
                    await context.PostAsync("you are UnderGrad");
                fv[0] = 1;
            }
            else
            {
                if (verbose)
                    await context.PostAsync("you are PostGrad");
                fv[1] = 1;
            }

            key = "subject";
            string subj = context.ConversationData.Get<string>(key);

            int id = -1;
            for (int i = 0; i < subs.GetLength(0); i++)
            {
                for (int j = 0; j < subs.GetLength(1); j++)
                {
                    string tem = subs[i, j];
                    if (subj.Equals(tem, StringComparison.InvariantCultureIgnoreCase))
                    {
                        id = i;
                        break;
                    }
                }

            }

            if (id == -1) id = 0;
            id += 2;
            fv[id] = 1;
            if (verbose)
                await context.PostAsync("you study " + subs[id - 2, 0]);

            key = "interests";
            string intr = context.ConversationData.Get<string>(key);

            id = -1;
            for (int i = 0; i < inters.GetLength(0); i++)
            {
                for (int j = 0; j < inters.GetLength(1); j++)
                {
                    string tem = inters[i, j];
                    if (intr.Equals(tem, StringComparison.InvariantCultureIgnoreCase))
                    {
                        id = i;
                        break;
                    }
                }

            }

            if (id == -1) id = 0;
            id += 22;
            fv[id] = 1;
            if (verbose)
                await context.PostAsync("you are interested " + inters[id - 22, 0]);

            return fv;
        }

        static public void InsertRows(string row, QC.SqlConnection connection)
        {
            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"INSERT INTO [dbo].[Table]  VALUES  ("+row+");"; //NEED TO WORK ON THIS LINE!
                command.ExecuteScalar();               
            }
        }
    }
}