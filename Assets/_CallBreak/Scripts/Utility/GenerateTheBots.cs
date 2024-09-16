using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    [CreateAssetMenu(fileName = "SpriteData", menuName = "ManagerData/BotsSprites", order = 2)]
    [Serializable]
    public class SpriteData : ScriptableObject
    {
        public List<Sprite> allBotDetails;
    }
    public class GenerateTheBots : MonoBehaviour
    {

        public List<Sprite> allBotSprite = new List<Sprite>();

        public List<BotDetails> allBotDetails;

        //public ManageData managerData;
        //public SpriteData spriteData;

        private void Awake()
        {
            List<string> namesIN = new List<string> {
    "Raj",
    "Priya",
    "Rahul",
    "Aisha",
    "Arjun",
    "Neha",
    "Vikram",
    "Anjali",
    "Sanjay",
    "Pooja",
    "Karthik",
    "Deepika",
    "Amit",
    "Nisha",
    "Sunil",
    "Ritu",
    "Rohan",
    "Meera",
    "Akash",
    "Swati",
    "Ajay",
    "Shweta",
    "Vishal",
    "Kavita",
    "Manoj",
    "James",
    "Emily",
    "William",
    "Sophie",
    "Alexander",
    "Charlotte",
    "Thomas",
    "Olivia",
    "Daniel",
    "Amelia",
    "Jack",
    "Jessica",
    "Harry",
    "Grace",
    "George",
    "Chloe",
    "Edward",
    "Ella",
    "Michael",
    "Mia",
    "David",
    "Lily",
    "Matthew",
    "Isabella",
    "John",
    "John",
    "Emily",
    "Michael",
    "Jessica",
    "William",
    "Ashley",
    "David",
    "Sarah",
    "James",
    "Amanda",
    "Robert",
    "Jennifer",
    "Joseph",
    "Brittany",
    "Daniel",
    "Megan",
    "Christopher",
    "Lauren",
    "Matthew",
    "Samantha",
    "Andrew",
    "Nicole",
    "Ryan",
    "Elizabeth",
    "Brandon",
    "Ivan",
    "Svetlana",
    "Dmitry",
    "Natalia",
    "Sergei",
    "Yelena",
    "Alexei",
    "Olga",
    "Vladimir",
    "Tatiana",
    "Andrei",
    "Maria",
    "Nikolai",
    "Anna",
    "Mikhail",
    "Elena",
    "Alexander",
    "Irina",
    "Pavel",
    "Yulia",
    "Oleg",
    "Marina",
    "Anatoly",
    "Galina",
    "Yuri"
};
            List<string> randomStrings = GenerateRandomStrings(50, 7);
            List<int> allNumber = new List<int>();

            for (int i = 0; i < 20; i++)
            {
                allNumber.Add(i);
            }


            allNumber = allNumber.OrderBy(a => Guid.NewGuid()).ToList();

            for (int i = 0; i < 20; i++)
            {
                allNumber.Add(i);

                BotDetails botDetails = new BotDetails();
                botDetails.userName = namesIN[i];
                botDetails.userId = randomStrings[i];
                botDetails.userAvatarIndex = allNumber[i];


                if (i > 0 && i < 5)
                {
                    botDetails.userKeys = UnityEngine.Random.Range(75, 100);
                    botDetails.userChips = UnityEngine.Random.Range(100000, 1100000);
                }
                else if (i > 5 && i < 10)
                {
                    botDetails.userChips = UnityEngine.Random.Range(10000, 100000);

                    botDetails.userKeys = UnityEngine.Random.Range(50, 75);
                }
                else if (i > 10 && i < 20)
                {
                    botDetails.userChips = UnityEngine.Random.Range(1000, 10000);
                    botDetails.userKeys = UnityEngine.Random.Range(25, 50);
                }
                else
                {
                    botDetails.userKeys = UnityEngine.Random.Range(0, 25);
                    botDetails.userChips = UnityEngine.Random.Range(25, 1000);
                }

                //managerData.allBotDetails.Add(botDetails);
                //spriteData.allBotDetails.Add(allBotSprite[i]);
                allBotDetails.Add(botDetails);
                //allBotDetails.Add(allBotSprite[i]);
            }

            allBotDetails = allBotDetails.OrderBy(a => Guid.NewGuid()).ToList();
        }

        List<string> GenerateRandomStrings(int count, int length)
        {
            List<string> randomStrings = new List<string>();
            System.Random random = new System.Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (int i = 0; i < count; i++)
            {
                char[] stringChars = new char[length];
                for (int j = 0; j < length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }
                randomStrings.Add(new string(stringChars));
            }

            return randomStrings;
        }
    }
}
