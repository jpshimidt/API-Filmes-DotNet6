using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.DTOs;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmeController : ControllerBase
    {
        private readonly FilmeContext _context;
        private readonly IMapper _mapper;

        public FilmeController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um filme ao banco de dados
        /// </summary>
        /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>
        [HttpPost]
        public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
        {
            Filme filme = _mapper.Map<Filme>(filmeDto);
            _context.Filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFilmesPorId), new { id = filme.Id }, filme);

        }

        /// <summary>
        /// Retorna todos os filmes do banco de dados
        /// </summary>
        /// <param name="skip">Mostrar a partir de qual ID</param>
        /// <param name="take">Quantos filmes deve retornar</param>
        /// <returns>IActionResult</returns>
        /// <response code="200">Caso retorno com sucesso</response>
        [HttpGet]
        public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
        }

        /// <summary>
        /// Retorna um filme especifico do banco de dados
        /// </summary>
        /// <param name="id">Id do filme que deve retornar</param>
        /// <returns>IActionResult</returns>
        /// <response code="200">Caso retorno com sucesso</response>
        /// <response code="404">Caso não tenha sido encontrado um filme com o Id fornecido</response>
        [HttpGet("{id}")]
        public IActionResult RecuperaFilmesPorId(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

            if (filme == null)
            {
                return NotFound();
            }

            var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
            return Ok(filmeDto);
        }

        /// <summary>
        /// Atuliza todos campos de um filme a partir de um Id
        /// </summary>
        /// <param name="id">Id do filme que deve ser atualizado</param>
        /// <param name="filmeDto">Objeto com os campos necessários para atualização de um filme</param>
        /// <returns>IActionResult</returns>
        /// <response code="204">Caso atualizado com sucesso</response>
        /// <response code="404">Caso não tenha sido encontrado um filme com o Id fornecido</response>
        [HttpPut("{id}")]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

            if(filme is null){
                return NotFound();
            }

            _mapper.Map(filmeDto, filme);
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Atuliza o campo especifico de um filme a partir de um Id
        /// </summary>
        /// <param name="id">Id do filme que deve ser atualizado</param>
        /// <param name="patch">Objeto com os campos solicitados para atualização de um filme</param>
        /// <returns>IActionResult</returns>
        /// <response code="204">Caso atualizado com sucesso</response>
        /// <response code="404">Caso não tenha sido encontrado um filme com o Id fornecido</response>
        [HttpPatch("{id}")]
        public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

            if (filme is null)
            {
                return NotFound();
            }

            var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
            patch.ApplyTo(filmeParaAtualizar, ModelState);

            if (!TryValidateModel(filmeParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Deleta um filme especifico do banco de dados
        /// </summary>
        /// <param name="id">Id do filme que deve ser deletado</param>
        /// <returns>IActionResult</returns>
        /// <response code="204">Caso retorno com sucesso</response>
        /// <response code="404">Caso não tenha sido encontrado um filme com o Id fornecido</response>
        [HttpDelete("{id}")]
        public IActionResult DeletaFilme(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

            if (filme is null)
            {
                return NotFound();
            }

            _context.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
