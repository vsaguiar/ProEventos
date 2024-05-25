using System.Text.Json.Serialization;

namespace ProEventos.Domain;

public class RedeSocial
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string URL { get; set; }
    public int? EventoId { get; set; }

    [JsonIgnore]
    public Evento Evento { get; set; }
    public int? PalestranteId { get; set; }

    [JsonIgnore]
    public Palestrante Palestrante { get; set; }
}
