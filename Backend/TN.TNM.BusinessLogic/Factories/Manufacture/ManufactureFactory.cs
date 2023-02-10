using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TN.TNM.BusinessLogic.Interfaces.Manufacture;
using TN.TNM.BusinessLogic.Messages.Requests.Manufacture;
using TN.TNM.BusinessLogic.Messages.Responses.Manufacture;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Manufacture;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.BusinessLogic.Factories.Manufacture
{
    public class ManufactureFactory : BaseFactory, IManufacture
    {
        private IManufactureDataAccess iManufactureDataAccess;

        public ManufactureFactory(IManufactureDataAccess _iManufactureDataAccess, ILogger<ManufactureFactory> _logger)
        {
            this.iManufactureDataAccess = _iManufactureDataAccess;
            this.logger = _logger;
        }

        public GetMasterDataCreateProductOrderWorkflowResponse GetMasterDataCreateProductOrderWorkflow(
            GetMasterDataCreateProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataCreateProductOrderWorkflow(parameter);
                var response = new GetMasterDataCreateProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCode = result.ListCode
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateProductOrderWorkflowResponse CreateProductOrderWorkflow(CreateProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateProductOrderWorkflow(parameter);
                var response = new CreateProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductOrderWorkflowId = result.ProductOrderWorkflowId,
                    ListCode = result.ListCode
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CheckIsDefaultProductOrderWorkflowResponse CheckIsDefaultProductOrderWorkflow(
            CheckIsDefaultProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CheckIsDefaultProductOrderWorkflow(parameter);
                var response = new CheckIsDefaultProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    IsDefaultExists = result.IsDefaultExists
                };

                return response;
            }
            catch (Exception e)
            {
                return new CheckIsDefaultProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetProductOrderWorkflowByIdResponse GetProductOrderWorkflowById(GetProductOrderWorkflowByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetProductOrderWorkflowById(parameter);
                var response = new GetProductOrderWorkflowByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCode = result.ListCode,
                    ProductOrderWorkflow = new ProductOrderWorkflowModel(result.ProductOrderWorkflow),
                    ListOrderTechniqueMapping = new List<OrderTechniqueMappingModel>(),
                    ListTechniqueRequest = new List<TechniqueRequestModel>(),
                    ListIgnoreTechniqueRequest = new List<TechniqueRequestModel>()
                };

                result.ListOrderTechniqueMapping.ForEach(item =>
                {
                    response.ListOrderTechniqueMapping.Add(new OrderTechniqueMappingModel(item));
                });

                result.ListTechniqueRequest.ForEach(item =>
                {
                    response.ListTechniqueRequest.Add(new TechniqueRequestModel(item));
                });

                result.ListIgnoreTechniqueRequest.ForEach(item =>
                {
                    response.ListIgnoreTechniqueRequest.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetProductOrderWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductOrderWorkflowResponse UpdateProductOrderWorkflow(UpdateProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductOrderWorkflow(parameter);
                var response = new UpdateProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListCode = result.ListCode
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchProductOrderWorkflowResponse GetMasterDataSearchProductOrderWorkflow(
            GetMasterDataSearchProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataSearchProductOrderWorkflow(parameter);
                var response = new GetMasterDataSearchProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchProductOrderWorkflowResponse SearchProductOrderWorkflow(SearchProductOrderWorkflowRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.SearchProductOrderWorkflow(parameter);
                var response = new SearchProductOrderWorkflowResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductOrderWorkflowList = new List<ProductOrderWorkflowModel>()
                };

                result.ProductOrderWorkflowList.ForEach(item =>
                {
                    response.ProductOrderWorkflowList.Add(new ProductOrderWorkflowModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new SearchProductOrderWorkflowResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductOrderWorkflowDefaultResponse UpdateProductOrderWorkflowDefault(
            UpdateProductOrderWorkflowDefaultRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductOrderWorkflowDefault(parameter);
                var response = new UpdateProductOrderWorkflowDefaultResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductOrderWorkflowDefaultResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductOrderWorkflowActiveResponse UpdateProductOrderWorkflowActive(
            UpdateProductOrderWorkflowActiveRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductOrderWorkflowActive(parameter);
                var response = new UpdateProductOrderWorkflowActiveResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductOrderWorkflowActiveResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataCreateTechniqueRequestResponse GetMasterDataCreateTechniqueRequest(
            GetMasterDataCreateTechniqueRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataCreateTechniqueRequest(parameter);
                var response = new GetMasterDataCreateTechniqueRequestResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListOrganization = new List<OrganizationModel>(),
                    ListParent = new List<TechniqueRequestModel>()
                };

                result.ListOrganization.ForEach(item =>
                {
                    response.ListOrganization.Add(new OrganizationModel(item));
                });

                result.ListParent.ForEach(item =>
                {
                    response.ListParent.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateTechniqueRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateTechniqueRequestResponse CreateTechniqueRequest(CreateTechniqueRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateTechniqueRequest(parameter);
                var response = new CreateTechniqueRequestResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    TechniqueRequestId = result.TechniqueRequestId,
                    ListParent = new List<TechniqueRequestModel>()
                };

                result.ListParent.ForEach(item =>
                {
                    response.ListParent.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new CreateTechniqueRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchTechniqueRequestResponse GetMasterDataSearchTechniqueRequest(
            GetMasterDataSearchTechniqueRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataSearchTechniqueRequest(parameter);
                var response = new GetMasterDataSearchTechniqueRequestResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListOrganization = new List<OrganizationModel>(),
                    ListParent = new List<TechniqueRequestModel>()
                };

                result.ListOrganization.ForEach(item =>
                {
                    response.ListOrganization.Add(new OrganizationModel(item));
                });

                result.ListParent.ForEach(item =>
                {
                    response.ListParent.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchTechniqueRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchTechniqueRequestResponse SearchTechniqueRequest(SearchTechniqueRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.SearchTechniqueRequest(parameter);
                var response = new SearchTechniqueRequestResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    TechniqueRequestList = new List<TechniqueRequestModel>()
                };

                result.TechniqueRequestList.ForEach(item =>
                {
                    response.TechniqueRequestList.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new SearchTechniqueRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataCreateProductionOrderResponse GetMasterDataCreateProductionOrder(GetMasterDataCreateProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataCreateProductionOrder(parameter);
                var response = new GetMasterDataCreateProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    CustomerOrderObject = new CustomerOrderModel(result.Order),
                    ListCustomerOrderDetail = new List<CustomerOrderDetailModel>(),
                    ListMappingOrder = new List<MappingOrderTechniqueModel>(),
                    ListTechniqueRequest = new List<TechniqueRequestModel>()
                };
                result.ListOrderDetail.ForEach(cod =>
                {
                    response.ListCustomerOrderDetail.Add(new CustomerOrderDetailModel(cod));
                });

                result.ListMappingOrder.ForEach(item =>
                {
                    var mappingOrder = new MappingOrderTechniqueModel(item);
                    mappingOrder.ListTechniqueRequest = new List<TechniqueRequestModel>();
                    item.ListTechniqueRequest.ForEach(x =>
                    {
                        mappingOrder.ListTechniqueRequest.Add(new TechniqueRequestModel(x));
                    });
                    response.ListMappingOrder.Add(mappingOrder);
                });

                result.ListTechniqueRequest.ForEach(item =>
                {
                    response.ListTechniqueRequest.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataProductionOrderDetailResponse GetMasterDataProductionOrderDetail(GetMasterDataProductionOrderDetailRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataProductionOrderDetail(parameter);
                var response = new GetMasterDataProductionOrderDetailResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrder = new ProductionOrderModel(result.ProductionOrder),
                    ListNote = new List<NoteModel>(),
                    ListProductItem = new List<ProductionOrderMappingModel>(),
                    ListStatusItem = new List<CategoryModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListMappingOrder = new List<MappingOrderTechniqueModel>(),
                    ListTechniqueRequest = new List<TechniqueRequestModel>()
                };

                result.ListNote.ForEach(item =>
                {
                    response.ListNote.Add(new NoteModel(item));
                });

                result.ListProductItem.ForEach(item =>
                {
                    var product = new ProductionOrderMappingModel(item);
                    product.ListTechnique = new List<TechniqueRequestModel>();
                    item.ListTechnique.ForEach(x =>
                    {
                        product.ListTechnique.Add(new TechniqueRequestModel(x));
                    });

                    response.ListProductItem.Add(product);
                });

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListStatusItem.ForEach(item =>
                {
                    response.ListStatusItem.Add(new CategoryModel(item));
                });

                result.ListMappingOrder.ForEach(item =>
                {
                    var mappingOrder = new MappingOrderTechniqueModel(item);
                    mappingOrder.ListTechniqueRequest = new List<TechniqueRequestModel>();
                    item.ListTechniqueRequest.ForEach(x =>
                    {
                        mappingOrder.ListTechniqueRequest.Add(new TechniqueRequestModel(x));
                    });

                    response.ListMappingOrder.Add(mappingOrder);
                });

                result.ListTechniqueRequest.ForEach(item =>
                {

                    response.ListTechniqueRequest.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataProductionOrderDetailResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetAllProductionOrderResponse GetAllProductionOrder(GetAllProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetAllProductionOrder(parameter);
                var response = new GetAllProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProductionOrder = new List<ProductionOrderModel>()
                };

                result.ListProductionOrder.ForEach(item =>
                {
                    var productOrder = new ProductionOrderModel(item);
                    productOrder.TotalProductionOrderCode = item.TotalProductionOrderCode;
                    response.ListProductionOrder.Add(productOrder);
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetAllProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataCreateTotalProductionOrderResponse GetMasterDataCreateTotalProductionOrder(
            GetMasterDataCreateTotalProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataCreateTotalProductionOrder(parameter);
                var response = new GetMasterDataCreateTotalProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatus = new List<CategoryModel>(),
                    ListProductionOrder = new List<ProductionOrderModel>()
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListProductionOrder.ForEach(item =>
                {
                    response.ListProductionOrder.Add(new ProductionOrderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateTotalProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetTechniqueRequestByIdResponse GetTechniqueRequestById(GetTechniqueRequestByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetTechniqueRequestById(parameter);
                var response = new GetTechniqueRequestByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    TechniqueRequest = new TechniqueRequestModel(result.TechniqueRequest),
                    ListOrganization = new List<OrganizationModel>(),
                    ListParent = new List<TechniqueRequestModel>()
                };

                result.ListOrganization.ForEach(item =>
                {
                    response.ListOrganization.Add(new OrganizationModel(item));
                });

                result.ListParent.ForEach(item =>
                {
                    response.ListParent.Add(new TechniqueRequestModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetTechniqueRequestByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateTechniqueRequestResponse UpdateTechniqueRequest(UpdateTechniqueRequestRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateTechniqueRequest(parameter);
                var response = new UpdateTechniqueRequestResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateTechniqueRequestResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataListSearchProductionOrderResponse GetMasterDataListSearchProductionOrder(GetMasterDataListSearchProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataListSearchProductionOrder(parameter);
                var response = new GetMasterDataListSearchProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatus = new List<CategoryModel>(),
                    Organizations = new List<OrganizationModel>()
                };
                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.Organizations.ForEach(item =>
                {
                    response.Organizations.Add(new OrganizationModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataListSearchProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductionOrderEspeciallyResponse UpdateProductionOrderEspecially(UpdateProductionOrderEspeciallyRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductionOrderEspecially(parameter);
                var response = new UpdateProductionOrderEspeciallyResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductionOrderEspeciallyResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateTotalProductionOrderResponse CreateTotalProductionOrder(CreateTotalProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateTotalProductionOrder(parameter);
                var response = new CreateTotalProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    TotalProductionOrderId = result.TotalProductionOrderId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateTotalProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetTotalProductionOrderByIdResponse GetTotalProductionOrderById(GetTotalProductionOrderByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetTotalProductionOrderById(parameter);
                var response = new GetTotalProductionOrderByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    TotalProductionOrder = new TotalProductionOrderModel(result.TotalProductionOrder),
                    ListProductionOrder = new List<ProductionOrderModel>(),
                    ListStatus = new List<CategoryModel>(),
                    ListStatusItem = new List<CategoryModel>()
                };

                result.ListProductionOrder.ForEach(item =>
                {
                    response.ListProductionOrder.Add(new ProductionOrderModel(item));
                });

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                result.ListStatusItem.ForEach(item =>
                {
                    response.ListStatusItem.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetTotalProductionOrderByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateTotalProductionOrderResponse UpdateTotalProductionOrder(UpdateTotalProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateTotalProductionOrder(parameter);
                var response = new UpdateTotalProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateTotalProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataAddProductionOrderDialogResponse GetMasterDataAddProductionOrderDialog(
            GetMasterDataAddProductionOrderDialogRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataAddProductionOrderDialog(parameter);
                var response = new GetMasterDataAddProductionOrderDialogResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProductionOrder = new List<ProductionOrderModel>()
                };

                result.ListProductionOrder.ForEach(item =>
                {
                    response.ListProductionOrder.Add(new ProductionOrderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataAddProductionOrderDialogResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataSearchTotalProductionOrderResponse GetMasterDataSearchTotalProductionOrder(
            GetMasterDataSearchTotalProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataSearchTotalProductionOrder(parameter);
                var response = new GetMasterDataSearchTotalProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListStatus = new List<CategoryModel>()
                };

                result.ListStatus.ForEach(item =>
                {
                    response.ListStatus.Add(new CategoryModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchTotalProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchTotalProductionOrderResponse SearchTotalProductionOrder(SearchTotalProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.SearchTotalProductionOrder(parameter);
                var response = new SearchTotalProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListTotalProductionOrder = new List<TotalProductionOrderModel>(),
                    TotalRecords = result.TotalRecords
                };

                result.ListTotalProductionOrder.ForEach(item =>
                {
                    response.ListTotalProductionOrder.Add(new TotalProductionOrderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new SearchTotalProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetTrackProductionResponse GetTrackProduction(GetTrackProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetTrackProduction(parameter);
                var response = new GetTrackProductionResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListTrackProduction = new List<TrackProductionModel>(),
                    TotalRecords = result.TotalRecords,
                    ListProductionOrder = result.ListProductionOrder
                };

                result.ListTrackProduction.ForEach(item =>
                {
                    response.ListTrackProduction.Add(new TrackProductionModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetTrackProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateProductionOrderResponse CreateProductionOrder(CreateProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateProductionOrder(parameter);
                var response = new CreateProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrderId = result.ProductionOrderId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateStatusItemStopResponse UpdateStatusItemStop(UpdateStatusItemStopRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateStatusItemStop(parameter);
                var response = new UpdateStatusItemStopResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrderStatusId = result.ProductionOrderStatusId
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateStatusItemStopResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateStatusItemCancelResponse UpdateStatusItemCancel(UpdateStatusItemCancelRequest request)
        {

            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateStatusItemCancel(parameter);
                var response = new UpdateStatusItemCancelResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrderStatusId = result.ProductionOrderStatusId
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateStatusItemCancelResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductionOrderResponse UpdateProductionOrder(UpdateProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductionOrder(parameter);
                var response = new UpdateProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    StatusProdutionCode = result.StatusProdutionCode
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateStatusItemWorkingResponse UpdateStatusItemWorking(UpdateStatusItemWorkingRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateStatusItemWorking(parameter);
                var response = new UpdateStatusItemWorkingResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrderStatusId = result.ProductionOrderStatusId
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateStatusItemWorkingResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public PlusItemResponse PlusItem(PlusItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.PlusItem(parameter);
                var response = new PlusItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new PlusItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public MinusItemResponse MinusItem(MinusItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.MinusItem(parameter);
                var response = new MinusItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new MinusItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateItemInProductionResponse UpdateItemInProduction(UpdateItemInProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateItemInProduction(parameter);
                var response = new UpdateItemInProductionResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateItemInProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteItemInProductionResponse DeleteItemInProduction(DeleteItemInProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.DeleteItemInProduction(parameter);
                var response = new DeleteItemInProductionResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new DeleteItemInProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateItemInProductionResponse CreateItemInProduction(CreateItemInProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateItemInProduction(parameter);
                var response = new CreateItemInProductionResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ProductionOrderMappingId = result.ProductionOrderMappingId
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateItemInProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateWorkFlowForProductionOrderResponse UpdateWorkFlowForProductionOrder(UpdateWorkFlowForProductionOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateWorkFlowForProductionOrder(parameter);
                var response = new UpdateWorkFlowForProductionOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateWorkFlowForProductionOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataReportQuanlityControlResponse GetDataReportQuanlityControl(GetDataReportQuanlityControlRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetDataReportQuanlityControl(parameter);
                var response = new GetDataReportQuanlityControlResponse
                {
                    ListTechniqueRequest = new List<TechniqueRequestModel>(),
                    ListQualityControlNote = new List<CategoryModel>(),
                    ListErrorType = new List<CategoryModel>(),
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                result.ListTechniqueRequest.ForEach(e => response.ListTechniqueRequest.Add(new TechniqueRequestModel(e)));
                result.ListQualityControlNote.ForEach(e => response.ListQualityControlNote.Add(new CategoryModel(e)));
                result.ListErrorType.ForEach(e => response.ListErrorType.Add(new CategoryModel(e)));

                return response;
            }
            catch (Exception e)
            {
                return new GetDataReportQuanlityControlResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SearchQuanlityControlReportResponse SearchQuanlityControlReport(SearchQuanlityControlReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.SearchQuanlityControlReport(parameter);
                var response = new SearchQuanlityControlReportResponse
                {
                    ListQuanlityControlReport = result.ListQuanlityControlReport,
                    ListTechniqueRequestReport = result.ListTechniqueRequestReport,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new SearchQuanlityControlReportResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataListItemDialogResponse GetMasterDataListItemDialog(GetMasterDataListItemDialogRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataListItemDialog(parameter);
                var response = new GetMasterDataListItemDialogResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListItem = new List<ProductionOrderMappingModel>()
                };

                result.ListItem.ForEach(item =>
                {
                    response.ListItem.Add(new ProductionOrderMappingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataListItemDialogResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductionOrderHistoryNoteQcAndErroTypeResponse UpdateProductionOrderHistoryNoteQcAndErroType(UpdateProductionOrderHistoryNoteQcAndErroTypeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductionOrderHistoryNoteQcAndErroType(parameter);
                var response = new UpdateProductionOrderHistoryNoteQcAndErroTypeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductionOrderHistoryNoteQcAndErroTypeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ExportManufactureReportResponse ExportManufactureReport(ExportManufactureReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.ExportManufactureReport(parameter);
                var response = new ExportManufactureReportResponse
                {
                    ListTechniqueRequestReport = result.ListTechniqueRequestReport,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new ExportManufactureReportResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataDialogListProductOrderResponse GetMasterDataDialogListProductOrder(GetMasterDataDialogListProductOrderRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataDialogListProductOrder(parameter);
                var response = new GetMasterDataDialogListProductOrderResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProduct = new List<ProductionOrderMappingModel>()
                };

                result.ListProduct.ForEach(item =>
                {
                    response.ListProduct.Add(new ProductionOrderMappingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataDialogListProductOrderResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateProductionOrderAdditionalResponse CreateProductionOrderAdditional(CreateProductionOrderAdditionalRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateProductionOrderAdditional(parameter);
                var response = new CreateProductionOrderAdditionalResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateProductionOrderAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataReportManufactureResponse GetDataReportManufacture(GetDataReportManufactureRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetDataReportManufacture(parameter);
                var response = new GetDataReportManufactureResponse
                {
                    ListTechniqueRequest = result.ListTechniqueRequest,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataReportManufactureResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetReportManuFactureByDayResponse GetReportManuFactureByDay(GetReportManuFactureByDayRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetReportManuFactureByDay(parameter);
                var response = new GetReportManuFactureByDayResponse
                {
                    ListReportManuFactureByDay = result.ListReportManuFactureByDay,
                    SumaryReportManuFactureByDay = result.SumaryReportManuFactureByDay,
                    //ListReportManuFactureByDayRemain = result.ListReportManuFactureByDayRemain,
                    //SumaryReportManuFactureByDayRemain = result.SumaryReportManuFactureByDayRemain,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetReportManuFactureByDayResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetReportManuFactureByMonthResponse GetReportManuFactureByMonth(GetReportManuFactureByMonthRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetReportManuFactureByMonth(parameter);
                var response = new GetReportManuFactureByMonthResponse
                {
                    ListReportManuFactureByMonth = result.ListReportManuFactureByMonth,
                    SumaryReportManuFactureByMonth = result.SumaryReportManuFactureByMonth,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetReportManuFactureByMonthResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetReportManuFactureByYearResponse GetReportManuFactureByYear(GetReportManuFactureByYearRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetReportManuFactureByYear(parameter);
                var response = new GetReportManuFactureByYearResponse
                {
                    ListReportManuFactureByYear = result.ListReportManuFactureByYear,
                    SumaryReportManuFactureByYear = result.SumaryReportManuFactureByYear,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetReportManuFactureByYearResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetReportManuFactureByMonthAdditionalResponse GetReportManuFactureByMonthAdditional(GetReportManuFactureByMonthAdditionalRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetReportManuFactureByMonthAdditional(parameter);
                var response = new GetReportManuFactureByMonthAdditionalResponse
                {
                    ListReportManuFactureByMonth = result.ListReportManuFactureByMonth,
                    SumaryReportManuFactureByMonth = result.SumaryReportManuFactureByMonth,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetReportManuFactureByMonthAdditionalResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataExportTrackProductionResponse GetDataExportTrackProduction(GetDataExportTrackProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetDataExportTrackProduction(parameter);
                var response = new GetDataExportTrackProductionResponse
                {
                    ListProductionOrder = result.ListProductionOrder,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetDataExportTrackProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListChildrentItemResponse GetListChildrentItem(GetListChildrentItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetListChildrentItem(parameter);
                var response = new GetListChildrentItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListProductionOrderMapping = new List<ProductionOrderMappingModel>()
                };

                result.ListProductionOrderMapping.ForEach(item =>
                {
                    response.ListProductionOrderMapping.Add(new ProductionOrderMappingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetListChildrentItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataTrackProductionResponse GetMasterDataTrackProduction(GetMasterDataTrackProductionRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataTrackProduction(parameter);
                var response = new GetMasterDataTrackProductionResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    OrganizationCode = result.OrganizationCode,
                    OrganizationName = result.OrganizationName,
                    CurrentTime = result.CurrentTime,
                    ListStatusItem = new List<CategoryModel>(),
                    ListProductionOrder = new List<ProductionOrderModel>()
                };

                result.ListStatusItem.ForEach(item =>
                {
                    response.ListStatusItem.Add(new CategoryModel(item));
                });

                result.ListProductionOrder.ForEach(item =>
                {
                    response.ListProductionOrder.Add(new ProductionOrderModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataTrackProductionResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateRememberItemResponse CreateRememberItem(CreateRememberItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateRememberItem(parameter);
                var response = new CreateRememberItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new CreateRememberItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataViewRememberItemDialogResponse GetMasterDataViewRememberItemDialog(
            GetMasterDataViewRememberItemDialogRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetMasterDataViewRememberItemDialog(parameter);
                var response = new GetMasterDataViewRememberItemDialogResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListRememberItem = new List<RememberItemModel>()
                };

                result.ListRememberItem.ForEach(item =>
                {
                    response.ListRememberItem.Add(new RememberItemModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetMasterDataViewRememberItemDialogResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateRememberItemResponse UpdateRememberItem(UpdateRememberItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateRememberItem(parameter);
                var response = new UpdateRememberItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new UpdateRememberItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetDataDasboardManufactureResponse GetDataDasboardManufacture(GetDataDasboardManufactureRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetDataDasboardManufacture(parameter);
                var response = new GetDataDasboardManufactureResponse
                {
                    ListTechniqueRequest = new List<TechniqueRequestModel>(),
                    ListDelayProductionOrder = result.ListDelayProductionOrder,
                    TotalArea = result.TotalArea,
                    TotalCompleteArea = result.TotalCompleteArea,
                    ListProductionOrderInDay = new List<ProductionOrderInDayModel>(),
                    SumaryDashboard = result.SumaryDashboard,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };

                result.ListTechniqueRequest.ForEach(item =>
                {
                    response.ListTechniqueRequest.Add(new TechniqueRequestModel(item));
                });

                result.ListProductionOrderInDay.ForEach(item =>
                {
                    response.ListProductionOrderInDay.Add(new ProductionOrderInDayModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetDataDasboardManufactureResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetTrackProductionReportResponse GetTrackProductionReport(GetTrackProductionReportRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetTrackProductionReport(parameter);
                var response = new GetTrackProductionReportResponse
                {
                    ListTrackProductionReport = result.ListTrackProductionReport,
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new GetTrackProductionReportResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public CreateAllBTPResponse CreateAllBTP(CreateAllBTPRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.CreateAllBTP(parameter);
                var response = new CreateAllBTPResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new CreateAllBTPResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateProductionOrderNoteResponse UpdateProductionOrderNote(UpdateProductionOrderNoteRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.UpdateProductionOrderNote(parameter);
                var response = new UpdateProductionOrderNoteResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new UpdateProductionOrderNoteResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public PlusListItemResponse PlusListItem(PlusListItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.PlusListItem(parameter);
                var response = new PlusListItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new PlusListItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public ChangeProductGroupCodeByItemResponse ChangeProductGroupCodeByItem(ChangeProductGroupCodeByItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.ChangeProductGroupCodeByItem(parameter);
                var response = new ChangeProductGroupCodeByItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new ChangeProductGroupCodeByItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public MinusQuantityForItemResponse MinusQuantityForItem(MinusQuantityForItemRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.MinusQuantityForItem(parameter);
                var response = new MinusQuantityForItemResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                };
                return response;
            }
            catch (Exception e)
            {
                return new MinusQuantityForItemResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListItemChangeResponse GetListItemChange(GetListItemChangeRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetListItemChange(parameter);
                var response = new GetListItemChangeResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListItem = new List<ProductionOrderMappingModel>()
                };

                result.ListItem.ForEach(item =>
                {
                    response.ListItem.Add(new ProductionOrderMappingModel(item));
                });

                return response;
            }
            catch (Exception e)
            {
                return new GetListItemChangeResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public SaveCatHaResponse SaveCatHa(SaveCatHaRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.SaveCatHa(parameter);
                var response = new SaveCatHaResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message
                };

                return response;
            }
            catch (Exception e)
            {
                return new SaveCatHaResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }

        public GetListWorkflowByIdResponse GetListWorkflowById(GetListWorkflowByIdRequest request)
        {
            try
            {
                var parameter = request.ToParameter();
                var result = iManufactureDataAccess.GetListWorkflowById(parameter);
                var response = new GetListWorkflowByIdResponse
                {
                    StatusCode = result.Status ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                    MessageCode = result.Message,
                    ListWorkflow = result.ListWorkflow
                };

                return response;
            }
            catch (Exception e)
            {
                return new GetListWorkflowByIdResponse()
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    MessageCode = e.Message
                };
            }
        }
    }
}
