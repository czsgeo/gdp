namespace Gdp
{
    public interface IXYZ
    {
        double Length { get; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }

        bool Equals(object obj);
        int GetHashCode();
        string ToString();
    }
}