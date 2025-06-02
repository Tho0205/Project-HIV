using System.ComponentModel.DataAnnotations;

namespace HIV.DTOs
{
    public class DTORegister
    {
        public class AccountRegisterDto
        {
            public class DTORegister
            {
                public int account_id { get; set; }
                public string username { get; set; }
                public string email { get; set; }
                public DateTime created_at { get; set; }
                public string full_name { get; set; }
                public string phone { get; set; }
                public string gender { get; set; }
                public DateTime? birthdate { get; set; }
                public string role { get; set; }
            }

        }

    }
}
