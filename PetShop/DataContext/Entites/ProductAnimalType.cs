namespace PetShop.DataContext.Entites
{
    public class ProductAnimalType
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int AnimalTypeId { get; set; }
        public AnimalType AnimalType { get; set; } = null!;
    }
}
