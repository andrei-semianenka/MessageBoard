using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using MessageBoard.Models;

namespace MessageBoard.Entities
{
    public class MessagesDbContext : DbContext
    {
        public MessagesDbContext()
            : base("DefaultConnection")
        {
        }
        public System.Data.Entity.DbSet<MessageEntity> Messages { get; set; }
        public System.Data.Entity.DbSet<UserEntity> Users { get; set; }
    }
}