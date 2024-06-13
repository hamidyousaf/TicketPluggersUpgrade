using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TicketServer.Helpers;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Application.Common.Contracts.IServices;
using TP.Upgrade.Application.DTOs.Request;
using TP.Upgrade.Application.DTOs.Response;
using TP.Upgrade.Domain.Enums;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.Services
{
    public class ReportProblemService : IReportProblemService
    {
        private readonly IFileHelper _fileHelper;
        private readonly IReportProblemRepository _reportProblemRepository;
        private readonly ICustomerOrderRepository _customerOrderRepository;
        private IMapper _mapper;
        public ReportProblemService(
            IFileHelper fileHelper,
            IMapper mapper,
            IReportProblemRepository reportProblemRepository,
            ICustomerOrderRepository customerOrderRepository)
        {
            _fileHelper = fileHelper;
            _mapper = mapper;
            _reportProblemRepository = reportProblemRepository;
            _customerOrderRepository = customerOrderRepository;
        }
        public async Task<ResponseModel> ReportProblem(ReportProblemRequest request, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            var transaction = _reportProblemRepository.BeginTransaction();
            
            // add report.
            request.FileUrl = await _fileHelper.UploadFile("Static//ReportProblems", request.File);
            var report = _mapper.Map<ReportProblem>(request);
            report.ReferenceFile = request.FileUrl;
            report.ChatType = (byte)ChatType.ReportProblem;
            report.IsCustomerSend = true;
            var result = await _reportProblemRepository.InsertReportProblem(report);
            
            // update order status.
            if (request.OrderId > 0)
            {
                var order = await _customerOrderRepository.Get(request.OrderId);
                if (order is not null)
                {
                    order.OrderStatusId = (byte)OrderStatus.InProblem;
                    await _customerOrderRepository.Change(order);
                }
            }

            transaction.Commit();

            if (!result)
            {
                return new ResponseModel()
                {
                    IsSuccess = false,
                    Message = "There is something issue."
                };
            }
            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Your report has been sent."
            };
        }

        public async Task<ResponseModel> ReportProblemMutiple(List<long> orderIds, long customerId, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested) return new ResponseModel()
            {
                Message = "Your request is cancelled."
            };

            List<ReportProblem> reportProblems = new List<ReportProblem>();
            foreach (long orderId in orderIds)
            {
                ReportProblem reportProblem = new ReportProblem();
                reportProblem.CustomerId = customerId;
                reportProblem.Message = "";
                reportProblem.ReferenceLink = "";
                reportProblem.ChatType = (byte)ChatType.ReportProblem;
                reportProblem.IsCustomerSend = true;
                reportProblem.OrderId = orderId;
                reportProblems.Add(reportProblem);
            }
            await _reportProblemRepository.AddRangeAsync(reportProblems);

            // update order status.
            var orderlist = await _customerOrderRepository.GetOrdersByOrderIds(orderIds).ToListAsync(ct);
            orderlist.ForEach(x => x.OrderStatusId = (byte)OrderStatus.InProblem);
            await _customerOrderRepository.ChangeRange(orderlist);

            return new ResponseModel()
            {
                IsSuccess = true,
                Message = "Report added."
            };
        }
    }
}
