﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RunWebPage2024.Models
{
    public class ReplyLog
    {
        private int _id;
        private string _replyMessage;
        private long _telegramId;
        private DateTime _lastUpdated;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get => _id; set => _id = value; }
        public string ReplyMessage { get => _replyMessage; set => _replyMessage = value; }
        public long TelegramId { get => _telegramId; set => _telegramId = value; }
        public DateTime LastUpdated { get => _lastUpdated; set => _lastUpdated = value; }
    }
}
