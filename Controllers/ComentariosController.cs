using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDb => libroDb.Id == libroId);
            if (!existeLibro) return NotFound();
             
            var comentarios = await context.Comentarios.Where(x=>x.LibroId== libroId).ToListAsync();   
            return Ok(mapper.Map<ComentarioDTO>(comentarios));
        }

        [HttpGet("{id:int}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await context.Comentarios
                .FirstOrDefaultAsync(comen => comen.Id == id);

            if (comentario == null) return NotFound();

            return  mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDb => libroDb.Id == libroId);
            if (!existeLibro) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO); ;
            comentario.LibroId = libroId;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerComentario", new {id = comentario.Id, libroId}, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId,int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDb => libroDb.Id == libroId);
            if (!existeLibro) return NotFound();

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDb => comentarioDb.Id == id);
            if (!existeComentario) return NotFound();

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO); ;
            comentario.LibroId = libroId;
            comentario.Id = id;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
