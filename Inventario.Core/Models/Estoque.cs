namespace Inventario.Core.Models
{
    public class Estoque
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}