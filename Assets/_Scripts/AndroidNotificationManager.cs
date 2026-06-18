using UnityEngine;
using Unity.Notifications.Android;

public class AndroidNotificationManager : MonoBehaviour
{
    private const string ChannelId = "game_notifications";

    void Start()
    {
        // 1. Initialize the notification channel
        // This is mandatory for Android 8.0 (Oreo) and above.
        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Game Notifications",
            Importance = Importance.Default,
            Description = "Generic notifications for game updates and reminders",
        };

        // Register the channel with the Android system
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    /// <summary>
    /// Call this method to send a notification immediately.
    /// </summary>
    public void SendSimpleNotification()
    {
        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Hello from Unity!";
        notification.Text = "This is a basic Android notification.";
        notification.FireTime = System.DateTime.Now; // Send it right now
        notification.SmallIcon = "icon_0"; // Make sure this matches your project settings

        AndroidNotificationCenter.SendNotification(notification, ChannelId);
        Debug.Log("Notification sent!");
    }

    /// <summary>
    /// Call this method to schedule a notification for the future.
    /// </summary>
    public int ScheduleNotification(int secondsFromNow)
    {
        var notification = new AndroidNotification();
        notification.Title = "Reminder!";
        notification.Text = "You scheduled this notification to appear.";
        notification.FireTime = System.DateTime.Now.AddSeconds(secondsFromNow);

        int id = AndroidNotificationCenter.SendNotification(notification, ChannelId);
        Debug.Log($"Notification scheduled in {secondsFromNow} seconds.");
        return id;
    }
    
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllNotifications();
        Debug.Log("All notifications canceled.");
    }

    /// <summary>
    /// Cancels a specific notification using its unique ID.
    /// Note: You must capture the ID returned by SendNotification to use this.
    /// </summary>
    public void CancelNotificationById(int notificationId)
    {
        AndroidNotificationCenter.CancelNotification(notificationId);
        Debug.Log($"Notification {notificationId} canceled.");
    }
}