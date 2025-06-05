namespace PracaDomowa10.DTOs;

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Datefrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public IEnumerable<CountryDTO> Countries { get; set; }
    public List<ClientDTO> Clients { get; set; }  

}

public class CountryDTO
{
    public string Name { get; set; }
}

public class ClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}