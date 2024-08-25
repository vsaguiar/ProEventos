using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.DTOs;

public class EventoDTO
{
    public int Id { get; set; }
    public string Local { get; set; }
    public string DataEvento { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MinLength(3, ErrorMessage = "{0} deve ter no mínimo 3 caracteres.")]
    [MaxLength(50, ErrorMessage = "{0} deve ter no máximo 50 caracteres.")]
    public string Tema { get; set; }

    [Display(Name = "Quantidade Pessoas")]
    [Range(1, 120000, ErrorMessage = "{0} não pode ser menor que 1 e maior que 120.000.")]
    public int QuantidadePessoas { get; set; }

    [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "Não é uma imagem válida. (gif, jpg, jpeg, bmp ou png)")]
    public string ImagemURL { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Phone(ErrorMessage = "O campo {0} está com número inválido.")]
    public string Telefone { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Display(Name = "e-mail")]
    [EmailAddress(ErrorMessage = "Endereço de e-mail inválido.")]
    public string Email { get; set; }

    public int UserId { get; set; }
    public UserDTO UserDTO { get; set; }
    
    public IEnumerable<LoteDTO> Lotes { get; set; }
    public IEnumerable<RedeSocialDTO> RedesSociais { get; set; }
    public IEnumerable<PalestranteDTO> PalestrantesEventos { get; set; }
}
