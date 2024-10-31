public class Ingredient
{
    public bool Contained;
    public PickupSO PickupSO;

    public Ingredient(PickupSO pickupSO)
    {
        Contained = false;
        PickupSO = pickupSO;
    }
}
