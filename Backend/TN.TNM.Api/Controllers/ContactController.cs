using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Contact;
using TN.TNM.BusinessLogic.Messages.Requests.Contact;
using TN.TNM.BusinessLogic.Messages.Responses.Contact;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Contact;
using TN.TNM.DataAccess.Messages.Results.Contact;

namespace TN.TNM.Api.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContact _iContact;
        private readonly IContactDataAccess _iContactDataAccess;
        public ContactController(IContact iContact, IContactDataAccess iContactDataAccess)
        {
            this._iContact = iContact;
            this._iContactDataAccess = iContactDataAccess;
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/create")]
        [Authorize(Policy = "Member")]
        public CreateContactResult CreateContact([FromBody]CreateContactParameter request)
        {
            return this._iContactDataAccess.CreateContact(request);
        }

        /// <summary>
        /// Get all Contact by ObjectType
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getAllContactByObjectType")]
        [Authorize(Policy = "Member")]
        public GetAllContactByObjectTypeResult GetAllContactByObjectType([FromBody]GetAllContactByObjectTypeParameter request)
        {
            return this._iContactDataAccess.GetAllContactByObjectType(request);
        }

        /// <summary>
        /// Search contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/searchContact")]
        [Authorize(Policy = "Member")]
        public SearchContactResult SearchContact([FromBody]SearchContactParameter request)
        {
            return this._iContactDataAccess.SearchContact(request);
        }

        /// <summary>
        /// Get contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getContactById")]
        [Authorize(Policy = "Member")]
        public GetContactByIdResult GetContactById([FromBody]GetContactByIdParameter request)
        {
            return this._iContactDataAccess.GetContactById(request);
        }

        /// <summary>
        /// Get contact by ObjectId
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/getContactByObjectId")]
        [Authorize(Policy = "Member")]
        public GetContactByIdResult GetContactByObjectId([FromBody]GetContactByObjectIdParameter request)
        {
            return this._iContactDataAccess.GetContactByObjectId(request);
        }
        /// <summary>
        /// Edit contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/editContactById")]
        [Authorize(Policy = "Member")]
        public EditContactByIdResult EditContactById([FromBody] EditContactByIdParameters request)
        {
            return this._iContactDataAccess.EditContactById(request);
        }

        /// <summary>
        /// Delete contact by Id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/deleteContactById")]
        [Authorize(Policy = "Member")]
        public DeleteContactByIdResult DeleteContactById([FromBody]DeleteContactByIdParameter request)
        {
            return this._iContactDataAccess.DeleteContactById(request);
        }

        //
        /// <summary>
        /// Update Personal Customer Contact
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contact/updatePersonalCustomerContact")]
        [Authorize(Policy = "Member")]
        public UpdatePersonalCustomerContactResult UpdatePersonalCustomerContact([FromBody]UpdatePersonalCustomerContactParameter request)
        {
            return this._iContactDataAccess.UpdatePersonalCustomerContact(request);
        }

        //
        [HttpPost]
        [Route("api/contact/getAddressByObject")]
        [Authorize(Policy = "Member")]
        public GetAddressByObjectResult GetAddressByObject([FromBody]GetAddressByObjectParameter request)
        {
            return this._iContactDataAccess.GetAddressByObject(request);
        }

        //
        [HttpPost]
        [Route("api/contact/getAddressByContactId")]
        [Authorize(Policy = "Member")]
        public GetAddressByContactIdResult GetAddressByContactId([FromBody]GetAddressByContactIdParameter request)
        {
            return this._iContactDataAccess.GetAddressByContactId(request);
        }

        //
        [HttpPost]
        [Route("api/contact/getAddressByChangeObject")]
        [Authorize(Policy = "Member")]
        public GetAddressByChangeObjectResult GetAddressByChangeObject([FromBody]GetAddressByChangeObjectParameter request)
        {
            return this._iContactDataAccess.GetAddressByChangeObject(request);
        }
    }
}
