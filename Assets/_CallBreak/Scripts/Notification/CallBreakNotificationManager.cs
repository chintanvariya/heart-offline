//using System.Collections;
//using System.Collections.Generic;
//using Unity.Notifications.Android;
//using UnityEngine;

//namespace FGSOfflineCallBreak
//{
//    public class CallBreakNotificationManager : MonoBehaviour
//    {
//        // Start is called before the first frame update
//        void Start()
//        {
//            var channel = new AndroidNotificationChannel()
//            {
//                Id = "Channel_Id",
//                Name = "Default Channel",
//                Importance = Importance.Default,
//                Description = "Generic notification",
//            };

//            AndroidNotificationCenter.RegisterNotificationChannel(channel);

//            var notification = new AndroidNotification();
//            notification.Title = "Call Break: Your Move!";
//            notification.Text = "Get back to the table and claim your victory. Time to play!";
//            notification.SmallIcon = "icon_0";
//            notification.LargeIcon = "icon_1";

//            notification.FireTime = System.DateTime.Now.AddMinutes(120);

//            AndroidNotificationCenter.SendNotification(notification, "Channel_Id");
//        }
//    }
//}
////Title: "Big Win Awaits!"
////Message: "Dive into Black Jack now and test your luck at the tables! "

////Title: "Feeling Lucky?"
////Message: "Play Black Jack now and claim the jackpot!"

////Title: "Your Seat is Ready!"
////Message: " Jump back into Black Jack and outsmart the dealer! "
///

