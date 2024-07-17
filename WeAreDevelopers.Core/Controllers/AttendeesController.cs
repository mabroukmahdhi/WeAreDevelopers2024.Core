// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;
using WeAreDevelopers.Core.Services.Foundations.Attendees;

namespace WeAreDevelopers.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendeesController(IAttendeeService AttendeeService) : RESTFulController
    {
        private readonly IAttendeeService AttendeeService = AttendeeService;

        [HttpPost]
        public async ValueTask<ActionResult<Attendee>> PostAttendeeAsync(Attendee Attendee)
        {
            try
            {
                Attendee addedAttendee =
                    await this.AttendeeService.AddAttendeeAsync(Attendee);

                return Created(addedAttendee);
            }
            catch (AttendeeValidationException AttendeeValidationException)
            {
                return BadRequest(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeValidationException)
                when (AttendeeValidationException.InnerException is InvalidAttendeeReferenceException)
            {
                return FailedDependency(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeDependencyValidationException)
               when (AttendeeDependencyValidationException.InnerException is AlreadyExistsAttendeeException)
            {
                return Conflict(AttendeeDependencyValidationException.InnerException);
            }
            catch (AttendeeDependencyException AttendeeDependencyException)
            {
                return InternalServerError(AttendeeDependencyException);
            }
            catch (AttendeeServiceException AttendeeServiceException)
            {
                return InternalServerError(AttendeeServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Attendee>> GetAllAttendees()
        {
            try
            {
                IQueryable<Attendee> retrievedAttendees =
                    this.AttendeeService.RetrieveAllAttendees();

                return Ok(retrievedAttendees);
            }
            catch (AttendeeDependencyException AttendeeDependencyException)
            {
                return InternalServerError(AttendeeDependencyException);
            }
            catch (AttendeeServiceException AttendeeServiceException)
            {
                return InternalServerError(AttendeeServiceException);
            }
        }

        [HttpGet("{AttendeeId}")]
        public async ValueTask<ActionResult<Attendee>> GetAttendeeByIdAsync(Guid AttendeeId)
        {
            try
            {
                Attendee Attendee = await this.AttendeeService.RetrieveAttendeeByIdAsync(AttendeeId);

                return Ok(Attendee);
            }
            catch (AttendeeValidationException AttendeeValidationException)
                when (AttendeeValidationException.InnerException is NotFoundAttendeeException)
            {
                return NotFound(AttendeeValidationException.InnerException);
            }
            catch (AttendeeValidationException AttendeeValidationException)
            {
                return BadRequest(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyException AttendeeDependencyException)
            {
                return InternalServerError(AttendeeDependencyException);
            }
            catch (AttendeeServiceException AttendeeServiceException)
            {
                return InternalServerError(AttendeeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Attendee>> PutAttendeeAsync(Attendee Attendee)
        {
            try
            {
                Attendee modifiedAttendee =
                    await this.AttendeeService.ModifyAttendeeAsync(Attendee);

                return Ok(modifiedAttendee);
            }
            catch (AttendeeValidationException AttendeeValidationException)
                when (AttendeeValidationException.InnerException is NotFoundAttendeeException)
            {
                return NotFound(AttendeeValidationException.InnerException);
            }
            catch (AttendeeValidationException AttendeeValidationException)
            {
                return BadRequest(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeValidationException)
                when (AttendeeValidationException.InnerException is InvalidAttendeeReferenceException)
            {
                return FailedDependency(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeDependencyValidationException)
               when (AttendeeDependencyValidationException.InnerException is AlreadyExistsAttendeeException)
            {
                return Conflict(AttendeeDependencyValidationException.InnerException);
            }
            catch (AttendeeDependencyException AttendeeDependencyException)
            {
                return InternalServerError(AttendeeDependencyException);
            }
            catch (AttendeeServiceException AttendeeServiceException)
            {
                return InternalServerError(AttendeeServiceException);
            }
        }

        [HttpDelete("{AttendeeId}")]
        public async ValueTask<ActionResult<Attendee>> DeleteAttendeeByIdAsync(Guid AttendeeId)
        {
            try
            {
                Attendee deletedAttendee =
                    await this.AttendeeService.RemoveAttendeeByIdAsync(AttendeeId);

                return Ok(deletedAttendee);
            }
            catch (AttendeeValidationException AttendeeValidationException)
                when (AttendeeValidationException.InnerException is NotFoundAttendeeException)
            {
                return NotFound(AttendeeValidationException.InnerException);
            }
            catch (AttendeeValidationException AttendeeValidationException)
            {
                return BadRequest(AttendeeValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeDependencyValidationException)
                when (AttendeeDependencyValidationException.InnerException is LockedAttendeeException)
            {
                return Locked(AttendeeDependencyValidationException.InnerException);
            }
            catch (AttendeeDependencyValidationException AttendeeDependencyValidationException)
            {
                return BadRequest(AttendeeDependencyValidationException);
            }
            catch (AttendeeDependencyException AttendeeDependencyException)
            {
                return InternalServerError(AttendeeDependencyException);
            }
            catch (AttendeeServiceException AttendeeServiceException)
            {
                return InternalServerError(AttendeeServiceException);
            }
        }
    }
}