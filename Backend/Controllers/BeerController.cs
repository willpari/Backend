﻿using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeerController : ControllerBase
    {        
        private IValidator<BeerInsertDto> _beerInsertValidator;
        private IValidator<BeerUpdateDto> _beerUpdateValidator;
        private ICommonService<BeerDto, BeerInsertDto, BeerUpdateDto> _beerService;

        public BeerController(IValidator<BeerInsertDto> beerInsertValidator, 
            IValidator<BeerUpdateDto> beerUpdateValidator,
            [FromKeyedServices("beerService")] ICommonService<BeerDto, BeerInsertDto, BeerUpdateDto> beerService)
        {     
            _beerInsertValidator = beerInsertValidator;
            _beerUpdateValidator = beerUpdateValidator;
            _beerService = beerService;
        }

        [HttpGet]
        public async Task<IEnumerable<BeerDto>> Get() =>
            await _beerService.Get();

        [HttpGet("{id}")]
        public async Task<ActionResult<BeerDto>> GetBYId(int id)
        {
            var beerDto = await _beerService.GetById(id);

            return beerDto == null ? NotFound() : Ok(beerDto);
        }

        [HttpPost]

        public async Task<ActionResult<BeerDto>> Add(BeerInsertDto beerInserDto)
        {
            var validationResult = await _beerInsertValidator.ValidateAsync(beerInserDto);

            if (!validationResult.IsValid) 
            {
                return BadRequest(validationResult.Errors);
            }

            if (!_beerService.Validate(beerInserDto)) 
            {
                return BadRequest(_beerService.Errors);
            }

            var beerDto = await _beerService.Add(beerInserDto);

            return CreatedAtAction(nameof(GetBYId), new { id = beerDto.Id }, beerDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<BeerDto>> Update(int id, BeerUpdateDto beerUpdateDto) 
        {
            var validationResult = await _beerUpdateValidator.ValidateAsync(beerUpdateDto);
            if (!validationResult.IsValid) 
            {
                return BadRequest(validationResult.Errors);
            }

            if (!_beerService.Validate(beerUpdateDto))
            {
                return BadRequest(_beerService.Errors);
            }

            var beerDto = await _beerService.Update(id, beerUpdateDto);

            return beerDto == null ? NotFound() : Ok(beerDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BeerDto>> Delete(int id) 
        {
            var beerDto = await _beerService.Delete(id);

            return beerDto == null ? NotFound() : Ok(beerDto);
        }
    }
}
