namespace Inventario.Core.Models
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<Estoque> Estoques { get; set; }

        public Usuario()
        {
            Estoques = new List<Estoque>();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}