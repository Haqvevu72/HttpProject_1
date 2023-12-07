namespace MyClasses
{
    [Serializable]
    public class Car
    {
        public enum Status 
        {
            None,
            Modified,
            Added,
            Deleted
        }
        
        public int Id { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public ushort? Year { get; set; }

        public string? VIN { get; set; }

        public string? Color { get; set; }

        public Status status { get; set; }


        public override string ToString()
        {
            return $"\nId: {Id}\n" +
                   $"Make: {Make}\n" +
                   $"Model: {Model}\n" +
                   $"Year: {Year}\n" +
                   $"VIN: {VIN}\n" +
                   $"Color: {Color}\n" +
                   $"Status: {status.ToString()}";

        }
    }
}