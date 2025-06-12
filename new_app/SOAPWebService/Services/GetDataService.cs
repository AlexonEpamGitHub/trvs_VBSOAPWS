using System.ServiceModel;
using SOAPWebService.Models;

namespace SOAPWebService.Services
{
    /// <summary>
    /// Service implementation for data retrieval operations using modern async patterns
    /// </summary>
    public class GetDataService : IGetDataService
    {
        private readonly ILogger<GetDataService> _logger;

        /// <summary>
        /// Initializes a new instance of the GetDataService class
        /// </summary>
        /// <param name="logger">The logger instance for structured logging</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
        public GetDataService(ILogger<GetDataService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns a simple "Hello World" greeting asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a greeting string.</returns>
        public async Task<string> HelloWorldAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HelloWorldAsync method called at {Timestamp}", DateTime.UtcNow);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            // Simulate async operation
            await Task.Delay(1, cancellationToken);
            
            _logger.LogInformation("HelloWorldAsync method completed successfully");
            return "Hello World";
        }

        /// <summary>
        /// Returns personalized greeting data asynchronously
        /// </summary>
        /// <param name="name">The name to include in the greeting</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a personalized greeting string.</returns>
        public async Task<string> GetDataAsync(string name, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetDataAsync method called with name: {Name} at {Timestamp}", name ?? "null", DateTime.UtcNow);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                // Simulate async operation
                await Task.Delay(1, cancellationToken);
                
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("GetDataAsync called with null or empty name parameter");
                    return "Hello Guest, this is a simple SOAP web service response.";
                }
                
                var result = $"Hello {name}, this is a simple SOAP web service response.";
                _logger.LogInformation("GetDataAsync method completed successfully for name: {Name}", name);
                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetDataAsync operation was cancelled for name: {Name}", name ?? "null");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetDataAsync for name: {Name}", name ?? "null");
                throw;
            }
        }

        /// <summary>
        /// Retrieves sample data set asynchronously using modern DTO pattern
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains response DTO with sample data.</returns>
        public async Task<ResponseDto> GetDataSetAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GetDataSetAsync method called at {Timestamp}", DateTime.UtcNow);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                // Simulate async database operation
                await Task.Delay(10, cancellationToken);
                
                var dataItems = new List<DataItem>
                {
                    new DataItem { Id = 1, Name = "Alice", Description = "Sample user Alice", CreatedDate = DateTime.UtcNow.AddDays(-10) },
                    new DataItem { Id = 2, Name = "Bob", Description = "Sample user Bob", CreatedDate = DateTime.UtcNow.AddDays(-5) }
                };
                
                var response = new ResponseDto
                {
                    Success = true,
                    Message = "Data retrieved successfully",
                    Data = dataItems,
                    Timestamp = DateTime.UtcNow,
                    TotalCount = dataItems.Count
                };
                
                _logger.LogInformation("GetDataSetAsync completed successfully with {ItemCount} items", dataItems.Count);
                
                return response;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetDataSetAsync operation was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving data set");
                
                return new ResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving data: {ex.Message}",
                    Data = new List<DataItem>(),
                    Timestamp = DateTime.UtcNow,
                    TotalCount = 0
                };
            }
        }

        /// <summary>
        /// Generates report data asynchronously based on the provided input parameters
        /// </summary>
        /// <param name="reportInput">The report input parameters containing report configuration</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains response DTO with report data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when reportInput is null</exception>
        public async Task<ResponseDto> GetReportAsync(ReportInput reportInput, CancellationToken cancellationToken = default)
        {
            if (reportInput == null)
            {
                _logger.LogError("GetReportAsync called with null ReportInput parameter");
                throw new ArgumentNullException(nameof(reportInput), "ReportInput parameter cannot be null");
            }
            
            _logger.LogInformation("GetReportAsync method called with ReportName: {ReportName} at {Timestamp}", 
                reportInput.ReportName, DateTime.UtcNow);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                _logger.LogInformation("Processing report request for: {ReportName}", reportInput.ReportName);
                
                // Simulate async report generation
                await Task.Delay(50, cancellationToken);
                
                // Generate sample report data based on input
                var reportData = await GenerateReportDataAsync(reportInput, cancellationToken);
                
                var response = new ResponseDto
                {
                    Success = true,
                    Message = $"Report '{reportInput.ReportName}' generated successfully",
                    Data = reportData,
                    Timestamp = DateTime.UtcNow,
                    TotalCount = reportData.Count
                };
                
                _logger.LogInformation("GetReportAsync completed successfully for ReportName: {ReportName} with {ItemCount} items", 
                    reportInput.ReportName, reportData.Count);
                
                return response;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetReportAsync operation was cancelled for ReportName: {ReportName}", reportInput.ReportName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating report for ReportName: {ReportName}", reportInput.ReportName);
                
                return new ResponseDto
                {
                    Success = false,
                    Message = $"Error generating report '{reportInput.ReportName}': {ex.Message}",
                    Data = new List<DataItem>(),
                    Timestamp = DateTime.UtcNow,
                    TotalCount = 0
                };
            }
        }

        /// <summary>
        /// Generates sample report data based on the report input parameters
        /// </summary>
        /// <param name="reportInput">The report input parameters</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of data items.</returns>
        private async Task<List<DataItem>> GenerateReportDataAsync(ReportInput reportInput, CancellationToken cancellationToken)
        {
            // Simulate async data processing
            await Task.Delay(20, cancellationToken);
            
            var reportData = new List<DataItem>();
            
            // Generate sample data based on report name
            switch (reportInput.ReportName?.ToLowerInvariant())
            {
                case "users":
                    reportData.AddRange(new[]
                    {
                        new DataItem { Id = 1, Name = "Alice Johnson", Description = "Active user", CreatedDate = DateTime.UtcNow.AddDays(-30) },
                        new DataItem { Id = 2, Name = "Bob Smith", Description = "Premium user", CreatedDate = DateTime.UtcNow.AddDays(-15) },
                        new DataItem { Id = 3, Name = "Charlie Brown", Description = "New user", CreatedDate = DateTime.UtcNow.AddDays(-5) }
                    });
                    break;
                
                case "sales":
                    reportData.AddRange(new[]
                    {
                        new DataItem { Id = 1, Name = "Q1 Sales", Description = "$10,000", CreatedDate = DateTime.UtcNow.AddDays(-90) },
                        new DataItem { Id = 2, Name = "Q2 Sales", Description = "$15,000", CreatedDate = DateTime.UtcNow.AddDays(-60) },
                        new DataItem { Id = 3, Name = "Q3 Sales", Description = "$12,000", CreatedDate = DateTime.UtcNow.AddDays(-30) }
                    });
                    break;
                
                default:
                    reportData.AddRange(new[]
                    {
                        new DataItem { Id = 1, Name = "Default Item 1", Description = $"Generated for report: {reportInput.ReportName}", CreatedDate = DateTime.UtcNow },
                        new DataItem { Id = 2, Name = "Default Item 2", Description = $"Generated for report: {reportInput.ReportName}", CreatedDate = DateTime.UtcNow }
                    });
                    break;
            }
            
            return reportData;
        }
    }
}