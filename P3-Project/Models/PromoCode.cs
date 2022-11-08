namespace P3_Project.Models 
{
    public enum PromoCodeDiscountType {
        Percentage, Fixed
    }

    public class PromoCode
    {
        // THE ORDER OF FIELDS MATTERS FOR THE DATABASE!!
        public int Id {get; private set;}
        public string Code {get; set;}

        public PromoCodeDiscountType Type;
        public int PromoCodeType{ 
            get=>(int)Type; 
            set=>Type = (PromoCodeDiscountType)value;
        }

        public DateTime ExpirationDate{get; set;}

        public PromoCode(int id) {
            this.Id   = id;
            this.Code = "";
            this.Type = PromoCodeDiscountType.Percentage;
        }

        public PromoCode(int id, string code, int type, DateTime expirationDate){
            PromoCodeType = type;
            Code = code;
            Id = id;
            ExpirationDate = expirationDate;
        }
        
        public PromoCode(int id, string code, PromoCodeDiscountType type, DateTime expirationDate){
            Type = type;
            Code = code;
            Id = id;
            ExpirationDate = expirationDate;
        }

    }
}
