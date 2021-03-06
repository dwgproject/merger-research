using System;

namespace GravityZero.HuntingSupport.Repository.Model
{
    public class UserHunting
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public Guid HuntingId { get; set; }
        public virtual Hunting Hunting { get; set; }
    }
}