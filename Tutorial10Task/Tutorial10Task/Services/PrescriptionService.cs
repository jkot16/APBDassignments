using Microsoft.EntityFrameworkCore;
using Tutorial10Task.Data;
using Tutorial10Task.Models;
using Tutorial10Task.Contracts.Responses;
using Tutorial10Task.Contracts.Requests;


namespace Tutorial10Task.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly AppDbContext _context;

    public PrescriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddPrescriptionAsync(PrescriptionRequest request, CancellationToken cancellationToken)
    {
        if (request.DueDate < request.Date)
            throw new ArgumentException("DueDate must be after Date");
        

       
        var doctor = await _context.Doctors.FindAsync(request.IdDoctor);

        
        var patient = await _context.Patients.FindAsync(request.Patient.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = request.Patient.IdPatient,
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                Birthdate = request.Patient.Birthdate
            };
            _context.Patients.Add(patient);
        }


        foreach (var m in request.Medicaments)
        {
            if (!await _context.Medicaments.AnyAsync(x => x.IdMedicament == m.IdMedicament))
                throw new Exception($"Medicament with ID {m.IdMedicament} not found");
        }

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            IdPatient = request.Patient.IdPatient,
            IdDoctor = request.IdDoctor,
            PrescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Details
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("EF ERROR: " + (ex.InnerException?.Message ?? ex.Message));
        }


        return prescription.IdPrescription;
    }
    public async Task<PatientResponse> GetPatientDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPatient == id);

        if (patient == null)
            throw new Exception("Patient not found");

        return new PatientResponse
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(p => p.DueDate)
                .Select(p => new PrescriptionDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName,
                        Email = p.Doctor.Email
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentInfoDto
                    {
                        IdMedicament = pm.Medicament.IdMedicament,
                        Name = pm.Medicament.Name,
                        Description = pm.Medicament.Description,
                        Dose = pm.Dose
                    }).ToList()
                }).ToList()
        };
    }

    
}