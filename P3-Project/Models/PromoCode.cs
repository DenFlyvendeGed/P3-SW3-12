namespace P3_Project.Models 
{
    public enum PromoCodeDiscountType {
        Percentage, Fixed
    }

    public class PromoCode
    {
        public string Id {get; private set;}
        public string Code {get; set;}
        public PromoCodeDiscountType PromoCodeType{ get; set; }

        public PromoCode(string id) {
            this.Id = id;
            this.Code = "";
            this.PromoCodeType = PromoCodeDiscountType.Percentage;
        }
    }
}
