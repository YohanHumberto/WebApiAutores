namespace WebApiAutores.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get;}
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
