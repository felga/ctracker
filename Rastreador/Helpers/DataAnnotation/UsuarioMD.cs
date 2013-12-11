using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Rastreador
{
    public partial class Usuario
    {
        public MembershipUser MembershipUser {
            get {
                return Membership.GetUser(this.Login);
            }
            set { }
        }
    }
}