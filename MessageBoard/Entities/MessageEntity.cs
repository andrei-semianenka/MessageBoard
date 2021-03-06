﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageBoard.Entities
{
    [Table("Messages")]
    public class MessageEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
    }
}