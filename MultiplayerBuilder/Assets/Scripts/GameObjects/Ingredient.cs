public class Ingredient
{
    public bool Contained;
    public ResourceSO ResourceSO;

    public Ingredient(ResourceSO resourceSO)
    {
        Contained = false;
        ResourceSO = resourceSO;
    }
}
