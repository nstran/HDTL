using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Category;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Category;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Category;
using TN.TNM.DataAccess.Messages.Results.Admin.Company;

namespace TN.TNM.Api.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory iCategory;
        private readonly ICategoryDataAccess iCategoryDataAccess;
        public CategoryController(ICategory _iCategory, ICategoryDataAccess _iCategoryDataAccess)
        {
            this.iCategory = _iCategory;
            this.iCategoryDataAccess = _iCategoryDataAccess;
        }

        /// <summary>
        /// Get category info by categoty type code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getAllCategoryByCategoryTypeCode")]
        [Authorize(Policy = "Member")]
        public GetAllCategoryByCategoryTypeCodeResult GetAllCategoryByCategoryTypeCode([FromBody]GetAllCategoryByCategoryTypeCodeParameter request)
        {
            return this.iCategoryDataAccess.GetAllCategoryByCategoryTypeCode(request);
        }

        /// <summary>
        /// Get category info by categoty type code
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getAllCategory")]
        [Authorize(Policy = "Member")]
        public GetAllCategoryResult GetAllCategoryResponse([FromBody]GetAllCategoryParameter request)
        {
            return this.iCategoryDataAccess.GetAllCategory(request);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/getCategoryById")]
        [Authorize(Policy = "Member")]
        public GetCategoryByIdResult GetCategoryByIdResponse([FromBody]GetCategoryByIdParameter request)
        {
            return this.iCategoryDataAccess.GetCategoryById(request);
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/createCategory")]
        [Authorize(Policy = "Member")]
        public CreateCategoryResult CreateCategory([FromBody]CreateCategoryParameter request)
        {
            return this.iCategoryDataAccess.CreateCategory(request);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/deleteCategoryById")]
        [Authorize(Policy = "Member")]
        public DeleteCategoryByIdResult DeleteCategoryById([FromBody]DeleteCategoryByIdParameter request)
        {
            return this.iCategoryDataAccess.DeleteCategoryById(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/editCategoryById")]
        [Authorize(Policy = "Member")]
        public EditCategoryByIdResult Ed([FromBody]EditCategoryByIdParameter request)
        {
            return this.iCategoryDataAccess.EditCategoryById(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/updateStatusIsActive")]
        [Authorize(Policy = "Member")]
        public UpdateStatusIsActiveResult UpdateStatusIsActive([FromBody]UpdateStatusIsActiveParameter request)
        {
            return this.iCategoryDataAccess.UpdateStatusIsActive(request);
        }

        /// <summary>
        /// Edit a category
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/updateStatusIsDefault")]
        [Authorize(Policy = "Member")]
        public UpdateStatusIsDefaultResult UpdateStatusIsDefault([FromBody]UpdateStatusIsDefaultParameter request)
        {
            return this.iCategoryDataAccess.UpdateStatusIsDefault(request);
        }
    }
}