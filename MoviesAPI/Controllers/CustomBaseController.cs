using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System.Linq;

namespace MoviesAPI.Controllers
{
    public class CustomBaseController:ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApplicationDBContext context;

        public CustomBaseController(IMapper mapper,ApplicationDBContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        protected async Task<List<TDTO>> Get<TEntity,TDTO>() where TEntity : class
        {
            var entities = await context.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entities);
            return dtos;
        }
        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDTO pagination) where TEntity : class
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            await HttpContext.InsertPaginationHeaderInResponse(queryable, pagination.RecordsPerPage);
            var entities = await queryable.Paginate(pagination).AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class,IId
        {
            var queryable=context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(id,queryable);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id,IQueryable<TEntity>queryable) where TEntity : class, IId
        {
            var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<TDTO>(entity);
            return Ok(dto);
        }

        protected async Task<ActionResult> Post<TEntity, TCreationDTO,TRead>(TCreationDTO creationDTO, string routeName) where TEntity : class,IId
        {
            var entity = mapper.Map<TEntity>(creationDTO);
            context.Add(entity);
            await context.SaveChangesAsync();
            var readDTO = mapper.Map<GenreDTO>(entity);
            return new CreatedAtRouteResult(routeName, new { entity.Id }, readDTO);
        }

        protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity: class,IId, new()
        {
            var exists = context.Set<TEntity>().Any(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new TEntity() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

        public async Task<ActionResult> Put<TEntity,TGenerationDTO>(int id, TGenerationDTO genreCreation) where TEntity : class,IId
        {
            var entity = mapper.Map<TEntity>(genreCreation);
            entity.Id = id;
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntity,TPatchDTO>(int id, JsonPatchDocument<TPatchDTO> patchDocument) where TEntity : class,IId 
            where TPatchDTO:class
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            var patchDTO = mapper.Map<TPatchDTO>(entity);
            patchDocument.ApplyTo(patchDTO, ModelState);
            var isValid = TryValidateModel(patchDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(patchDTO, entity);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
