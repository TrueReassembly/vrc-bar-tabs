using UdonSharp;

public class Drink : UdonSharpBehaviour
{
    public string Id { get; }
    public string Name { get; }
    public int Price { get; } // in copper
    
    public Drink(string name, int price)
    {
        Name = name;
        Price = price;
    }

    public Drink Create(string name, int price)
    {
        return gameObject.AddComponent<Drink>();
    }

}
