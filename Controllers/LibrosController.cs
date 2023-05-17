using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name ="ObtenerLibros")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include(libroDb=> libroDb.AutoresLibros)
                .ThenInclude(autorLibroDb => autorLibroDb.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null) return NotFound();

            libro.AutoresLibros =  libro.AutoresLibros.OrderBy(x=>x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            var autoresIds = await context.Autores
                .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();


            if (libroCreacionDTO.AutoresIds == null) return BadRequest("No se puede crear un libro sin autores");

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No exite uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutor(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibros", new { id = libro.Id}, libroDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {

            var libroDb = await context.Libros.Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDb == null) return NotFound();

            libroDb = mapper.Map(libroCreacionDTO, libroDb);
            AsignarOrdenAutor(libroDb);


            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutor(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO>  patchDocument)
        {
            if (patchDocument == null) return BadRequest();

            var libroDB = await context.Libros.FirstOrDefaultAsync(X => X.Id == id);
            if (libroDB == null) return NotFound();

                var libroPatchDTO = mapper.Map<LibroPatchDTO>(libroDB);
            patchDocument.ApplyTo(libroPatchDTO, ModelState);

            var esValido = TryValidateModel(libroPatchDTO);
            if (!esValido) return BadRequest(ModelState);

            mapper.Map(libroPatchDTO, libroDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe) return NotFound();

            context.Remove(new Libro { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }

}
