using System.Collections.Generic;

namespace BulletinBoard.Entities.Models
{
    public class Notification
    {
        public Notification()
        {
            
        }
        
        public int Id { get; set; }
        public string Text { get; set; }
        public List<UserNotification> UserNotification { get; set; }
    }
}