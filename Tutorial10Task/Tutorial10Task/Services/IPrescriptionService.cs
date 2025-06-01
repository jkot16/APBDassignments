using Tutorial10Task.Contracts.Requests;
using Tutorial10Task.Contracts.Responses;

namespace Tutorial10Task.Services;

public interface IPrescriptionService
{
    Task<int> AddPrescriptionAsync(PrescriptionRequest request, CancellationToken cancellationToken);
    Task<PatientResponse> GetPatientDetailsAsync(int id, CancellationToken cancellationToken);

}