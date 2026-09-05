namespace PetShop.DataContext.Entites
{
    public class AnimalType
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;       // Məs: "İt", "Pişik", "Quş"
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<ProductAnimalType> ProductAnimalTypes { get; set; } = new List<ProductAnimalType>();
    }
}

